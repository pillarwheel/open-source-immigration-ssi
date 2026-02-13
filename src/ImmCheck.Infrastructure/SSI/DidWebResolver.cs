using System.Net.Http.Json;
using ImmCheck.Core.SSI;

namespace ImmCheck.Infrastructure.SSI;

/// <summary>
/// Resolves did:web identifiers by fetching the DID document over HTTPS.
/// See: https://w3c-ccg.github.io/did-method-web/
///
/// Resolution algorithm:
///   did:web:example.com       → https://example.com/.well-known/did.json
///   did:web:example.com:path  → https://example.com/path/did.json
///   Colons in the method-specific identifier are replaced with slashes.
///   Percent-encoded characters are decoded.
/// </summary>
public class DidWebResolver : IDidResolver
{
    private readonly HttpClient _httpClient;

    public DidWebResolver(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public string Method => "web";

    public bool CanResolve(string did) => did.StartsWith("did:web:");

    public async Task<DidDocument?> ResolveAsync(string did)
    {
        if (!CanResolve(did))
            return null;

        var url = DidToUrl(did);
        if (url == null)
            return null;

        try
        {
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return null;

            var doc = await response.Content.ReadFromJsonAsync<DidDocument>();
            return doc;
        }
        catch
        {
            return null;
        }
    }

    internal static string? DidToUrl(string did)
    {
        var parts = did.Split(':');
        if (parts.Length < 3 || parts[0] != "did" || parts[1] != "web")
            return null;

        // Decode percent-encoded domain
        var domain = Uri.UnescapeDataString(parts[2]);

        if (parts.Length == 3)
        {
            // did:web:example.com → https://example.com/.well-known/did.json
            return $"https://{domain}/.well-known/did.json";
        }

        // did:web:example.com:path:to:resource → https://example.com/path/to/resource/did.json
        var pathSegments = parts[3..].Select(Uri.UnescapeDataString);
        var path = string.Join("/", pathSegments);
        return $"https://{domain}/{path}/did.json";
    }
}
