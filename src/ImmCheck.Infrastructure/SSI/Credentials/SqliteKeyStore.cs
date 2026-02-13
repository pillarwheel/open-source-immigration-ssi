using System.Collections.Concurrent;
using ImmCheck.Core.SSI.Credentials;
using Microsoft.EntityFrameworkCore;
using ImmCheck.Infrastructure.Data;
using NSec.Cryptography;

namespace ImmCheck.Infrastructure.SSI.Credentials;

/// <summary>
/// SQLite-backed key store for credential signing keys.
/// New issuers get Ed25519 key pairs (EdDSA) by default.
/// Existing HS256 (symmetric) keys continue to work for backward compatibility.
/// </summary>
public class SqliteKeyStore : IKeyStore
{
    private readonly AppDbContext _db;
    private readonly ConcurrentDictionary<string, byte[]> _keyCache = new();
    private readonly ConcurrentDictionary<string, string> _algorithmCache = new();

    public SqliteKeyStore(AppDbContext db)
    {
        _db = db;
    }

    public async Task<byte[]> GetOrCreateSigningKeyAsync(string issuerDid)
    {
        if (_keyCache.TryGetValue(issuerDid, out var cached))
            return cached;

        var record = await _db.SigningKeys.FindAsync(issuerDid);
        if (record != null)
        {
            _keyCache[issuerDid] = record.KeyMaterial;
            _algorithmCache[issuerDid] = record.Algorithm;
            return record.KeyMaterial;
        }

        // Generate Ed25519 key pair via NSec
        using var key = Key.Create(SignatureAlgorithm.Ed25519,
            new KeyCreationParameters { ExportPolicy = KeyExportPolicies.AllowPlaintextExport });
        var privateKey = key.Export(KeyBlobFormat.RawPrivateKey);
        var publicKey = key.Export(KeyBlobFormat.RawPublicKey);

        _db.SigningKeys.Add(new SigningKeyRecord
        {
            IssuerDid = issuerDid,
            KeyMaterial = privateKey,
            PublicKeyMaterial = publicKey,
            Algorithm = "EdDSA",
            CreatedAt = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();

        _keyCache[issuerDid] = privateKey;
        _algorithmCache[issuerDid] = "EdDSA";
        return privateKey;
    }

    public async Task<byte[]?> GetPublicKeyAsync(string issuerDid)
    {
        var record = await _db.SigningKeys.FindAsync(issuerDid);
        if (record == null)
            return null;

        _algorithmCache[issuerDid] = record.Algorithm;

        if (record.Algorithm == "EdDSA" && record.PublicKeyMaterial != null)
            return record.PublicKeyMaterial;

        // For HS256, signing key = verification key (symmetric)
        _keyCache[issuerDid] = record.KeyMaterial;
        return record.KeyMaterial;
    }

    public byte[] GetSigningKeySync(string issuerDid)
    {
        if (_keyCache.TryGetValue(issuerDid, out var cached))
            return cached;

        var record = _db.SigningKeys.Find(issuerDid);
        if (record != null)
        {
            _keyCache[issuerDid] = record.KeyMaterial;
            _algorithmCache[issuerDid] = record.Algorithm;
            return record.KeyMaterial;
        }

        // Auto-create Ed25519 key synchronously
        using var key = Key.Create(SignatureAlgorithm.Ed25519,
            new KeyCreationParameters { ExportPolicy = KeyExportPolicies.AllowPlaintextExport });
        var privateKey = key.Export(KeyBlobFormat.RawPrivateKey);
        var publicKey = key.Export(KeyBlobFormat.RawPublicKey);

        _db.SigningKeys.Add(new SigningKeyRecord
        {
            IssuerDid = issuerDid,
            KeyMaterial = privateKey,
            PublicKeyMaterial = publicKey,
            Algorithm = "EdDSA",
            CreatedAt = DateTime.UtcNow
        });
        _db.SaveChanges();

        _keyCache[issuerDid] = privateKey;
        _algorithmCache[issuerDid] = "EdDSA";
        return privateKey;
    }

    public async Task<string> GetAlgorithmAsync(string issuerDid)
    {
        if (_algorithmCache.TryGetValue(issuerDid, out var cached))
            return cached;

        var record = await _db.SigningKeys.FindAsync(issuerDid);
        var algorithm = record?.Algorithm ?? "EdDSA";
        _algorithmCache[issuerDid] = algorithm;
        return algorithm;
    }
}

/// <summary>
/// Database record for signing keys.
/// Supports both Ed25519 (EdDSA) and HMAC-SHA256 (HS256) algorithms.
/// </summary>
public class SigningKeyRecord
{
    public string IssuerDid { get; set; } = string.Empty;
    public byte[] KeyMaterial { get; set; } = Array.Empty<byte>();
    public byte[]? PublicKeyMaterial { get; set; }
    public string Algorithm { get; set; } = "EdDSA";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
