using System.Net;
using System.Text;
using ImmCheck.Infrastructure.SSI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;

namespace ImmCheck.Api.Tests.SSI;

public class DidCheqdResolverTests
{
    private static DidCheqdResolver CreateResolver(HttpMessageHandler handler)
    {
        var httpClient = new HttpClient(handler);
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Cheqd:ResolverUrl"] = "https://resolver.cheqd.net"
            })
            .Build();
        return new DidCheqdResolver(httpClient, config, NullLogger<DidCheqdResolver>.Instance);
    }

    [Fact]
    public void CanResolve_CheqdDid_ReturnsTrue()
    {
        var resolver = CreateResolver(new FakeHandler(HttpStatusCode.OK, "{}"));
        Assert.True(resolver.CanResolve("did:cheqd:mainnet:abc123"));
    }

    [Fact]
    public void CanResolve_OtherMethod_ReturnsFalse()
    {
        var resolver = CreateResolver(new FakeHandler(HttpStatusCode.OK, "{}"));
        Assert.False(resolver.CanResolve("did:key:z6MkTest"));
        Assert.False(resolver.CanResolve("did:web:example.com"));
        Assert.False(resolver.CanResolve("did:prism:abc"));
    }

    [Fact]
    public void Method_ReturnsCheqd()
    {
        var resolver = CreateResolver(new FakeHandler(HttpStatusCode.OK, "{}"));
        Assert.Equal("cheqd", resolver.Method);
    }

    [Fact]
    public async Task Resolve_ValidResponse_ReturnsDidDocument()
    {
        var responseJson = """
        {
            "didDocument": {
                "@context": ["https://www.w3.org/ns/did/v1"],
                "id": "did:cheqd:mainnet:abc123",
                "verificationMethod": [
                    {
                        "id": "did:cheqd:mainnet:abc123#key-1",
                        "type": "Ed25519VerificationKey2020",
                        "controller": "did:cheqd:mainnet:abc123",
                        "publicKeyMultibase": "z6MkTest"
                    }
                ],
                "authentication": ["did:cheqd:mainnet:abc123#key-1"],
                "assertionMethod": ["did:cheqd:mainnet:abc123#key-1"]
            },
            "didResolutionMetadata": {},
            "didDocumentMetadata": {}
        }
        """;

        var resolver = CreateResolver(new FakeHandler(HttpStatusCode.OK, responseJson));
        var doc = await resolver.ResolveAsync("did:cheqd:mainnet:abc123");

        Assert.NotNull(doc);
        Assert.Equal("did:cheqd:mainnet:abc123", doc.Id);
        Assert.Single(doc.VerificationMethod!);
        Assert.Equal("Ed25519VerificationKey2020", doc.VerificationMethod[0].Type);
        Assert.Equal("z6MkTest", doc.VerificationMethod[0].PublicKeyMultibase);
    }

    [Fact]
    public async Task Resolve_404_ReturnsNull()
    {
        var resolver = CreateResolver(new FakeHandler(HttpStatusCode.NotFound, ""));
        var doc = await resolver.ResolveAsync("did:cheqd:mainnet:nonexistent");

        Assert.Null(doc);
    }

    [Fact]
    public async Task Resolve_NetworkError_ReturnsNull()
    {
        var resolver = CreateResolver(new ThrowingHandler());
        var doc = await resolver.ResolveAsync("did:cheqd:mainnet:abc123");

        Assert.Null(doc);
    }

    /// <summary>Fake HTTP handler that returns a configured response.</summary>
    private class FakeHandler : HttpMessageHandler
    {
        private readonly HttpStatusCode _statusCode;
        private readonly string _content;

        public FakeHandler(HttpStatusCode statusCode, string content)
        {
            _statusCode = statusCode;
            _content = content;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new HttpResponseMessage(_statusCode)
            {
                Content = new StringContent(_content, Encoding.UTF8, "application/json")
            });
        }
    }

    /// <summary>Fake HTTP handler that throws a network exception.</summary>
    private class ThrowingHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            throw new HttpRequestException("Connection refused");
        }
    }
}
