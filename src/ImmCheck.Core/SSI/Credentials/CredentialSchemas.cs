namespace ImmCheck.Core.SSI.Credentials;

/// <summary>
/// Defines the credential schemas (claim sets) for immigration document types.
/// Each schema specifies required claims, optional claims, and which claims
/// are selectively disclosable by default.
/// </summary>
public static class CredentialSchemas
{
    /// <summary>
    /// I-20 Credential — proves enrollment status in a U.S. educational institution.
    /// Issued by DSO (Designated School Official).
    /// </summary>
    public static readonly CredentialSchemaDefinition I20Credential = new()
    {
        Type = "I20Credential",
        Description = "I-20 Certificate of Eligibility for F-1 Student Status",
        RequiredClaims = new[]
        {
            "sevisId",
            "studentName",
            "programStatus",
            "educationLevel",
            "primaryMajor",
            "programStartDate",
            "programEndDate",
            "institutionName"
        },
        OptionalClaims = new[]
        {
            "secondMajor",
            "minor",
            "lengthOfStudy",
            "educationComments",
            "englishProficiencyRequired",
            "englishRequirementsMet",
            "issuanceDate",
            "institutionalKey"
        },
        DefaultSelectiveDisclosure = new[]
        {
            // SEVIS ID and student name are sensitive — selectively disclosable
            "sevisId",
            "studentName",
            // Dates can be sensitive
            "programStartDate",
            "programEndDate",
            "issuanceDate"
        }
    };

    /// <summary>
    /// Financial Support Credential — proves funding for studies.
    /// Can be issued by banks, scholarship orgs, or institutions.
    /// </summary>
    public static readonly CredentialSchemaDefinition FinancialSupportCredential = new()
    {
        Type = "FinancialSupportCredential",
        Description = "Financial Support Attestation for F-1 Student Visa",
        RequiredClaims = new[]
        {
            "sevisId",
            "studentName",
            "academicTerm",
            "totalExpenses",
            "totalFunding"
        },
        OptionalClaims = new[]
        {
            "tuition",
            "livingExpenses",
            "dependentExpenses",
            "otherExpenses",
            "otherExpensesDescription",
            "personalFunds",
            "schoolFunds",
            "schoolFundsDescription",
            "otherFunds",
            "otherFundsDescription",
            "employmentFunds",
            "remarks"
        },
        DefaultSelectiveDisclosure = new[]
        {
            "sevisId",
            "studentName",
            "personalFunds",
            "schoolFunds",
            "otherFunds",
            "employmentFunds",
            "tuition",
            "livingExpenses"
        }
    };

    public static CredentialSchemaDefinition? GetSchema(string credentialType) => credentialType switch
    {
        "I20Credential" => I20Credential,
        "FinancialSupportCredential" => FinancialSupportCredential,
        _ => null
    };
}

public class CredentialSchemaDefinition
{
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string[] RequiredClaims { get; set; } = Array.Empty<string>();
    public string[] OptionalClaims { get; set; } = Array.Empty<string>();
    public string[] DefaultSelectiveDisclosure { get; set; } = Array.Empty<string>();
}
