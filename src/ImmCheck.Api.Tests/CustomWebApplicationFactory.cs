using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ImmCheck.Infrastructure.Data;
using ImmCheck.Core.Models;

namespace ImmCheck.Api.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the existing DbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (descriptor != null) services.Remove(descriptor);

            // Add in-memory database with unique name per factory instance
            var dbName = "TestDb_" + Guid.NewGuid().ToString("N");
            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase(dbName));

            // Seed test data
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();
            SeedTestData(db);
        });
    }

    private static void SeedTestData(AppDbContext db)
    {
        if (db.I20Programs.Any()) return;

        db.I20Programs.Add(new I20
        {
            recnum = 1, idnumber = 100, sevisid = "N0001234567",
            status = "AC", eduLevel = "MS", primaryMajor = "CS",
            datestamp = "2024-01-01", ipfsCID = "QmTest123"
        });
        db.DS2019Infos.Add(new DS2019
        {
            recnum = 1, idnumber = 100, sponsor = "Test University",
            orgCode = "P-1-00001", sevisid = "N0001234567",
            datestamp = "2024-01-01"
        });
        db.I94s.Add(new I94
        {
            recnum = 1, idnumber = 100, i94num = "123456789AB",
            i94poe = "JFK", datestamp = "2024-01-01"
        });
        db.Passports.Add(new Passport
        {
            recnum = 1, idnumber = 100, givenName = "John",
            lastname = "Doe", cpass = "US", passnum = "AB1234567",
            datestamp = "2024-01-01"
        });
        db.VisaInfos.Add(new VisaInfo
        {
            recnum = 1, idnumber = 100, status = "F-1",
            visastamp = "F1", datestamp = "2024-01-01"
        });
        db.SponsoredStudentInfos.Add(new SponsoredStudentInfo
        {
            recnum = 1, idnumber = 100, sponsor = "Fulbright",
            isSponsoredStudent = "Y", datestamp = "2024-01-01"
        });
        db.SaveChanges();
    }
}
