using System.Text.Json.Serialization;

namespace ImmCheck.Core.SSI;

/// <summary>
/// DID Resolution result per W3C DID Resolution specification.
/// </summary>
public class DidResolutionResult
{
    [JsonPropertyName("didDocument")]
    public DidDocument? DidDocument { get; set; }

    [JsonPropertyName("didResolutionMetadata")]
    public DidResolutionMetadata Metadata { get; set; } = new();
}

public class DidResolutionMetadata
{
    [JsonPropertyName("contentType")]
    public string ContentType { get; set; } = "application/did+ld+json";

    [JsonPropertyName("error")]
    public string? Error { get; set; }

    [JsonPropertyName("retrieved")]
    public DateTime Retrieved { get; set; } = DateTime.UtcNow;

    [JsonPropertyName("duration")]
    public long? DurationMs { get; set; }
}
