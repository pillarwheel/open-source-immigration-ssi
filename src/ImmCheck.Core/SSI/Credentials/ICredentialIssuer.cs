namespace ImmCheck.Core.SSI.Credentials;

/// <summary>
/// Issues Verifiable Credentials in a specific format (e.g., SD-JWT, JSON-LD).
/// </summary>
public interface ICredentialIssuer
{
    /// <summary>
    /// Issues a verifiable credential and returns the serialized form.
    /// </summary>
    Task<IssuedCredential> IssueAsync(CredentialIssuanceRequest request);

    /// <summary>
    /// Verifies a presented credential and returns the disclosed claims.
    /// </summary>
    Task<CredentialVerificationResult> VerifyAsync(string serializedCredential);
}

public class CredentialIssuanceRequest
{
    /// <summary>The issuer's DID.</summary>
    public string IssuerDid { get; set; } = string.Empty;

    /// <summary>The subject's DID (student).</summary>
    public string SubjectDid { get; set; } = string.Empty;

    /// <summary>Credential type (e.g., "I20Credential", "FinancialSupportCredential").</summary>
    public string CredentialType { get; set; } = string.Empty;

    /// <summary>All claims to include in the credential.</summary>
    public Dictionary<string, object> Claims { get; set; } = new();

    /// <summary>
    /// Claims that should be selectively disclosable (SD-JWT).
    /// Claims not in this list are always disclosed.
    /// </summary>
    public List<string>? SelectiveDisclosureClaims { get; set; }

    /// <summary>Validity period in days. Null = no expiry.</summary>
    public int? ValidityDays { get; set; }
}

public class IssuedCredential
{
    /// <summary>Unique credential identifier.</summary>
    public string CredentialId { get; set; } = string.Empty;

    /// <summary>The serialized credential (e.g., SD-JWT compact form).</summary>
    public string SerializedCredential { get; set; } = string.Empty;

    /// <summary>Format of the credential (e.g., "vc+sd-jwt").</summary>
    public string Format { get; set; } = "vc+sd-jwt";

    /// <summary>The W3C VC data model representation.</summary>
    public VerifiableCredential Credential { get; set; } = new();
}

public class CredentialVerificationResult
{
    public bool IsValid { get; set; }
    public string? Error { get; set; }
    public string? IssuerDid { get; set; }
    public string? SubjectDid { get; set; }
    public Dictionary<string, object>? DisclosedClaims { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidUntil { get; set; }
    public bool? IsRevoked { get; set; }
}
