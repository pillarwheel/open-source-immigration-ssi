using Microsoft.AspNetCore.Mvc;
using ImmCheck.Core.SSI;
using ImmCheck.Infrastructure.SSI;

namespace ImmCheck.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DidController : ControllerBase
{
    private readonly UniversalDidResolver _resolver;
    private readonly IEnumerable<IDidManager> _managers;
    private readonly IDidPublisher? _publisher;

    public DidController(
        UniversalDidResolver resolver,
        IEnumerable<IDidManager> managers,
        IDidPublisher? publisher = null)
    {
        _resolver = resolver;
        _managers = managers;
        _publisher = publisher;
    }

    /// <summary>
    /// Resolve a DID to its DID Document.
    /// Supports did:key, did:web, did:prism, and did:cheqd methods.
    /// </summary>
    [HttpGet("resolve/{*did}")]
    public async Task<ActionResult<DidResolutionResult>> Resolve(string did)
    {
        // URL decoding: the DID comes in URL-encoded, decode colons
        did = Uri.UnescapeDataString(did);

        var result = await _resolver.ResolveAsync(did);

        if (result.Metadata.Error == "invalidDid")
            return BadRequest(new { error = "Invalid DID format. Must start with 'did:'" });

        if (result.Metadata.Error == "notFound" || result.DidDocument == null)
            return NotFound(new { error = $"Could not resolve DID: {did}", method = ExtractMethod(did) });

        return Ok(result);
    }

    /// <summary>
    /// Create a new DID of the specified method.
    /// </summary>
    [HttpPost("create")]
    public async Task<ActionResult<DidDocument>> Create([FromBody] CreateDidRequest request)
    {
        var method = request.Method?.ToLowerInvariant();

        if (string.IsNullOrEmpty(method))
            return BadRequest(new { error = "DID method is required (e.g., 'key', 'web', 'prism')" });

        var manager = _managers.FirstOrDefault(m =>
            m is DidKeyManager && method == "key" ||
            m is DidPrismResolver && method == "prism");

        if (manager == null)
            return BadRequest(new { error = $"Unsupported DID method for creation: {method}. Supported: key, prism" });

        var options = new DidCreationOptions
        {
            Domain = request.Domain,
            Path = request.Path,
            AgentApiUrl = request.AgentApiUrl,
            ApiKey = request.ApiKey
        };

        var doc = await manager.CreateDidAsync(method, options);
        return CreatedAtAction(nameof(Resolve), new { did = doc.Id }, doc);
    }

    /// <summary>
    /// Publish a DID to its blockchain. Currently supports did:prism (Cardano).
    /// </summary>
    [HttpPost("publish/{*did}")]
    public async Task<ActionResult<DidPublicationResult>> Publish(string did)
    {
        did = Uri.UnescapeDataString(did);

        if (!did.StartsWith("did:prism:"))
            return BadRequest(new { error = "Only did:prism DIDs can be published to Cardano" });

        if (_publisher == null)
            return StatusCode(503, new { error = "DID publisher service is not available" });

        var result = await _publisher.PublishAsync(did);

        if (!result.Success && result.Error?.Contains("Cannot reach") == true)
            return StatusCode(502, new { error = result.Error });

        if (!result.Success)
            return BadRequest(new { error = result.Error });

        return Ok(result);
    }

    /// <summary>
    /// Get the publication status of a DID on its blockchain.
    /// </summary>
    [HttpGet("status/{*did}")]
    public async Task<ActionResult<DidStatusResult>> GetStatus(string did)
    {
        did = Uri.UnescapeDataString(did);

        if (!did.StartsWith("did:prism:"))
            return BadRequest(new { error = "Status tracking is only available for did:prism DIDs" });

        if (_publisher == null)
            return StatusCode(503, new { error = "DID publisher service is not available" });

        var result = await _publisher.GetStatusAsync(did);
        return Ok(result);
    }

    /// <summary>
    /// List supported DID methods.
    /// </summary>
    [HttpGet("methods")]
    public ActionResult<object> GetSupportedMethods()
    {
        return Ok(new
        {
            resolve = _resolver.SupportedMethods,
            create = new[] { "key", "prism" }
        });
    }

    private static string? ExtractMethod(string did)
    {
        var parts = did.Split(':');
        return parts.Length >= 2 ? parts[1] : null;
    }
}

public class CreateDidRequest
{
    public string? Method { get; set; }
    public string? Domain { get; set; }
    public string? Path { get; set; }
    public string? AgentApiUrl { get; set; }
    public string? ApiKey { get; set; }
}
