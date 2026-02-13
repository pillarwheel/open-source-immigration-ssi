using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ImmCheck.Core.SSI;

namespace ImmCheck.Api.Tests;

public class DidEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public DidEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetMethods_ReturnsSupportedMethods()
    {
        var response = await _client.GetAsync("/api/did/methods");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(json);

        var resolve = doc.RootElement.GetProperty("resolve");
        Assert.True(resolve.GetArrayLength() >= 3);

        var create = doc.RootElement.GetProperty("create");
        Assert.Contains("key", create.EnumerateArray().Select(e => e.GetString()));
        Assert.Contains("prism", create.EnumerateArray().Select(e => e.GetString()));
    }

    [Fact]
    public async Task CreateDidKey_ReturnsCreatedDocument()
    {
        var response = await _client.PostAsJsonAsync("/api/did/create", new { method = "key" });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var doc = await response.Content.ReadFromJsonAsync<DidDocument>();
        Assert.NotNull(doc);
        Assert.StartsWith("did:key:z", doc.Id);
        Assert.NotNull(doc.VerificationMethod);
        Assert.NotEmpty(doc.VerificationMethod);
        Assert.Equal("Ed25519VerificationKey2020", doc.VerificationMethod[0].Type);
        Assert.NotNull(doc.Authentication);
        Assert.NotEmpty(doc.Authentication);
        Assert.NotNull(doc.AssertionMethod);
        Assert.NotEmpty(doc.AssertionMethod);
    }

    [Fact]
    public async Task CreateDid_InvalidMethod_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync("/api/did/create", new { method = "invalid" });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateDid_NoMethod_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync("/api/did/create", new { });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ResolveDidKey_ReturnsDocument()
    {
        // First create a DID
        var createResponse = await _client.PostAsJsonAsync("/api/did/create", new { method = "key" });
        var createdDoc = await createResponse.Content.ReadFromJsonAsync<DidDocument>();
        Assert.NotNull(createdDoc);

        // Now resolve it
        var resolveResponse = await _client.GetAsync($"/api/did/resolve/{createdDoc.Id}");
        resolveResponse.EnsureSuccessStatusCode();

        var result = await resolveResponse.Content.ReadFromJsonAsync<DidResolutionResult>();
        Assert.NotNull(result);
        Assert.NotNull(result.DidDocument);
        Assert.Equal(createdDoc.Id, result.DidDocument.Id);
        Assert.Null(result.Metadata.Error);
        Assert.NotNull(result.Metadata.DurationMs);
    }

    [Fact]
    public async Task ResolveInvalidDid_ReturnsBadRequest()
    {
        var response = await _client.GetAsync("/api/did/resolve/notadid");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ResolveUnknownDid_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/did/resolve/did:unknown:abc123");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ResolvePrismDid_WithoutAgent_ReturnsNotFound()
    {
        // Identus Cloud Agent isn't running in tests, so resolution should fail gracefully
        var response = await _client.GetAsync("/api/did/resolve/did:prism:abc123");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Publish_NonPrismDid_ReturnsBadRequest()
    {
        var response = await _client.PostAsync("/api/did/publish/did:key:z6MkTest", null);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetStatus_NonPrismDid_ReturnsBadRequest()
    {
        var response = await _client.GetAsync("/api/did/status/did:key:z6MkTest");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Publish_PrismDid_WithoutAgent_Returns502()
    {
        // Agent not running — should fail with 502 (Bad Gateway)
        var response = await _client.PostAsync("/api/did/publish/did:prism:testdid123", null);
        Assert.Equal(HttpStatusCode.BadGateway, response.StatusCode);
    }

    [Fact]
    public async Task GetMethods_IncludesCheqd()
    {
        var response = await _client.GetAsync("/api/did/methods");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(json);

        var resolve = doc.RootElement.GetProperty("resolve");
        var methods = resolve.EnumerateArray().Select(e => e.GetString()).ToList();
        Assert.Contains("cheqd", methods);
    }
}
