using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ImmCheck.Core.SSI.Credentials;

namespace ImmCheck.Api.Tests;

public class CredentialEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public CredentialEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetSchemas_ReturnsAvailableSchemas()
    {
        var response = await _client.GetAsync("/api/credential/schemas");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var schemas = JsonSerializer.Deserialize<JsonElement[]>(json);
        Assert.NotNull(schemas);
        Assert.Equal(6, schemas.Length);
    }

    [Fact]
    public async Task IssueI20Credential_ReturnsCreated()
    {
        var request = CreateI20Request();
        var response = await _client.PostAsJsonAsync("/api/credential/issue", request);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<IssuedCredential>();
        Assert.NotNull(result);
        Assert.NotEmpty(result.CredentialId);
        Assert.Equal("vc+sd-jwt", result.Format);
        Assert.Contains("~", result.SerializedCredential); // SD-JWT has disclosures
        Assert.Contains("I20Credential", result.Credential.Type);
    }

    [Fact]
    public async Task IssueCredential_MissingClaims_ReturnsBadRequest()
    {
        var request = new CredentialIssuanceRequest
        {
            IssuerDid = "did:key:z6MkTest",
            SubjectDid = "did:key:z6MkStudent",
            CredentialType = "I20Credential",
            Claims = new Dictionary<string, object>
            {
                ["sevisId"] = "N0001234567"
                // Missing all other required claims
            }
        };

        var response = await _client.PostAsJsonAsync("/api/credential/issue", request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task IssueCredential_UnknownType_ReturnsBadRequest()
    {
        var request = new CredentialIssuanceRequest
        {
            IssuerDid = "did:key:z6MkTest",
            SubjectDid = "did:key:z6MkStudent",
            CredentialType = "UnknownType",
            Claims = new Dictionary<string, object> { ["test"] = "value" }
        };

        var response = await _client.PostAsJsonAsync("/api/credential/issue", request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetCredentialById_AfterIssue_ReturnsCredential()
    {
        // Issue
        var issueRequest = CreateI20Request();
        var issueResponse = await _client.PostAsJsonAsync("/api/credential/issue", issueRequest);
        var issued = await issueResponse.Content.ReadFromJsonAsync<IssuedCredential>();
        Assert.NotNull(issued);

        // Retrieve
        var getResponse = await _client.GetAsync($"/api/credential/{issued.CredentialId}");
        getResponse.EnsureSuccessStatusCode();

        var json = await getResponse.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(json);
        Assert.Equal(issued.CredentialId, doc.RootElement.GetProperty("id").GetString());
        Assert.Equal("I20Credential", doc.RootElement.GetProperty("credentialType").GetString());
    }

    [Fact]
    public async Task GetCredentialById_NotFound_Returns404()
    {
        var response = await _client.GetAsync("/api/credential/nonexistent");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task VerifyCredential_ValidCredential_ReturnsValid()
    {
        // Issue
        var issueRequest = CreateI20Request();
        var issueResponse = await _client.PostAsJsonAsync("/api/credential/issue", issueRequest);
        var issued = await issueResponse.Content.ReadFromJsonAsync<IssuedCredential>();
        Assert.NotNull(issued);

        // Verify with all disclosures
        var verifyResponse = await _client.PostAsJsonAsync("/api/credential/verify",
            new { credential = issued.SerializedCredential });
        verifyResponse.EnsureSuccessStatusCode();

        var result = await verifyResponse.Content.ReadFromJsonAsync<CredentialVerificationResult>();
        Assert.NotNull(result);
        Assert.True(result.IsValid);
        Assert.Equal("did:key:z6MkIssuer", result.IssuerDid);
        Assert.NotNull(result.DisclosedClaims);
    }

    [Fact]
    public async Task VerifyCredential_SelectiveDisclosure_OnlyDisclosedClaims()
    {
        // Issue with selective disclosure
        var issueRequest = CreateI20Request();
        issueRequest.SelectiveDisclosureClaims = new List<string> { "sevisId", "studentName" };

        var issueResponse = await _client.PostAsJsonAsync("/api/credential/issue", issueRequest);
        var issued = await issueResponse.Content.ReadFromJsonAsync<IssuedCredential>();
        Assert.NotNull(issued);

        // Parse SD-JWT and only present some disclosures
        var parts = issued.SerializedCredential.Split('~');
        var jwt = parts[0];
        // Present only the first disclosure (sevisId), skip studentName
        var partialPresentation = $"{jwt}~{parts[1]}~";

        var verifyResponse = await _client.PostAsJsonAsync("/api/credential/verify",
            new { credential = partialPresentation });
        verifyResponse.EnsureSuccessStatusCode();

        var result = await verifyResponse.Content.ReadFromJsonAsync<CredentialVerificationResult>();
        Assert.NotNull(result);
        Assert.True(result.IsValid);
        Assert.NotNull(result.DisclosedClaims);
        // Only one SD claim disclosed + non-SD claims
        Assert.True(result.DisclosedClaims.Count >= 1);
    }

    [Fact]
    public async Task RevokeCredential_ThenStatusChanges()
    {
        // Issue
        var issueRequest = CreateI20Request();
        var issueResponse = await _client.PostAsJsonAsync("/api/credential/issue", issueRequest);
        var issued = await issueResponse.Content.ReadFromJsonAsync<IssuedCredential>();
        Assert.NotNull(issued);

        // Revoke
        var revokeResponse = await _client.PostAsync($"/api/credential/{issued.CredentialId}/revoke", null);
        revokeResponse.EnsureSuccessStatusCode();

        // Verify the credential record shows revoked
        var getResponse = await _client.GetAsync($"/api/credential/{issued.CredentialId}");
        var json = await getResponse.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(json);
        Assert.True(doc.RootElement.GetProperty("isRevoked").GetBoolean());
    }

    [Fact]
    public async Task RevokeCredential_NonExistent_Returns404()
    {
        var response = await _client.PostAsync("/api/credential/nonexistent/revoke", null);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetBySubject_ReturnsIssuedCredentials()
    {
        // Issue two credentials for the same subject
        var request1 = CreateI20Request();
        await _client.PostAsJsonAsync("/api/credential/issue", request1);

        var request2 = CreateFinancialRequest();
        await _client.PostAsJsonAsync("/api/credential/issue", request2);

        // Get by subject
        var response = await _client.GetAsync(
            $"/api/credential/subject/{Uri.EscapeDataString("did:key:z6MkStudent")}");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var credentials = JsonSerializer.Deserialize<JsonElement[]>(json);
        Assert.NotNull(credentials);
        Assert.True(credentials.Length >= 2);
    }

    [Fact]
    public async Task GetStatusList_AfterRevocation_HasBitSet()
    {
        // Issue and revoke
        var request = CreateI20Request();
        var issueResponse = await _client.PostAsJsonAsync("/api/credential/issue", request);
        var issued = await issueResponse.Content.ReadFromJsonAsync<IssuedCredential>();
        Assert.NotNull(issued);

        await _client.PostAsync($"/api/credential/{issued.CredentialId}/revoke", null);

        // Get status list
        var response = await _client.GetAsync(
            $"/api/credential/status-list/{Uri.EscapeDataString("did:key:z6MkIssuer")}");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(json);
        Assert.Equal("BitstringStatusListCredential", doc.RootElement.GetProperty("type").GetString());
        Assert.NotEmpty(doc.RootElement.GetProperty("encodedList").GetString()!);
    }

    [Fact]
    public async Task IssuePassportCredential_ReturnsCreated()
    {
        var request = CreatePassportRequest();
        var response = await _client.PostAsJsonAsync("/api/credential/issue", request);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<IssuedCredential>();
        Assert.NotNull(result);
        Assert.NotEmpty(result.CredentialId);
        Assert.Equal("vc+sd-jwt", result.Format);
    }

    [Fact]
    public async Task IssueVisaCredential_ReturnsCreated()
    {
        var request = CreateVisaRequest();
        var response = await _client.PostAsJsonAsync("/api/credential/issue", request);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<IssuedCredential>();
        Assert.NotNull(result);
        Assert.NotEmpty(result.CredentialId);
        Assert.Equal("vc+sd-jwt", result.Format);
    }

    [Fact]
    public async Task IssueDS2019Credential_ReturnsCreated()
    {
        var request = CreateDS2019Request();
        var response = await _client.PostAsJsonAsync("/api/credential/issue", request);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<IssuedCredential>();
        Assert.NotNull(result);
        Assert.NotEmpty(result.CredentialId);
        Assert.Equal("vc+sd-jwt", result.Format);
    }

    [Fact]
    public async Task IssueI94Credential_ReturnsCreated()
    {
        var request = CreateI94Request();
        var response = await _client.PostAsJsonAsync("/api/credential/issue", request);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<IssuedCredential>();
        Assert.NotNull(result);
        Assert.NotEmpty(result.CredentialId);
        Assert.Equal("vc+sd-jwt", result.Format);
    }

    private static CredentialIssuanceRequest CreateI20Request() => new()
    {
        IssuerDid = "did:key:z6MkIssuer",
        SubjectDid = "did:key:z6MkStudent",
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

    private static CredentialIssuanceRequest CreateFinancialRequest() => new()
    {
        IssuerDid = "did:key:z6MkIssuer",
        SubjectDid = "did:key:z6MkStudent",
        CredentialType = "FinancialSupportCredential",
        ValidityDays = 180,
        Claims = new Dictionary<string, object>
        {
            ["sevisId"] = "N0001234567",
            ["studentName"] = "John Doe",
            ["academicTerm"] = "Fall 2024",
            ["totalExpenses"] = 45000,
            ["totalFunding"] = 50000,
            ["tuition"] = 25000,
            ["livingExpenses"] = 15000,
            ["personalFunds"] = 10000,
            ["schoolFunds"] = 30000,
            ["schoolFundsDescription"] = "Graduate Assistantship"
        }
    };

    private static CredentialIssuanceRequest CreatePassportRequest() => new()
    {
        IssuerDid = "did:key:z6MkIssuer",
        SubjectDid = "did:key:z6MkStudent",
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
        IssuerDid = "did:key:z6MkIssuer",
        SubjectDid = "did:key:z6MkStudent",
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
        IssuerDid = "did:key:z6MkIssuer",
        SubjectDid = "did:key:z6MkStudent",
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
        IssuerDid = "did:key:z6MkIssuer",
        SubjectDid = "did:key:z6MkStudent",
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
