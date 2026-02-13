using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using ImmCheck.Core.SSI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ImmCheck.Infrastructure.SSI;

/// <summary>
/// Resolves did:prism identifiers via the Hyperledger Identus Cloud Agent REST API.
/// Supports creating, publishing, and resolving did:prism DIDs on Cardano.
///
/// Identus Cloud Agent API:
///   GET  /did-registrar/dids/{did}  — resolve a DID
///   POST /did-registrar/dids        — create a new DID
///   POST /did-registrar/dids/{did}/publications — publish to Cardano
/// </summary>
public class DidPrismResolver : IDidResolver, IDidManager, IDidPublisher
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<DidPrismResolver> _logger;
    private readonly string _agentUrl;
    private readonly string? _apiKey;

    public DidPrismResolver(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<DidPrismResolver> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _agentUrl = configuration["Identus:AgentUrl"]?.TrimEnd('/') ?? "http://localhost:8080/cloud-agent";
        _apiKey = configuration["Identus:ApiKey"];

        if (!string.IsNullOrEmpty(_apiKey))
        {
            _httpClient.DefaultRequestHeaders.Add("apikey", _apiKey);
        }
    }

    public string Method => "prism";

    public bool CanResolve(string did) => did.StartsWith("did:prism:");

    public async Task<DidDocument?> ResolveAsync(string did)
    {
        if (!CanResolve(did))
            return null;

        try
        {
            // Use the Identus Cloud Agent DID resolution endpoint
            var response = await _httpClient.GetAsync($"{_agentUrl}/did-registrar/dids/{Uri.EscapeDataString(did)}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to resolve {Did}: {Status}", did, response.StatusCode);
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            var agentResponse = JsonSerializer.Deserialize<IdentusManagedDid>(json);

            if (agentResponse == null)
                return null;

            // Convert Identus response to W3C DID Document
            return ConvertToDidDocument(agentResponse);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex, "Could not connect to Identus Cloud Agent at {Url}", _agentUrl);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resolving did:prism {Did}", did);
            return null;
        }
    }

    public async Task<DidDocument> CreateDidAsync(string method, DidCreationOptions? options = null)
    {
        if (method != "prism")
            throw new ArgumentException($"This manager only supports 'prism', got '{method}'");

        var agentUrl = options?.AgentApiUrl?.TrimEnd('/') ?? _agentUrl;

        var createRequest = new
        {
            documentTemplate = new
            {
                publicKeys = new[]
                {
                    new
                    {
                        id = "key-1",
                        purpose = "authentication"
                    },
                    new
                    {
                        id = "key-2",
                        purpose = "assertionMethod"
                    }
                }
            }
        };

        var httpClient = _httpClient;
        if (!string.IsNullOrEmpty(options?.ApiKey) && options.ApiKey != _apiKey)
        {
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("apikey", options.ApiKey);
        }

        var response = await httpClient.PostAsJsonAsync($"{agentUrl}/did-registrar/dids", createRequest);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var createdDid = JsonSerializer.Deserialize<IdentusCreateDidResponse>(json)
            ?? throw new InvalidOperationException("Failed to parse DID creation response");

        _logger.LogInformation("Created did:prism DID: {Did}", createdDid.LongFormDid);

        // Return the DID document for the newly created (unpublished) DID
        return new DidDocument
        {
            Context = new List<string> { "https://www.w3.org/ns/did/v1" },
            Id = createdDid.LongFormDid ?? $"did:prism:{createdDid.Did}",
            VerificationMethod = new List<VerificationMethod>
            {
                new()
                {
                    Id = $"did:prism:{createdDid.Did}#key-1",
                    Type = "Ed25519VerificationKey2020",
                    Controller = $"did:prism:{createdDid.Did}"
                },
                new()
                {
                    Id = $"did:prism:{createdDid.Did}#key-2",
                    Type = "Ed25519VerificationKey2020",
                    Controller = $"did:prism:{createdDid.Did}"
                }
            },
            Authentication = new List<string> { $"did:prism:{createdDid.Did}#key-1" },
            AssertionMethod = new List<string> { $"did:prism:{createdDid.Did}#key-2" }
        };
    }

    public async Task<DidPublicationResult> PublishAsync(string did)
    {
        if (!CanResolve(did))
            return new DidPublicationResult
            {
                Success = false,
                Error = "Only did:prism DIDs can be published to Cardano"
            };

        try
        {
            var response = await _httpClient.PostAsync(
                $"{_agentUrl}/did-registrar/dids/{Uri.EscapeDataString(did)}/publications", null);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed to publish {Did}: {Status} {Body}",
                    did, response.StatusCode, errorBody);
                return new DidPublicationResult
                {
                    Success = false,
                    Status = "error",
                    Error = $"Identus agent returned {(int)response.StatusCode}: {errorBody}"
                };
            }

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<IdentusPublicationResponse>(json);

            _logger.LogInformation("Published {Did} to Cardano: {Operation}",
                did, result?.ScheduledOperation?.Id);

            return new DidPublicationResult
            {
                Success = true,
                Status = "publication_pending",
                ScheduledOperation = result?.ScheduledOperation?.Id
            };
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex, "Could not connect to Identus Cloud Agent at {Url}", _agentUrl);
            return new DidPublicationResult
            {
                Success = false,
                Error = $"Cannot reach Identus Cloud Agent at {_agentUrl}"
            };
        }
    }

    public async Task<DidStatusResult> GetStatusAsync(string did)
    {
        if (!CanResolve(did))
            return new DidStatusResult { Did = did, Status = "UNSUPPORTED" };

        try
        {
            var response = await _httpClient.GetAsync(
                $"{_agentUrl}/did-registrar/dids/{Uri.EscapeDataString(did)}");

            if (!response.IsSuccessStatusCode)
            {
                return new DidStatusResult { Did = did, Status = "UNKNOWN" };
            }

            var json = await response.Content.ReadAsStringAsync();
            var managedDid = JsonSerializer.Deserialize<IdentusManagedDid>(json);

            return new DidStatusResult
            {
                Did = did,
                Status = managedDid?.Status?.ToUpperInvariant() ?? "UNKNOWN",
                Blockchain = "cardano",
                Network = "preprod"
            };
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex, "Could not connect to Identus Cloud Agent at {Url}", _agentUrl);
            return new DidStatusResult
            {
                Did = did,
                Status = "AGENT_UNREACHABLE"
            };
        }
    }

    private static DidDocument ConvertToDidDocument(IdentusManagedDid managedDid)
    {
        var did = managedDid.Did ?? "";
        var doc = new DidDocument
        {
            Context = new List<string> { "https://www.w3.org/ns/did/v1" },
            Id = did,
            Authentication = new List<string>(),
            AssertionMethod = new List<string>(),
            VerificationMethod = new List<VerificationMethod>()
        };

        if (managedDid.LongFormDid != null)
            doc.Id = managedDid.LongFormDid;

        return doc;
    }
}

// Identus Cloud Agent response models
internal class IdentusManagedDid
{
    [JsonPropertyName("did")]
    public string? Did { get; set; }

    [JsonPropertyName("longFormDid")]
    public string? LongFormDid { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }
}

internal class IdentusCreateDidResponse
{
    [JsonPropertyName("did")]
    public string? Did { get; set; }

    [JsonPropertyName("longFormDid")]
    public string? LongFormDid { get; set; }
}

internal class IdentusPublicationResponse
{
    [JsonPropertyName("scheduledOperation")]
    public IdentusScheduledOperation? ScheduledOperation { get; set; }
}

internal class IdentusScheduledOperation
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("didRef")]
    public string? DidRef { get; set; }
}
