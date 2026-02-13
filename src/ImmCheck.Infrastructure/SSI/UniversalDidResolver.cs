using System.Diagnostics;
using System.Net.Http.Json;
using ImmCheck.Core.SSI;
using Microsoft.Extensions.Logging;

namespace ImmCheck.Infrastructure.SSI;

/// <summary>
/// Aggregates multiple IDidResolver implementations and routes resolution
/// to the appropriate resolver based on the DID method.
/// Also supports fallback to the DIF Universal Resolver HTTP API.
/// </summary>
public class UniversalDidResolver
{
    private readonly IEnumerable<IDidResolver> _resolvers;
    private readonly HttpClient? _universalResolverClient;
    private readonly ILogger<UniversalDidResolver> _logger;
    private readonly string? _universalResolverUrl;

    public UniversalDidResolver(
        IEnumerable<IDidResolver> resolvers,
        ILogger<UniversalDidResolver> logger,
        HttpClient? universalResolverClient = null,
        string? universalResolverUrl = null)
    {
        _resolvers = resolvers;
        _logger = logger;
        _universalResolverClient = universalResolverClient;
        _universalResolverUrl = universalResolverUrl?.TrimEnd('/');
    }

    /// <summary>
    /// Resolves a DID using the appropriate local resolver, falling back
    /// to the DIF Universal Resolver if no local resolver handles the method.
    /// </summary>
    public async Task<DidResolutionResult> ResolveAsync(string did)
    {
        var sw = Stopwatch.StartNew();
        var result = new DidResolutionResult();

        // Validate DID format
        if (string.IsNullOrWhiteSpace(did) || !did.StartsWith("did:"))
        {
            result.Metadata.Error = "invalidDid";
            result.Metadata.DurationMs = sw.ElapsedMilliseconds;
            return result;
        }

        // Try local resolvers first
        foreach (var resolver in _resolvers)
        {
            if (resolver.CanResolve(did))
            {
                _logger.LogDebug("Resolving {Did} with {Resolver}", did, resolver.GetType().Name);

                var doc = await resolver.ResolveAsync(did);
                if (doc != null)
                {
                    result.DidDocument = doc;
                    result.Metadata.DurationMs = sw.ElapsedMilliseconds;
                    return result;
                }
            }
        }

        // Fallback to DIF Universal Resolver
        if (_universalResolverClient != null && _universalResolverUrl != null)
        {
            _logger.LogDebug("Falling back to Universal Resolver for {Did}", did);

            try
            {
                var response = await _universalResolverClient.GetAsync(
                    $"{_universalResolverUrl}/1.0/identifiers/{Uri.EscapeDataString(did)}");

                if (response.IsSuccessStatusCode)
                {
                    var doc = await response.Content.ReadFromJsonAsync<DidDocument>();
                    if (doc != null)
                    {
                        result.DidDocument = doc;
                        result.Metadata.DurationMs = sw.ElapsedMilliseconds;
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Universal Resolver fallback failed for {Did}", did);
            }
        }

        result.Metadata.Error = "notFound";
        result.Metadata.DurationMs = sw.ElapsedMilliseconds;
        return result;
    }

    /// <summary>Returns the list of DID methods supported by local resolvers.</summary>
    public IEnumerable<string> SupportedMethods => _resolvers.Select(r => r.Method);
}
