using System.Text.Json.Serialization;

namespace ImmCheck.Core.SSI.Credentials;

/// <summary>
/// W3C Verifiable Credential Data Model 2.0.
/// See: https://www.w3.org/TR/vc-data-model-2.0/
/// </summary>
public class VerifiableCredential
{
    [JsonPropertyName("@context")]
    public List<string> Context { get; set; } = new()
    {
        "https://www.w3.org/ns/credentials/v2"
    };

    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("type")]
    public List<string> Type { get; set; } = new() { "VerifiableCredential" };

    [JsonPropertyName("issuer")]
    public string Issuer { get; set; } = string.Empty;

    [JsonPropertyName("validFrom")]
    public DateTime ValidFrom { get; set; } = DateTime.UtcNow;

    [JsonPropertyName("validUntil")]
    public DateTime? ValidUntil { get; set; }

    [JsonPropertyName("credentialSubject")]
    public Dictionary<string, object> CredentialSubject { get; set; } = new();

    [JsonPropertyName("credentialStatus")]
    public CredentialStatus? CredentialStatus { get; set; }

    [JsonPropertyName("credentialSchema")]
    public CredentialSchemaRef? CredentialSchema { get; set; }
}

public class CredentialStatus
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = "BitstringStatusListEntry";

    [JsonPropertyName("statusPurpose")]
    public string StatusPurpose { get; set; } = "revocation";

    [JsonPropertyName("statusListIndex")]
    public int StatusListIndex { get; set; }

    [JsonPropertyName("statusListCredential")]
    public string StatusListCredential { get; set; } = string.Empty;
}

public class CredentialSchemaRef
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = "JsonSchema";
}
