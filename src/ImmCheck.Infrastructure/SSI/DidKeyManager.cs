using NSec.Cryptography;
using ImmCheck.Core.SSI;

namespace ImmCheck.Infrastructure.SSI;

/// <summary>
/// Creates did:key DIDs by generating Ed25519 key pairs locally.
/// Uses NSec.Cryptography (libsodium) for real Ed25519 key generation.
/// </summary>
public class DidKeyManager : IDidManager
{
    public Task<DidDocument> CreateDidAsync(string method, DidCreationOptions? options = null)
    {
        if (method != "key")
            throw new ArgumentException($"This manager only supports 'key', got '{method}'");

        // Generate real Ed25519 key pair via NSec (libsodium)
        using var key = Key.Create(SignatureAlgorithm.Ed25519,
            new KeyCreationParameters { ExportPolicy = KeyExportPolicies.AllowPlaintextExport });
        var publicKey = key.Export(KeyBlobFormat.RawPublicKey);

        // Encode as did:key with Ed25519 multicodec prefix (0xed, 0x01)
        var multicodecKey = new byte[2 + publicKey.Length];
        multicodecKey[0] = 0xed;
        multicodecKey[1] = 0x01;
        Array.Copy(publicKey, 0, multicodecKey, 2, publicKey.Length);

        var multibaseEncoded = "z" + Base58.Encode(multicodecKey);
        var did = $"did:key:{multibaseEncoded}";
        var verificationMethodId = $"{did}#{multibaseEncoded}";

        var doc = new DidDocument
        {
            Context = new List<string>
            {
                "https://www.w3.org/ns/did/v1",
                "https://w3id.org/security/suites/ed2519-2020/v1"
            },
            Id = did,
            VerificationMethod = new List<VerificationMethod>
            {
                new()
                {
                    Id = verificationMethodId,
                    Type = "Ed25519VerificationKey2020",
                    Controller = did,
                    PublicKeyMultibase = $"z{Base58.Encode(publicKey)}"
                }
            },
            Authentication = new List<string> { verificationMethodId },
            AssertionMethod = new List<string> { verificationMethodId }
        };

        return Task.FromResult(doc);
    }
}
