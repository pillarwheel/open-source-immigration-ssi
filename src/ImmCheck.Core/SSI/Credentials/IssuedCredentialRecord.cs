using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImmCheck.Core.SSI.Credentials;

/// <summary>
/// Database record for a credential that has been issued.
/// Tracks metadata for retrieval, revocation, and status list management.
/// </summary>
[Table("IssuedCredentials")]
public class IssuedCredentialRecord
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    public string IssuerDid { get; set; } = string.Empty;
    public string SubjectDid { get; set; } = string.Empty;
    public string CredentialType { get; set; } = string.Empty;

    /// <summary>The full serialized credential (SD-JWT compact form).</summary>
    public string SerializedCredential { get; set; } = string.Empty;

    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiresAt { get; set; }

    /// <summary>Index in the issuer's status list bitstring.</summary>
    public int StatusListIndex { get; set; }

    public bool IsRevoked { get; set; }
    public DateTime? RevokedAt { get; set; }
}

/// <summary>
/// Tracks the bitstring status list per issuer for credential revocation.
/// See: https://www.w3.org/TR/vc-bitstring-status-list/
/// </summary>
[Table("StatusLists")]
public class StatusListRecord
{
    [Key]
    public string IssuerDid { get; set; } = string.Empty;

    /// <summary>Base64url-encoded bitstring. Each bit represents a credential's revocation status.</summary>
    public string EncodedList { get; set; } = string.Empty;

    /// <summary>Total number of entries allocated in the bitstring.</summary>
    public int Size { get; set; } = 16384; // 16K entries default

    /// <summary>Next available index for a new credential.</summary>
    public int NextIndex { get; set; }

    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}
