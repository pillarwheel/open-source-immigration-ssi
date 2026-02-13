using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using FluentValidation;
using FluentValidation.AspNetCore;
using ImmCheck.Core.Interfaces;
using ImmCheck.Core.Models;
using ImmCheck.Core.SSI;
using ImmCheck.Core.SSI.Credentials;
using ImmCheck.Infrastructure.Data;
using ImmCheck.Infrastructure.Repositories;
using ImmCheck.Infrastructure.SSI;
using ImmCheck.Infrastructure.SSI.Credentials;
using ImmCheck.Infrastructure.SSI.OID4VC;
using ImmCheck.Api.Data;
using ImmCheck.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

// EF Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? "ImmCheckDefaultSecretKey1234567890!";
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "ImmCheck",
        ValidAudience = builder.Configuration["Jwt:Audience"] ?? "ImmCheck",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

// Repositories
builder.Services.AddScoped<IDocumentRepository<I20>, DocumentRepository<I20>>();
builder.Services.AddScoped<IDocumentRepository<DS2019>, DocumentRepository<DS2019>>();
builder.Services.AddScoped<IDocumentRepository<I94>, DocumentRepository<I94>>();
builder.Services.AddScoped<IDocumentRepository<Passport>, DocumentRepository<Passport>>();
builder.Services.AddScoped<IDocumentRepository<VisaInfo>, DocumentRepository<VisaInfo>>();
builder.Services.AddScoped<IDocumentRepository<SponsoredStudentInfo>, DocumentRepository<SponsoredStudentInfo>>();
builder.Services.AddScoped<IDocumentRepository<FinancialSupport>, DocumentRepository<FinancialSupport>>();

// DID Resolvers
builder.Services.AddHttpClient<DidWebResolver>();
builder.Services.AddHttpClient<DidPrismResolver>();
builder.Services.AddHttpClient<DidCheqdResolver>();
builder.Services.AddHttpClient<DidMidnightResolver>();
builder.Services.AddSingleton<IDidResolver, DidKeyResolver>();
builder.Services.AddScoped<IDidResolver, DidWebResolver>();
builder.Services.AddScoped<IDidResolver, DidPrismResolver>();
builder.Services.AddScoped<IDidResolver, DidCheqdResolver>();
builder.Services.AddScoped<IDidResolver, DidMidnightResolver>();
builder.Services.AddSingleton<IDidManager, DidKeyManager>();
builder.Services.AddScoped<IDidManager, DidPrismResolver>();
builder.Services.AddScoped<IDidPublisher, DidPrismResolver>();
builder.Services.AddScoped<UniversalDidResolver>(sp =>
{
    var resolvers = sp.GetServices<IDidResolver>();
    var logger = sp.GetRequiredService<ILogger<UniversalDidResolver>>();
    var config = sp.GetRequiredService<IConfiguration>();
    var universalResolverUrl = config["UniversalResolver:Url"];

    HttpClient? universalResolverClient = null;
    if (!string.IsNullOrEmpty(universalResolverUrl))
    {
        var factory = sp.GetRequiredService<IHttpClientFactory>();
        universalResolverClient = factory.CreateClient("UniversalResolver");
    }

    return new UniversalDidResolver(resolvers, logger, universalResolverClient, universalResolverUrl);
});

// Credential services
builder.Services.AddScoped<IKeyStore, SqliteKeyStore>();
builder.Services.AddScoped<ICredentialRepository, CredentialRepository>();
builder.Services.AddScoped<ICredentialIssuer, SdJwtIssuer>();

// OID4VC stores (singleton — in-memory session state)
builder.Services.AddSingleton<OfferStore>();
builder.Services.AddSingleton<PresentationStore>();

// FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "ImmCheck API", Version = "v1",
        Description = "Immigration Document Management API with SSI/Verifiable Credentials support" });
});

var app = builder.Build();

// Auto-migrate database (creates Identity tables if missing)
// Skip migration for InMemory provider (used in tests)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (db.Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory")
    {
        await db.Database.MigrateAsync();
    }
}

// Seed roles
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    foreach (var role in AppRoles.All)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }
}

// Seed development data
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await SeedData.SeedDevelopmentDataAsync(db);
}

// Global exception handling
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Swagger
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ImmCheck API v1"));

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }
