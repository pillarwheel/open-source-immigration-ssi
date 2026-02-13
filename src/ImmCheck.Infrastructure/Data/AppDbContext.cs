using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ImmCheck.Core.Models;
using ImmCheck.Core.SSI.Credentials;
using ImmCheck.Infrastructure.SSI.Credentials;

namespace ImmCheck.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Document tables
    public DbSet<I20> I20Programs { get; set; } = null!;
    public DbSet<DS2019> DS2019Infos { get; set; } = null!;
    public DbSet<I94> I94s { get; set; } = null!;
    public DbSet<Passport> Passports { get; set; } = null!;
    public DbSet<VisaInfo> VisaInfos { get; set; } = null!;
    public DbSet<SponsoredStudentInfo> SponsoredStudentInfos { get; set; } = null!;
    public DbSet<FinancialSupport> FinancialSupports { get; set; } = null!;
    public DbSet<Institution> Institutions { get; set; } = null!;

    // Credential tables
    public DbSet<IssuedCredentialRecord> IssuedCredentials { get; set; } = null!;
    public DbSet<StatusListRecord> StatusLists { get; set; } = null!;
    public DbSet<SigningKeyRecord> SigningKeys { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<SigningKeyRecord>(e =>
        {
            e.ToTable("SigningKeys");
            e.HasKey(k => k.IssuerDid);
        });
    }
}
