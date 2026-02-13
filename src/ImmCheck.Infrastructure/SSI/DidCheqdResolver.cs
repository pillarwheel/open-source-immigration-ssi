using System.Text.Json;
using System.Text.Json.Serialization;
using ImmCheck.Core.SSI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ImmCheck.Infrastructure.SSI;

/// <summary>
/// Resolves did:cheqd identifiers via cheqd's public DID resolver.
/// cheqd is a Cosmos SDK-based blockchain for decentralized identity.
/// Resolver API follows DIF DID Resolution specification.
/// </summary>
public class DidCheqdResolver : IDidResolver
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<DidCheqdResolver> _logger;
    private readonly string _resolverUrl;

    public DidCheqdResolver(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<DidCheqdResolver> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _resolverUrl = configuration["Cheqd:ResolverUrl"]?.TrimEnd('/') ?? "https://resolver.cheqd.net";
    }

    public string Method => "cheqd";

    public bool CanResolve(string did) => did.StartsWith("did:cheqd:");

    public async Task<DidDocument?> ResolveAsync(string did)
    {
        if (!CanResolve(did))
            return null;

        try
        {
            var response = await _httpClient.GetAsync(
                $"{_resolverUrl}/1.0/identifiers/{Uri.EscapeDataString(did)}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to resolve {Did}: {Status}", did, response.StatusCode);
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            var resolutionResult = JsonSerializer.Deserialize<CheqdResolutionResponse>(json);

            if (resolutionResult?.DidDocument == null)
                return null;

            return ConvertToDidDocument(resolutionResult.DidDocument);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex, "Could not connect to cheqd resolver at {Url}", _resolverUrl);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resolving did:cheqd {Did}", did);
            return null;
        }
    }

    private static DidDocument ConvertToDidDocument(CheqdDidDocument cheqdDoc)
    {
        var doc = new DidDocument
        {
            Context = cheqdDoc.Context ?? new List<string> { "https://www.w3.org/ns/did/v1" },
            Id = cheqdDoc.Id ?? "",
            Authentication = cheqdDoc.Authentication ?? new List<string>(),
            AssertionMethod = cheqdDoc.AssertionMethod ?? new List<string>(),
            VerificationMethod = new List<VerificationMethod>()
        };

        if (cheqdDoc.VerificationMethod != null)
        {
            foreach (var vm in cheqdDoc.VerificationMethod)
            {
                doc.VerificationMethod.Add(new VerificationMethod
                {
                    Id = vm.Id ?? "",
                    Type = vm.Type ?? "Ed25519VerificationKey2020",
                    Controller = vm.Controller ?? doc.Id,
                    PublicKeyMultibase = vm.PublicKeyMultibase
                });
            }
        }

        return doc;
    }
}

// cheqd DID Resolution response models (DIF format)
internal class CheqdResolutionResponse
{
    [JsonPropertyName("didDocument")]
    public CheqdDidDocument? DidDocument { get; set; }

    [JsonPropertyName("didResolutionMetadata")]
    public JsonElement? DidResolutionMetadata { get; set; }

    [JsonPropertyName("didDocumentMetadata")]
    public JsonElement? DidDocumentMetadata { get; set; }
}

internal class CheqdDidDocument
{
    [JsonPropertyName("@context")]
    public List<string>? Context { get; set; }

    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("verificationMethod")]
    public List<CheqdVerificationMethod>? VerificationMethod { get; set; }

    [JsonPropertyName("authentication")]
    public List<string>? Authentication { get; set; }

    [JsonPropertyName("assertionMethod")]
    public List<string>? AssertionMethod { get; set; }
}

internal class CheqdVerificationMethod
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("controller")]
    public string? Controller { get; set; }

    [JsonPropertyName("publicKeyMultibase")]
    public string? PublicKeyMultibase { get; set; }
}
