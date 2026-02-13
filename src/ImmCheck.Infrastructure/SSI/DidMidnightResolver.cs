using System.Text.Json;
using System.Text.Json.Serialization;
using ImmCheck.Core.SSI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ImmCheck.Infrastructure.SSI;

/// <summary>
/// Resolves did:midnight identifiers via Midnight's DID indexer/resolver.
/// Midnight is IOG's privacy-focused Cardano partner chain using ZK-SNARKs.
/// Resolver API follows DIF DID Resolution specification.
/// </summary>
public class DidMidnightResolver : IDidResolver
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<DidMidnightResolver> _logger;
    private readonly string _resolverUrl;

    public DidMidnightResolver(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<DidMidnightResolver> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _resolverUrl = configuration["Midnight:ResolverUrl"]?.TrimEnd('/') ?? "https://indexer.testnet.midnight.network";
    }

    public string Method => "midnight";

    public bool CanResolve(string did) => did.StartsWith("did:midnight:");

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
            var resolutionResult = JsonSerializer.Deserialize<MidnightResolutionResponse>(json);

            if (resolutionResult?.DidDocument == null)
                return null;

            return ConvertToDidDocument(resolutionResult.DidDocument);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex, "Could not connect to Midnight resolver at {Url}", _resolverUrl);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resolving did:midnight {Did}", did);
            return null;
        }
    }

    private static DidDocument ConvertToDidDocument(MidnightDidDocument midnightDoc)
    {
        var doc = new DidDocument
        {
            Context = midnightDoc.Context ?? new List<string> { "https://www.w3.org/ns/did/v1" },
            Id = midnightDoc.Id ?? "",
            Authentication = midnightDoc.Authentication ?? new List<string>(),
            AssertionMethod = midnightDoc.AssertionMethod ?? new List<string>(),
            VerificationMethod = new List<VerificationMethod>()
        };

        if (midnightDoc.VerificationMethod != null)
        {
            foreach (var vm in midnightDoc.VerificationMethod)
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

// Midnight DID Resolution response models (DIF format)
internal class MidnightResolutionResponse
{
    [JsonPropertyName("didDocument")]
    public MidnightDidDocument? DidDocument { get; set; }

    [JsonPropertyName("didResolutionMetadata")]
    public JsonElement? DidResolutionMetadata { get; set; }

    [JsonPropertyName("didDocumentMetadata")]
    public JsonElement? DidDocumentMetadata { get; set; }
}

internal class MidnightDidDocument
{
    [JsonPropertyName("@context")]
    public List<string>? Context { get; set; }

    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("verificationMethod")]
    public List<MidnightVerificationMethod>? VerificationMethod { get; set; }

    [JsonPropertyName("authentication")]
    public List<string>? Authentication { get; set; }

    [JsonPropertyName("assertionMethod")]
    public List<string>? AssertionMethod { get; set; }
}

internal class MidnightVerificationMethod
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
