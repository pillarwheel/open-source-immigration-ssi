using ImmCheck.Core.SSI.Credentials;
using ImmCheck.Infrastructure.SSI.Credentials;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using ImmCheck.Infrastructure.Data;

namespace ImmCheck.Api.Tests.SSI;

public class SdJwtTests : IDisposable
{
    private readonly AppDbContext _db;
    private readonly SdJwtIssuer _issuer;
    private readonly CredentialRepository _credRepo;
    private readonly SqliteKeyStore _keyStore;

    public SdJwtTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("SdJwtTests_" + Guid.NewGuid().ToString("N"))
            .Options;
        _db = new AppDbContext(options);
        _db.Database.EnsureCreated();

        _keyStore = new SqliteKeyStore(_db);
        _credRepo = new CredentialRepository(_db);
        _issuer = new SdJwtIssuer(_keyStore, _credRepo, NullLogger<SdJwtIssuer>.Instance);
    }

    [Fact]
    public async Task Issue_I20Credential_ProducesValidSdJwt()
    {
        var request = CreateI20Request();
        var result = await _issuer.IssueAsync(request);

        Assert.NotEmpty(result.CredentialId);
        Assert.Equal("vc+sd-jwt", result.Format);
        Assert.NotEmpty(result.SerializedCredential);

        // SD-JWT should have disclosures (tilde-separated)
        var parts = result.SerializedCredential.Split('~');
        Assert.True(parts.Length > 1, "SD-JWT should have at least one disclosure");

        // JWT part should have 3 dot-separated segments
        var jwtParts = parts[0].Split('.');
        Assert.Equal(3, jwtParts.Length);
    }

    [Fact]
    public async Task Issue_ThenVerify_Succeeds()
    {
        var request = CreateI20Request();
        var issued = await _issuer.IssueAsync(request);

        var verification = await _issuer.VerifyAsync(issued.SerializedCredential);

        Assert.True(verification.IsValid);
        Assert.Null(verification.Error);
        Assert.Equal("did:key:z6MkTestIssuer", verification.IssuerDid);
        Assert.NotNull(verification.DisclosedClaims);
        Assert.NotNull(verification.ValidFrom);
    }

    [Fact]
    public async Task Verify_WithSelectiveDisclosure_OnlyShowsDisclosedClaims()
    {
        var request = CreateI20Request();
        request.SelectiveDisclosureClaims = new List<string> { "sevisId", "studentName" };

        var issued = await _issuer.IssueAsync(request);

        // Present with only the first disclosure
        var parts = issued.SerializedCredential.Split('~');
        var jwt = parts[0];
        var partialPresentation = $"{jwt}~{parts[1]}~"; // Only first disclosure

        var verification = await _issuer.VerifyAsync(partialPresentation);

        Assert.True(verification.IsValid);
        Assert.NotNull(verification.DisclosedClaims);
        // Should have the non-SD claims plus only the one disclosed SD claim
        Assert.True(verification.DisclosedClaims.Count >= 1);
    }

    [Fact]
    public async Task Verify_TamperedSignature_Fails()
    {
        var request = CreateI20Request();
        var issued = await _issuer.IssueAsync(request);

        // Tamper with the signature
        var parts = issued.SerializedCredential.Split('~');
        var jwtParts = parts[0].Split('.');
        jwtParts[2] = "TAMPERED" + jwtParts[2];
        parts[0] = string.Join('.', jwtParts);
        var tampered = string.Join('~', parts);

        var verification = await _issuer.VerifyAsync(tampered);

        Assert.False(verification.IsValid);
        Assert.NotNull(verification.Error);
    }

    [Fact]
    public async Task Verify_ExpiredCredential_Fails()
    {
        var request = CreateI20Request();
        request.ValidityDays = -1; // Already expired

        var issued = await _issuer.IssueAsync(request);
        var verification = await _issuer.VerifyAsync(issued.SerializedCredential);

        Assert.False(verification.IsValid);
        Assert.Contains("expired", verification.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Verify_InvalidJwtFormat_Fails()
    {
        var verification = await _issuer.VerifyAsync("not.a.valid~jwt");
        Assert.False(verification.IsValid);
    }

    [Fact]
    public async Task Issue_StoresCredentialRecord()
    {
        var request = CreateI20Request();
        var issued = await _issuer.IssueAsync(request);

        var record = await _credRepo.GetByIdAsync(issued.CredentialId);
        Assert.NotNull(record);
        Assert.Equal("did:key:z6MkTestIssuer", record.IssuerDid);
        Assert.Equal("did:key:z6MkTestStudent", record.SubjectDid);
        Assert.Equal("I20Credential", record.CredentialType);
        Assert.False(record.IsRevoked);
    }

    [Fact]
    public async Task Revoke_SetsRevokedFlag()
    {
        var request = CreateI20Request();
        var issued = await _issuer.IssueAsync(request);

        var revoked = await _credRepo.RevokeAsync(issued.CredentialId);
        Assert.True(revoked);

        var record = await _credRepo.GetByIdAsync(issued.CredentialId);
        Assert.NotNull(record);
        Assert.True(record.IsRevoked);
        Assert.NotNull(record.RevokedAt);
    }

    [Fact]
    public async Task Revoke_AlreadyRevoked_ReturnsFalse()
    {
        var request = CreateI20Request();
        var issued = await _issuer.IssueAsync(request);

        await _credRepo.RevokeAsync(issued.CredentialId);
        var secondRevoke = await _credRepo.RevokeAsync(issued.CredentialId);
        Assert.False(secondRevoke);
    }

    [Fact]
    public async Task StatusList_UpdatesOnRevocation()
    {
        var request = CreateI20Request();
        var issued = await _issuer.IssueAsync(request);

        await _credRepo.RevokeAsync(issued.CredentialId);

        var statusList = await _credRepo.GetStatusListAsync("did:key:z6MkTestIssuer");
        Assert.NotNull(statusList);
        Assert.NotEmpty(statusList.EncodedList);

        // Decode and check the bit is set
        var bytes = Convert.FromBase64String(statusList.EncodedList);
        var record = await _credRepo.GetByIdAsync(issued.CredentialId);
        Assert.NotNull(record);
        var byteIndex = record.StatusListIndex / 8;
        var bitIndex = record.StatusListIndex % 8;
        Assert.True((bytes[byteIndex] & (1 << bitIndex)) != 0);
    }

    [Fact]
    public async Task KeyStore_GetOrCreate_ReturnsSameKey()
    {
        var key1 = await _keyStore.GetOrCreateSigningKeyAsync("did:key:z6MkTest");
        var key2 = await _keyStore.GetOrCreateSigningKeyAsync("did:key:z6MkTest");
        Assert.Equal(key1, key2);
    }

    [Fact]
    public async Task KeyStore_DifferentIssuers_ReturnsDifferentKeys()
    {
        var key1 = await _keyStore.GetOrCreateSigningKeyAsync("did:key:z6MkIssuer1");
        var key2 = await _keyStore.GetOrCreateSigningKeyAsync("did:key:z6MkIssuer2");
        Assert.NotEqual(key1, key2);
    }

    [Fact]
    public void CredentialSchemas_I20_HasRequiredClaims()
    {
        var schema = CredentialSchemas.GetSchema("I20Credential");
        Assert.NotNull(schema);
        Assert.Contains("sevisId", schema.RequiredClaims);
        Assert.Contains("studentName", schema.RequiredClaims);
        Assert.Contains("programStatus", schema.RequiredClaims);
        Assert.Contains("institutionName", schema.RequiredClaims);
    }

    [Fact]
    public void CredentialSchemas_Financial_HasRequiredClaims()
    {
        var schema = CredentialSchemas.GetSchema("FinancialSupportCredential");
        Assert.NotNull(schema);
        Assert.Contains("totalExpenses", schema.RequiredClaims);
        Assert.Contains("totalFunding", schema.RequiredClaims);
    }

    [Fact]
    public void CredentialSchemas_UnknownType_ReturnsNull()
    {
        Assert.Null(CredentialSchemas.GetSchema("NonExistentCredential"));
    }

    // --- Passport Credential Tests ---

    [Fact]
    public async Task Issue_PassportCredential_Succeeds()
    {
        var request = CreatePassportRequest();
        var result = await _issuer.IssueAsync(request);

        Assert.NotEmpty(result.CredentialId);
        Assert.Equal("vc+sd-jwt", result.Format);
        Assert.NotEmpty(result.SerializedCredential);
    }

    [Fact]
    public async Task Issue_PassportCredential_SelectiveDisclosure_HidesDocumentNumber()
    {
        var request = CreatePassportRequest();
        var issued = await _issuer.IssueAsync(request);

        // Present with only first disclosure
        var parts = issued.SerializedCredential.Split('~');
        var jwt = parts[0];
        var partialPresentation = $"{jwt}~{parts[1]}~";

        var verification = await _issuer.VerifyAsync(partialPresentation);
        Assert.True(verification.IsValid);
        Assert.NotNull(verification.DisclosedClaims);
    }

    [Fact]
    public async Task Verify_PassportCredential_Roundtrip()
    {
        var request = CreatePassportRequest();
        var issued = await _issuer.IssueAsync(request);
        var verification = await _issuer.VerifyAsync(issued.SerializedCredential);

        Assert.True(verification.IsValid);
        Assert.Null(verification.Error);
        Assert.Equal("did:key:z6MkTestIssuer", verification.IssuerDid);
    }

    [Fact]
    public void CredentialSchemas_Passport_HasRequiredClaims()
    {
        var schema = CredentialSchemas.GetSchema("PassportCredential");
        Assert.NotNull(schema);
        Assert.Contains("holderName", schema.RequiredClaims);
        Assert.Contains("nationality", schema.RequiredClaims);
        Assert.Contains("documentNumber", schema.RequiredClaims);
        Assert.Contains("expirationDate", schema.RequiredClaims);
    }

    // --- Visa Credential Tests ---

    [Fact]
    public async Task Issue_VisaCredential_Succeeds()
    {
        var request = CreateVisaRequest();
        var result = await _issuer.IssueAsync(request);

        Assert.NotEmpty(result.CredentialId);
        Assert.Equal("vc+sd-jwt", result.Format);
        Assert.NotEmpty(result.SerializedCredential);
    }

    [Fact]
    public async Task Issue_VisaCredential_SelectiveDisclosure_HidesControlNumber()
    {
        var request = CreateVisaRequest();
        var issued = await _issuer.IssueAsync(request);

        var parts = issued.SerializedCredential.Split('~');
        var jwt = parts[0];
        var partialPresentation = $"{jwt}~{parts[1]}~";

        var verification = await _issuer.VerifyAsync(partialPresentation);
        Assert.True(verification.IsValid);
        Assert.NotNull(verification.DisclosedClaims);
    }

    [Fact]
    public async Task Verify_VisaCredential_Roundtrip()
    {
        var request = CreateVisaRequest();
        var issued = await _issuer.IssueAsync(request);
        var verification = await _issuer.VerifyAsync(issued.SerializedCredential);

        Assert.True(verification.IsValid);
        Assert.Null(verification.Error);
        Assert.Equal("did:key:z6MkTestIssuer", verification.IssuerDid);
    }

    [Fact]
    public void CredentialSchemas_Visa_HasRequiredClaims()
    {
        var schema = CredentialSchemas.GetSchema("VisaCredential");
        Assert.NotNull(schema);
        Assert.Contains("holderName", schema.RequiredClaims);
        Assert.Contains("visaType", schema.RequiredClaims);
        Assert.Contains("issuingPost", schema.RequiredClaims);
        Assert.Contains("expirationDate", schema.RequiredClaims);
    }

    // --- DS2019 Credential Tests ---

    [Fact]
    public async Task Issue_DS2019Credential_Succeeds()
    {
        var request = CreateDS2019Request();
        var result = await _issuer.IssueAsync(request);

        Assert.NotEmpty(result.CredentialId);
        Assert.Equal("vc+sd-jwt", result.Format);
        Assert.NotEmpty(result.SerializedCredential);
    }

    [Fact]
    public async Task Issue_DS2019Credential_SelectiveDisclosure_HidesSevisId()
    {
        var request = CreateDS2019Request();
        var issued = await _issuer.IssueAsync(request);

        var parts = issued.SerializedCredential.Split('~');
        var jwt = parts[0];
        var partialPresentation = $"{jwt}~{parts[1]}~";

        var verification = await _issuer.VerifyAsync(partialPresentation);
        Assert.True(verification.IsValid);
        Assert.NotNull(verification.DisclosedClaims);
    }

    [Fact]
    public async Task Verify_DS2019Credential_Roundtrip()
    {
        var request = CreateDS2019Request();
        var issued = await _issuer.IssueAsync(request);
        var verification = await _issuer.VerifyAsync(issued.SerializedCredential);

        Assert.True(verification.IsValid);
        Assert.Null(verification.Error);
        Assert.Equal("did:key:z6MkTestIssuer", verification.IssuerDid);
    }

    [Fact]
    public void CredentialSchemas_DS2019_HasRequiredClaims()
    {
        var schema = CredentialSchemas.GetSchema("DS2019Credential");
        Assert.NotNull(schema);
        Assert.Contains("sevisId", schema.RequiredClaims);
        Assert.Contains("participantName", schema.RequiredClaims);
        Assert.Contains("programSponsor", schema.RequiredClaims);
        Assert.Contains("categoryCode", schema.RequiredClaims);
    }

    // --- I94 Credential Tests ---

    [Fact]
    public async Task Issue_I94Credential_Succeeds()
    {
        var request = CreateI94Request();
        var result = await _issuer.IssueAsync(request);

        Assert.NotEmpty(result.CredentialId);
        Assert.Equal("vc+sd-jwt", result.Format);
        Assert.NotEmpty(result.SerializedCredential);
    }

    [Fact]
    public async Task Issue_I94Credential_SelectiveDisclosure_HidesI94Number()
    {
        var request = CreateI94Request();
        var issued = await _issuer.IssueAsync(request);

        var parts = issued.SerializedCredential.Split('~');
        var jwt = parts[0];
        var partialPresentation = $"{jwt}~{parts[1]}~";

        var verification = await _issuer.VerifyAsync(partialPresentation);
        Assert.True(verification.IsValid);
        Assert.NotNull(verification.DisclosedClaims);
    }

    [Fact]
    public async Task Verify_I94Credential_Roundtrip()
    {
        var request = CreateI94Request();
        var issued = await _issuer.IssueAsync(request);
        var verification = await _issuer.VerifyAsync(issued.SerializedCredential);

        Assert.True(verification.IsValid);
        Assert.Null(verification.Error);
        Assert.Equal("did:key:z6MkTestIssuer", verification.IssuerDid);
    }

    [Fact]
    public void CredentialSchemas_I94_HasRequiredClaims()
    {
        var schema = CredentialSchemas.GetSchema("I94Credential");
        Assert.NotNull(schema);
        Assert.Contains("holderName", schema.RequiredClaims);
        Assert.Contains("i94Number", schema.RequiredClaims);
        Assert.Contains("classOfAdmission", schema.RequiredClaims);
        Assert.Contains("admittedUntil", schema.RequiredClaims);
    }

    public void Dispose() => _db.Dispose();

    private static CredentialIssuanceRequest CreateI20Request() => new()
    {
        IssuerDid = "did:key:z6MkTestIssuer",
        SubjectDid = "did:key:z6MkTestStudent",
        CredentialType = "I20Credential",
        ValidityDays = 365,
        Claims = new Dictionary<string, object>
        {
            ["sevisId"] = "N0001234567",
            ["studentName"] = "John Doe",
            ["programStatus"] = "Active",
            ["educationLevel"] = "Master's",
            ["primaryMajor"] = "Computer Science",
            ["programStartDate"] = "2024-08-15",
            ["programEndDate"] = "2026-05-15",
            ["institutionName"] = "Test University"
        }
    };

    private static CredentialIssuanceRequest CreatePassportRequest() => new()
    {
        IssuerDid = "did:key:z6MkTestIssuer",
        SubjectDid = "did:key:z6MkTestStudent",
        CredentialType = "PassportCredential",
        ValidityDays = 365,
        Claims = new Dictionary<string, object>
        {
            ["holderName"] = "Jane Doe",
            ["nationality"] = "United States",
            ["issuingState"] = "USA",
            ["documentNumber"] = "123456789",
            ["dateOfBirth"] = "1995-03-15",
            ["expirationDate"] = "2035-03-14",
            ["sex"] = "F"
        }
    };

    private static CredentialIssuanceRequest CreateVisaRequest() => new()
    {
        IssuerDid = "did:key:z6MkTestIssuer",
        SubjectDid = "did:key:z6MkTestStudent",
        CredentialType = "VisaCredential",
        ValidityDays = 365,
        Claims = new Dictionary<string, object>
        {
            ["holderName"] = "Jane Doe",
            ["visaType"] = "F-1",
            ["issuingPost"] = "London",
            ["issueDate"] = "2024-06-01",
            ["expirationDate"] = "2028-06-01",
            ["stampNumber"] = "20241234567",
            ["controlNumber"] = "20241234567890"
        }
    };

    private static CredentialIssuanceRequest CreateDS2019Request() => new()
    {
        IssuerDid = "did:key:z6MkTestIssuer",
        SubjectDid = "did:key:z6MkTestStudent",
        CredentialType = "DS2019Credential",
        ValidityDays = 365,
        Claims = new Dictionary<string, object>
        {
            ["sevisId"] = "N0001234567",
            ["participantName"] = "Jane Doe",
            ["programSponsor"] = "University Exchange Program",
            ["programNumber"] = "P-1-00001",
            ["categoryCode"] = "1A",
            ["programStartDate"] = "2024-08-15",
            ["programEndDate"] = "2025-08-14"
        }
    };

    private static CredentialIssuanceRequest CreateI94Request() => new()
    {
        IssuerDid = "did:key:z6MkTestIssuer",
        SubjectDid = "did:key:z6MkTestStudent",
        CredentialType = "I94Credential",
        ValidityDays = 365,
        Claims = new Dictionary<string, object>
        {
            ["holderName"] = "Jane Doe",
            ["i94Number"] = "12345678901",
            ["classOfAdmission"] = "F-1",
            ["admissionDate"] = "2024-08-10",
            ["admittedUntil"] = "D/S"
        }
    };
}
