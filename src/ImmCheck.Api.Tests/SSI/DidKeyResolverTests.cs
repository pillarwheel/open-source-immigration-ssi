using ImmCheck.Infrastructure.SSI;

namespace ImmCheck.Api.Tests.SSI;

public class DidKeyResolverTests
{
    private readonly DidKeyResolver _resolver = new();

    [Fact]
    public void CanResolve_DidKey_ReturnsTrue()
    {
        Assert.True(_resolver.CanResolve("did:key:z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK"));
    }

    [Fact]
    public void CanResolve_OtherMethod_ReturnsFalse()
    {
        Assert.False(_resolver.CanResolve("did:web:example.com"));
        Assert.False(_resolver.CanResolve("did:prism:abc123"));
        Assert.False(_resolver.CanResolve("not-a-did"));
    }

    [Fact]
    public async Task Resolve_ValidEd25519Key_ReturnsDocument()
    {
        // did:key with Ed25519 multicodec prefix (0xed01)
        var did = "did:key:z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK";
        var doc = await _resolver.ResolveAsync(did);

        Assert.NotNull(doc);
        Assert.Equal(did, doc.Id);
        Assert.NotNull(doc.VerificationMethod);
        Assert.Single(doc.VerificationMethod);
        Assert.Equal("Ed25519VerificationKey2020", doc.VerificationMethod[0].Type);
        Assert.Equal(did, doc.VerificationMethod[0].Controller);
        Assert.NotNull(doc.Authentication);
        Assert.NotEmpty(doc.Authentication);
        Assert.NotNull(doc.AssertionMethod);
        Assert.NotEmpty(doc.AssertionMethod);
    }

    [Fact]
    public async Task Resolve_InvalidDid_ReturnsNull()
    {
        var doc = await _resolver.ResolveAsync("did:web:example.com");
        Assert.Null(doc);
    }

    [Fact]
    public async Task Resolve_InvalidMultibase_ReturnsNull()
    {
        // 'z' prefix but invalid base58
        var doc = await _resolver.ResolveAsync("did:key:z0000");
        Assert.Null(doc);
    }

    [Fact]
    public void Method_ReturnsKey()
    {
        Assert.Equal("key", _resolver.Method);
    }
}

public class DidWebResolverTests
{
    [Theory]
    [InlineData("did:web:example.com", "https://example.com/.well-known/did.json")]
    [InlineData("did:web:w3c-ccg.github.io:user:alice", "https://w3c-ccg.github.io/user/alice/did.json")]
    [InlineData("did:web:example.com%3A3000:user:alice", "https://example.com:3000/user/alice/did.json")]
    public void DidToUrl_ConvertsCorrectly(string did, string expectedUrl)
    {
        var url = DidWebResolver.DidToUrl(did);
        Assert.Equal(expectedUrl, url);
    }

    [Theory]
    [InlineData("not-a-did")]
    [InlineData("did:key:z6MkTest")]
    [InlineData("did:")]
    public void DidToUrl_InvalidDid_ReturnsNull(string did)
    {
        var url = DidWebResolver.DidToUrl(did);
        Assert.Null(url);
    }
}

public class Base58Tests
{
    [Fact]
    public void Encode_Decode_Roundtrip()
    {
        var original = new byte[] { 0xed, 0x01, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var encoded = Base58.Encode(original);
        var decoded = Base58.Decode(encoded);
        Assert.Equal(original, decoded);
    }

    [Fact]
    public void Decode_InvalidCharacter_Throws()
    {
        Assert.Throws<FormatException>(() => Base58.Decode("0OIl")); // 0, O, I, l not in Base58
    }

    [Fact]
    public void Encode_EmptyArray_ReturnsEmpty()
    {
        var encoded = Base58.Encode(Array.Empty<byte>());
        Assert.Equal("", encoded);
    }

    [Fact]
    public void Encode_LeadingZeros_PreservesCount()
    {
        var data = new byte[] { 0, 0, 1 };
        var encoded = Base58.Encode(data);
        Assert.StartsWith("11", encoded); // Leading zeros become '1's
        var decoded = Base58.Decode(encoded);
        Assert.Equal(data, decoded);
    }
}
