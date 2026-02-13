namespace ImmCheck.Core.SSI.Credentials;

/// <summary>
/// Manages signing keys for credential issuance.
/// Each issuer (institution) has their own key pair.
/// Supports both Ed25519 (EdDSA) and HMAC-SHA256 (HS256) keys.
/// </summary>
public interface IKeyStore
{
    /// <summary>Get or create a signing key for the given issuer DID.</summary>
    Task<byte[]> GetOrCreateSigningKeyAsync(string issuerDid);

    /// <summary>Get the public key for signature verification.</summary>
    Task<byte[]?> GetPublicKeyAsync(string issuerDid);

    /// <summary>Synchronous key retrieval for signing operations.</summary>
    byte[] GetSigningKeySync(string issuerDid);

    /// <summary>Get the algorithm used by the issuer's key ("EdDSA" or "HS256").</summary>
    Task<string> GetAlgorithmAsync(string issuerDid);
}
