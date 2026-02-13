using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using ImmCheck.Core.SSI.Credentials;
using ImmCheck.Infrastructure.SSI;
using ImmCheck.Infrastructure.SSI.Credentials;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using ImmCheck.Infrastructure.Data;
using NSec.Cryptography;

namespace ImmCheck.Api.Tests.SSI;

public class Ed25519SigningTests : IDisposable
{
    private readonly AppDbContext _db;
    private readonly SdJwtIssuer _issuer;
    private readonly CredentialRepository _credRepo;
    private readonly SqliteKeyStore _keyStore;

    public Ed25519SigningTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("Ed25519Tests_" + Guid.NewGuid().ToString("N"))
            .Options;
        _db = new AppDbContext(options);
        _db.Database.EnsureCreated();

        _keyStore = new SqliteKeyStore(_db);
        _credRepo = new CredentialRepository(_db);
        _issuer = new SdJwtIssuer(_keyStore, _credRepo, NullLogger<SdJwtIssuer>.Instance);
    }

    [Fact]
    public async Task Issue_WithEdDSA_ProducesValidSdJwt()
    {
        var request = CreateRequest();
        var result = await _issuer.IssueAsync(request);

        // Check JWT header says EdDSA
        var jwt = result.SerializedCredential.Split('~')[0];
        var headerB64 = jwt.Split('.')[0];
        var headerJson = DecodeBase64Url(headerB64);
        var header = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(headerJson);

        Assert.Equal("EdDSA", header!["alg"].GetString());
        Assert.Equal("vc+sd-jwt", header["typ"].GetString());
    }

    [Fact]
    public async Task Issue_ThenVerify_RoundtripSucceeds_WithEd25519()
    {
        var request = CreateRequest();
        var issued = await _issuer.IssueAsync(request);

        var verification = await _issuer.VerifyAsync(issued.SerializedCredential);

        Assert.True(verification.IsValid);
        Assert.Null(verification.Error);
        Assert.Equal("did:key:z6MkEdDSAIssuer", verification.IssuerDid);
    }

    [Fact]
    public async Task Verify_TamperedEd25519Signature_Fails()
    {
        var request = CreateRequest();
        var issued = await _issuer.IssueAsync(request);

        // Tamper with the signature
        var parts = issued.SerializedCredential.Split('~');
        var jwtParts = parts[0].Split('.');
        // Flip a byte in the signature
        var sigBytes = Convert.FromBase64String(
            jwtParts[2].Replace('-', '+').Replace('_', '/').PadRight(
                jwtParts[2].Length + (4 - jwtParts[2].Length % 4) % 4, '='));
        sigBytes[0] ^= 0xFF;
        jwtParts[2] = Convert.ToBase64String(sigBytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        parts[0] = string.Join('.', jwtParts);
        var tampered = string.Join('~', parts);

        var verification = await _issuer.VerifyAsync(tampered);

        Assert.False(verification.IsValid);
        Assert.Equal("Invalid signature", verification.Error);
    }

    [Fact]
    public async Task Ed25519Signature_Is64Bytes()
    {
        var request = CreateRequest();
        var issued = await _issuer.IssueAsync(request);

        var jwt = issued.SerializedCredential.Split('~')[0];
        var sigB64 = jwt.Split('.')[2];
        var sigBytes = DecodeBase64UrlBytes(sigB64);

        Assert.Equal(64, sigBytes.Length);
    }

    [Fact]
    public async Task BackwardCompat_HS256Key_IssuesAndVerifies()
    {
        // Manually seed an HS256 key
        var hmacKey = RandomNumberGenerator.GetBytes(32);
        _db.SigningKeys.Add(new SigningKeyRecord
        {
            IssuerDid = "did:key:z6MkHS256Issuer",
            KeyMaterial = hmacKey,
            PublicKeyMaterial = null,
            Algorithm = "HS256",
            CreatedAt = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();

        var request = CreateRequest();
        request.IssuerDid = "did:key:z6MkHS256Issuer";

        var issued = await _issuer.IssueAsync(request);

        // Should use HS256
        var jwt = issued.SerializedCredential.Split('~')[0];
        var headerJson = DecodeBase64Url(jwt.Split('.')[0]);
        var header = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(headerJson);
        Assert.Equal("HS256", header!["alg"].GetString());

        // Should verify
        var verification = await _issuer.VerifyAsync(issued.SerializedCredential);
        Assert.True(verification.IsValid);
    }

    [Fact]
    public async Task BackwardCompat_HS256Key_ProducesHS256Header()
    {
        var hmacKey = RandomNumberGenerator.GetBytes(32);
        _db.SigningKeys.Add(new SigningKeyRecord
        {
            IssuerDid = "did:key:z6MkHS256Only",
            KeyMaterial = hmacKey,
            PublicKeyMaterial = null,
            Algorithm = "HS256",
            CreatedAt = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();

        var request = CreateRequest();
        request.IssuerDid = "did:key:z6MkHS256Only";
        var issued = await _issuer.IssueAsync(request);

        var jwt = issued.SerializedCredential.Split('~')[0];
        var headerJson = DecodeBase64Url(jwt.Split('.')[0]);
        var header = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(headerJson);

        Assert.Equal("HS256", header!["alg"].GetString());
    }

    [Fact]
    public async Task NewIssuer_DefaultsToEdDSA()
    {
        var algorithm = await _keyStore.GetAlgorithmAsync("did:key:z6MkNewIssuer");
        // New issuer (not in DB) should default to EdDSA
        Assert.Equal("EdDSA", algorithm);
    }

    [Fact]
    public async Task KeyStore_StoresAndRetrieves_Ed25519KeyPair()
    {
        var signingKey = await _keyStore.GetOrCreateSigningKeyAsync("did:key:z6MkKeyPairTest");
        var publicKey = await _keyStore.GetPublicKeyAsync("did:key:z6MkKeyPairTest");

        Assert.NotNull(signingKey);
        Assert.NotNull(publicKey);
        Assert.NotEqual(signingKey, publicKey); // Private != Public for Ed25519
    }

    [Fact]
    public async Task PublicKey_Is32Bytes()
    {
        await _keyStore.GetOrCreateSigningKeyAsync("did:key:z6MkPubKeySize");
        var publicKey = await _keyStore.GetPublicKeyAsync("did:key:z6MkPubKeySize");

        Assert.NotNull(publicKey);
        Assert.Equal(32, publicKey!.Length);
    }

    [Fact]
    public async Task DidKeyManager_GeneratesRealEd25519Keys()
    {
        var manager = new DidKeyManager();
        var doc = await manager.CreateDidAsync("key");

        Assert.StartsWith("did:key:z", doc.Id);
        Assert.NotEmpty(doc.VerificationMethod!);

        // Verify the key can be resolved back — decode multibase, check multicodec prefix
        var multibaseKey = doc.VerificationMethod[0].PublicKeyMultibase!;
        Assert.StartsWith("z", multibaseKey);

        // The key should be a valid 32-byte Ed25519 public key
        var decoded = Base58.Decode(multibaseKey[1..]);
        Assert.Equal(32, decoded.Length);

        // Verify we can import it as an Ed25519 public key
        var pubKey = PublicKey.Import(SignatureAlgorithm.Ed25519, decoded, KeyBlobFormat.RawPublicKey);
        Assert.NotNull(pubKey);
    }

    public void Dispose() => _db.Dispose();

    private static CredentialIssuanceRequest CreateRequest() => new()
    {
        IssuerDid = "did:key:z6MkEdDSAIssuer",
        SubjectDid = "did:key:z6MkEdDSAStudent",
        CredentialType = "I20Credential",
        ValidityDays = 365,
        Claims = new Dictionary<string, object>
        {
            ["sevisId"] = "N0001234567",
            ["studentName"] = "Alice Smith",
            ["programStatus"] = "Active",
            ["educationLevel"] = "PhD",
            ["primaryMajor"] = "Cryptography",
            ["programStartDate"] = "2025-08-15",
            ["programEndDate"] = "2029-05-15",
            ["institutionName"] = "Crypto University"
        }
    };

    private static string DecodeBase64Url(string input)
    {
        var bytes = DecodeBase64UrlBytes(input);
        return Encoding.UTF8.GetString(bytes);
    }

    private static byte[] DecodeBase64UrlBytes(string input)
    {
        var padded = input.Replace('-', '+').Replace('_', '/');
        switch (padded.Length % 4)
        {
            case 2: padded += "=="; break;
            case 3: padded += "="; break;
        }
        return Convert.FromBase64String(padded);
    }
}
