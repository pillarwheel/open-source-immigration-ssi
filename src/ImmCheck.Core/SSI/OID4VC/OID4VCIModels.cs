using System.Text.Json.Serialization;

namespace ImmCheck.Core.SSI.OID4VC;

/// <summary>
/// OID4VCI Credential Issuer Metadata.
/// See: https://openid.net/specs/openid-4-verifiable-credential-issuance-1_0.html
/// </summary>
public class CredentialIssuerMetadata
{
    [JsonPropertyName("credential_issuer")]
    public string CredentialIssuer { get; set; } = string.Empty;

    [JsonPropertyName("credential_endpoint")]
    public string CredentialEndpoint { get; set; } = string.Empty;

    [JsonPropertyName("token_endpoint")]
    public string TokenEndpoint { get; set; } = string.Empty;

    [JsonPropertyName("credential_configurations_supported")]
    public Dictionary<string, CredentialConfiguration> CredentialConfigurationsSupported { get; set; } = new();
}

public class CredentialConfiguration
{
    [JsonPropertyName("format")]
    public string Format { get; set; } = "vc+sd-jwt";

    [JsonPropertyName("scope")]
    public string? Scope { get; set; }

    [JsonPropertyName("credential_definition")]
    public CredentialDefinition CredentialDefinition { get; set; } = new();

    [JsonPropertyName("display")]
    public List<CredentialDisplay>? Display { get; set; }
}

public class CredentialDefinition
{
    [JsonPropertyName("type")]
    public List<string> Type { get; set; } = new() { "VerifiableCredential" };

    [JsonPropertyName("credentialSubject")]
    public Dictionary<string, ClaimDefinition>? CredentialSubject { get; set; }
}

public class ClaimDefinition
{
    [JsonPropertyName("mandatory")]
    public bool? Mandatory { get; set; }

    [JsonPropertyName("display")]
    public List<ClaimDisplay>? Display { get; set; }
}

public class ClaimDisplay
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("locale")]
    public string Locale { get; set; } = "en-US";
}

public class CredentialDisplay
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("locale")]
    public string Locale { get; set; } = "en-US";

    [JsonPropertyName("description")]
    public string? Description { get; set; }
}

/// <summary>
/// Credential Offer — sent to the holder (e.g., via QR code or deep link).
/// </summary>
public class CredentialOffer
{
    [JsonPropertyName("credential_issuer")]
    public string CredentialIssuer { get; set; } = string.Empty;

    [JsonPropertyName("credential_configuration_ids")]
    public List<string> CredentialConfigurationIds { get; set; } = new();

    [JsonPropertyName("grants")]
    public OfferGrants Grants { get; set; } = new();
}

public class OfferGrants
{
    [JsonPropertyName("urn:ietf:params:oauth:grant-type:pre-authorized_code")]
    public PreAuthorizedCodeGrant? PreAuthorizedCode { get; set; }
}

public class PreAuthorizedCodeGrant
{
    [JsonPropertyName("pre-authorized_code")]
    public string PreAuthorizedCodeValue { get; set; } = string.Empty;
}

/// <summary>
/// Token response for pre-authorized code exchange.
/// </summary>
public class TokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;

    [JsonPropertyName("token_type")]
    public string TokenType { get; set; } = "Bearer";

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; } = 3600;

    [JsonPropertyName("c_nonce")]
    public string? CNonce { get; set; }

    [JsonPropertyName("c_nonce_expires_in")]
    public int? CNonceExpiresIn { get; set; }
}

/// <summary>
/// Credential request from the holder's wallet.
/// </summary>
public class CredentialRequest
{
    [JsonPropertyName("format")]
    public string Format { get; set; } = "vc+sd-jwt";

    [JsonPropertyName("credential_definition")]
    public CredentialDefinition? CredentialDefinition { get; set; }
}

/// <summary>
/// Credential response containing the issued credential.
/// </summary>
public class CredentialResponse
{
    [JsonPropertyName("format")]
    public string Format { get; set; } = "vc+sd-jwt";

    [JsonPropertyName("credential")]
    public string Credential { get; set; } = string.Empty;

    [JsonPropertyName("c_nonce")]
    public string? CNonce { get; set; }

    [JsonPropertyName("c_nonce_expires_in")]
    public int? CNonceExpiresIn { get; set; }
}
