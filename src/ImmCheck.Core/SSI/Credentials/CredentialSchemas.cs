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

    /// <summary>
    /// Passport Credential — ICAO 9303 MRZ fields as verifiable claims.
    /// Enables selective disclosure: prove nationality without revealing document number.
    /// </summary>
    public static readonly CredentialSchemaDefinition PassportCredential = new()
    {
        Type = "PassportCredential",
        Description = "Passport Identity Document — ICAO 9303 MRZ fields as verifiable claims",
        RequiredClaims = new[]
        {
            "holderName",
            "nationality",
            "issuingState",
            "documentNumber",
            "dateOfBirth",
            "expirationDate",
            "sex"
        },
        OptionalClaims = new[]
        {
            "givenName",
            "surname",
            "passportType",
            "issuanceDate",
            "placeOfBirth",
            "mrzLine1",
            "mrzLine2"
        },
        DefaultSelectiveDisclosure = new[]
        {
            "documentNumber",
            "dateOfBirth",
            "mrzLine1",
            "mrzLine2"
        }
    };

    /// <summary>
    /// Visa Credential — U.S. visa stamp classification and validity as verifiable claims.
    /// </summary>
    public static readonly CredentialSchemaDefinition VisaCredential = new()
    {
        Type = "VisaCredential",
        Description = "U.S. Visa Stamp — visa classification and validity as verifiable claims",
        RequiredClaims = new[]
        {
            "holderName",
            "visaType",
            "issuingPost",
            "issueDate",
            "expirationDate"
        },
        OptionalClaims = new[]
        {
            "stampNumber",
            "controlNumber",
            "entryDate",
            "annotations",
            "numberOfEntries",
            "nationality"
        },
        DefaultSelectiveDisclosure = new[]
        {
            "stampNumber",
            "controlNumber",
            "nationality"
        }
    };

    /// <summary>
    /// DS-2019 Credential — Certificate of Eligibility for J-1 Exchange Visitor Status.
    /// </summary>
    public static readonly CredentialSchemaDefinition DS2019Credential = new()
    {
        Type = "DS2019Credential",
        Description = "DS-2019 Certificate of Eligibility for J-1 Exchange Visitor Status",
        RequiredClaims = new[]
        {
            "sevisId",
            "participantName",
            "programSponsor",
            "programNumber",
            "categoryCode",
            "programStartDate",
            "programEndDate"
        },
        OptionalClaims = new[]
        {
            "subjectField",
            "sponsorAddress",
            "officialName",
            "officialTitle",
            "issuanceDate"
        },
        DefaultSelectiveDisclosure = new[]
        {
            "sevisId",
            "participantName",
            "programStartDate",
            "programEndDate"
        }
    };

    /// <summary>
    /// I-94 Credential — Arrival/Departure Record admission status as verifiable claims.
    /// </summary>
    public static readonly CredentialSchemaDefinition I94Credential = new()
    {
        Type = "I94Credential",
        Description = "I-94 Arrival/Departure Record — admission status as verifiable claims",
        RequiredClaims = new[]
        {
            "holderName",
            "i94Number",
            "classOfAdmission",
            "admissionDate",
            "admittedUntil"
        },
        OptionalClaims = new[]
        {
            "portOfEntry",
            "departureDate",
            "durationOfStatus"
        },
        DefaultSelectiveDisclosure = new[]
        {
            "i94Number",
            "holderName"
        }
    };

    public static CredentialSchemaDefinition? GetSchema(string credentialType) => credentialType switch
    {
        "I20Credential" => I20Credential,
        "FinancialSupportCredential" => FinancialSupportCredential,
        "PassportCredential" => PassportCredential,
        "VisaCredential" => VisaCredential,
        "DS2019Credential" => DS2019Credential,
        "I94Credential" => I94Credential,
        _ => null
    };

    public static CredentialSchemaDefinition[] GetAllSchemas() => new[]
    {
        I20Credential,
        FinancialSupportCredential,
        PassportCredential,
        VisaCredential,
        DS2019Credential,
        I94Credential
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
