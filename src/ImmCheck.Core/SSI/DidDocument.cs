using System.Text.Json.Serialization;

namespace ImmCheck.Core.SSI;

/// <summary>
/// W3C DID Core v1.0 compliant DID Document.
/// See: https://www.w3.org/TR/did-core/
/// </summary>
public class DidDocument
{
    [JsonPropertyName("@context")]
    public List<string> Context { get; set; } = new() { "https://www.w3.org/ns/did/v1" };

    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("controller")]
    public List<string>? Controller { get; set; }

    [JsonPropertyName("verificationMethod")]
    public List<VerificationMethod>? VerificationMethod { get; set; }

    [JsonPropertyName("authentication")]
    public List<string>? Authentication { get; set; }

    [JsonPropertyName("assertionMethod")]
    public List<string>? AssertionMethod { get; set; }

    [JsonPropertyName("keyAgreement")]
    public List<string>? KeyAgreement { get; set; }

    [JsonPropertyName("service")]
    public List<DidService>? Service { get; set; }
}

public class VerificationMethod
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("controller")]
    public string Controller { get; set; } = string.Empty;

    [JsonPropertyName("publicKeyMultibase")]
    public string? PublicKeyMultibase { get; set; }

    [JsonPropertyName("publicKeyJwk")]
    public Dictionary<string, string>? PublicKeyJwk { get; set; }
}

public class DidService
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("serviceEndpoint")]
    public string ServiceEndpoint { get; set; } = string.Empty;
}
