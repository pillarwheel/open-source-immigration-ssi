using ImmCheck.Core.Models;
using ImmCheck.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ImmCheck.Api.Data;

public static class SeedData
{
    public static async Task SeedDevelopmentDataAsync(AppDbContext db)
    {
        // Only seed if the Institutions table is empty (new schema)
        if (await db.Institutions.AnyAsync())
            return;

        // Seed institutions
        var institutions = new[]
        {
            new Institution { Id = 1, Name = "International University", SchoolCode = "IU-001", Did = "did:key:z6MkIntlUniv" },
            new Institution { Id = 2, Name = "Pacific Coast University", SchoolCode = "PCU-002", Did = "did:key:z6MkPacificCoast" },
            new Institution { Id = 3, Name = "Midwest Technical Institute", SchoolCode = "MTI-003", Did = "did:key:z6MkMidwestTech" }
        };
        db.Institutions.AddRange(institutions);

        // Seed financial support records
        if (!await db.FinancialSupports.AnyAsync())
        {
            db.FinancialSupports.AddRange(
                new FinancialSupport
                {
                    recnum = 1, idnumber = 1, sevisid = "N0001234567",
                    academicTerm = "Fall 2024", tuition = 25000, livingExpenses = 15000,
                    dependentExp = 0, personalFunds = 10000, schoolFundsAmount = 25000,
                    schoolFundsDesc = "Graduate Assistantship", employmentFunds = 5000,
                    datestamp = "2024-06-01"
                },
                new FinancialSupport
                {
                    recnum = 2, idnumber = 2, sevisid = "N0002345678",
                    academicTerm = "Fall 2024", tuition = 30000, livingExpenses = 18000,
                    dependentExp = 5000, personalFunds = 20000, schoolFundsAmount = 15000,
                    schoolFundsDesc = "Scholarship", otherFundsAmount = 18000,
                    otherFundsDesc = "Government Sponsorship", datestamp = "2024-06-15"
                }
            );
        }

        await db.SaveChangesAsync();
    }
}
