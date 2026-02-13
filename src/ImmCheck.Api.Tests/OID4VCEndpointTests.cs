using System.Net;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Text.Json;
using ImmCheck.Core.SSI.OID4VC;

namespace ImmCheck.Api.Tests;

public class OID4VCEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public OID4VCEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    // --- OID4VCI Tests ---

    [Fact]
    public async Task GetIssuerMetadata_ReturnsCredentialConfigurations()
    {
        var response = await _client.GetAsync("/api/oid4vci/metadata");
        response.EnsureSuccessStatusCode();

        var metadata = await response.Content.ReadFromJsonAsync<CredentialIssuerMetadata>();
        Assert.NotNull(metadata);
        Assert.Contains("I20Credential", metadata.CredentialConfigurationsSupported.Keys);
        Assert.Contains("FinancialSupportCredential", metadata.CredentialConfigurationsSupported.Keys);
        Assert.NotEmpty(metadata.CredentialEndpoint);
        Assert.NotEmpty(metadata.TokenEndpoint);
    }

    [Fact]
    public async Task FullOID4VCIFlow_PreAuthorizedCode_Succeeds()
    {
        // Step 1: Create credential offer
        var offerResponse = await _client.PostAsJsonAsync("/api/oid4vci/credential-offer", new
        {
            issuerDid = "did:key:z6MkTestIssuer",
            subjectDid = "did:key:z6MkTestStudent",
            credentialConfigurationId = "I20Credential",
            claims = new Dictionary<string, object>
            {
                ["sevisId"] = "N0001234567",
                ["studentName"] = "Jane Smith",
                ["programStatus"] = "Active",
                ["educationLevel"] = "PhD",
                ["primaryMajor"] = "Physics",
                ["programStartDate"] = "2024-08-15",
                ["programEndDate"] = "2029-05-15",
                ["institutionName"] = "Test University"
            },
            validityDays = 365
        });
        offerResponse.EnsureSuccessStatusCode();

        var offer = await offerResponse.Content.ReadFromJsonAsync<CredentialOffer>();
        Assert.NotNull(offer);
        Assert.NotEmpty(offer.CredentialConfigurationIds);
        Assert.NotNull(offer.Grants.PreAuthorizedCode);

        var preAuthCode = offer.Grants.PreAuthorizedCode.PreAuthorizedCodeValue;
        Assert.NotEmpty(preAuthCode);

        // Step 2: Exchange pre-authorized code for access token
        var tokenContent = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "urn:ietf:params:oauth:grant-type:pre-authorized_code"),
            new KeyValuePair<string, string>("pre-authorized_code", preAuthCode)
        });
        var tokenResponse = await _client.PostAsync("/api/oid4vci/token", tokenContent);
        tokenResponse.EnsureSuccessStatusCode();

        var token = await tokenResponse.Content.ReadFromJsonAsync<TokenResponse>();
        Assert.NotNull(token);
        Assert.NotEmpty(token.AccessToken);
        Assert.Equal("Bearer", token.TokenType);

        // Step 3: Request credential using access token
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

        var credResponse = await _client.PostAsJsonAsync("/api/oid4vci/credential", new
        {
            format = "vc+sd-jwt",
            credential_definition = new { type = new[] { "VerifiableCredential", "I20Credential" } }
        });
        credResponse.EnsureSuccessStatusCode();

        var credResult = await credResponse.Content.ReadFromJsonAsync<CredentialResponse>();
        Assert.NotNull(credResult);
        Assert.Equal("vc+sd-jwt", credResult.Format);
        Assert.NotEmpty(credResult.Credential);
        Assert.Contains("~", credResult.Credential); // SD-JWT has disclosures

        // Clean up auth header
        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task TokenEndpoint_InvalidGrantType_ReturnsBadRequest()
    {
        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "authorization_code"),
            new KeyValuePair<string, string>("code", "test")
        });
        var response = await _client.PostAsync("/api/oid4vci/token", content);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CredentialEndpoint_NoAuth_ReturnsUnauthorized()
    {
        var response = await _client.PostAsJsonAsync("/api/oid4vci/credential", new
        {
            format = "vc+sd-jwt"
        });
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    // --- OID4VP Tests ---

    [Fact]
    public async Task GetScenarios_ReturnsPredefinedScenarios()
    {
        var response = await _client.GetAsync("/api/oid4vp/scenarios");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var scenarios = JsonSerializer.Deserialize<JsonElement[]>(json);
        Assert.NotNull(scenarios);
        Assert.Equal(5, scenarios.Length);
    }

    [Fact]
    public async Task CreatePresentationRequest_F1Status_ReturnsDefinition()
    {
        var response = await _client.PostAsJsonAsync("/api/oid4vp/request", new { scenario = "f1-status" });
        response.EnsureSuccessStatusCode();

        var request = await response.Content.ReadFromJsonAsync<PresentationRequest>();
        Assert.NotNull(request);
        Assert.Equal("vp_token", request.ResponseType);
        Assert.NotEmpty(request.Nonce);
        Assert.NotNull(request.PresentationDefinition);
        Assert.Equal("f1-status-verification", request.PresentationDefinition.Id);
    }

    [Fact]
    public async Task FullOID4VPFlow_SubmitPresentation_Verifies()
    {
        // First issue a credential to present
        var issueResponse = await _client.PostAsJsonAsync("/api/credential/issue", new
        {
            issuerDid = "did:key:z6MkVPIssuer",
            subjectDid = "did:key:z6MkVPStudent",
            credentialType = "I20Credential",
            claims = new Dictionary<string, object>
            {
                ["sevisId"] = "N0009999999",
                ["studentName"] = "VP Test Student",
                ["programStatus"] = "Active",
                ["educationLevel"] = "Bachelor's",
                ["primaryMajor"] = "Engineering",
                ["programStartDate"] = "2024-08-15",
                ["programEndDate"] = "2028-05-15",
                ["institutionName"] = "VP Test University"
            },
            validityDays = 365
        });
        var issued = await issueResponse.Content.ReadFromJsonAsync<JsonElement>();
        var vpToken = issued.GetProperty("serializedCredential").GetString();
        Assert.NotNull(vpToken);

        // Create presentation request
        var reqResponse = await _client.PostAsJsonAsync("/api/oid4vp/request", new { scenario = "f1-status" });
        var presReq = await reqResponse.Content.ReadFromJsonAsync<PresentationRequest>();
        Assert.NotNull(presReq);

        // Submit presentation
        var submitResponse = await _client.PostAsJsonAsync("/api/oid4vp/response", new
        {
            vp_token = vpToken,
            state = presReq.State,
            presentation_submission = new
            {
                id = "submission-1",
                definition_id = presReq.PresentationDefinition.Id,
                descriptor_map = new[]
                {
                    new { id = "i20-credential", format = "vc+sd-jwt", path = "$" }
                }
            }
        });
        submitResponse.EnsureSuccessStatusCode();

        var result = await submitResponse.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(result.GetProperty("isValid").GetBoolean());

        // Check status
        var statusResponse = await _client.GetAsync($"/api/oid4vp/status/{presReq.State}");
        statusResponse.EnsureSuccessStatusCode();
        var status = await statusResponse.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("completed", status.GetProperty("status").GetString());
    }

    [Fact]
    public async Task CreatePresentationRequest_PassportIdentity_ReturnsDefinition()
    {
        var response = await _client.PostAsJsonAsync("/api/oid4vp/request", new { scenario = "passport-identity" });
        response.EnsureSuccessStatusCode();

        var request = await response.Content.ReadFromJsonAsync<PresentationRequest>();
        Assert.NotNull(request);
        Assert.Equal("vp_token", request.ResponseType);
        Assert.NotEmpty(request.Nonce);
        Assert.NotNull(request.PresentationDefinition);
        Assert.Equal("passport-identity-verification", request.PresentationDefinition.Id);
    }

    [Fact]
    public async Task CreatePresentationRequest_J1Status_ReturnsDefinition()
    {
        var response = await _client.PostAsJsonAsync("/api/oid4vp/request", new { scenario = "j1-status" });
        response.EnsureSuccessStatusCode();

        var request = await response.Content.ReadFromJsonAsync<PresentationRequest>();
        Assert.NotNull(request);
        Assert.Equal("vp_token", request.ResponseType);
        Assert.NotEmpty(request.Nonce);
        Assert.NotNull(request.PresentationDefinition);
        Assert.Equal("j1-status-verification", request.PresentationDefinition.Id);
    }

    [Fact]
    public async Task CreatePresentationRequest_AdmissionStatus_ReturnsDefinition()
    {
        var response = await _client.PostAsJsonAsync("/api/oid4vp/request", new { scenario = "admission-status" });
        response.EnsureSuccessStatusCode();

        var request = await response.Content.ReadFromJsonAsync<PresentationRequest>();
        Assert.NotNull(request);
        Assert.Equal("vp_token", request.ResponseType);
        Assert.NotEmpty(request.Nonce);
        Assert.NotNull(request.PresentationDefinition);
        Assert.Equal("admission-status-verification", request.PresentationDefinition.Id);
    }
}
