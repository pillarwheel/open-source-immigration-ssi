using Microsoft.AspNetCore.Mvc;
using ImmCheck.Core.SSI.Credentials;

namespace ImmCheck.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CredentialController : ControllerBase
{
    private readonly ICredentialIssuer _issuer;
    private readonly ICredentialRepository _repository;

    public CredentialController(ICredentialIssuer issuer, ICredentialRepository repository)
    {
        _issuer = issuer;
        _repository = repository;
    }

    /// <summary>
    /// Issue a new Verifiable Credential (SD-JWT format).
    /// </summary>
    [HttpPost("issue")]
    public async Task<ActionResult<IssuedCredential>> Issue([FromBody] CredentialIssuanceRequest request)
    {
        if (string.IsNullOrEmpty(request.IssuerDid))
            return BadRequest(new { error = "issuerDid is required" });
        if (string.IsNullOrEmpty(request.SubjectDid))
            return BadRequest(new { error = "subjectDid is required" });
        if (string.IsNullOrEmpty(request.CredentialType))
            return BadRequest(new { error = "credentialType is required" });

        var schema = CredentialSchemas.GetSchema(request.CredentialType);
        if (schema == null)
            return BadRequest(new { error = $"Unknown credential type: {request.CredentialType}. Supported: I20Credential, FinancialSupportCredential" });

        // Validate required claims
        var missingClaims = schema.RequiredClaims
            .Where(c => !request.Claims.ContainsKey(c))
            .ToList();
        if (missingClaims.Any())
            return BadRequest(new { error = $"Missing required claims: {string.Join(", ", missingClaims)}" });

        var result = await _issuer.IssueAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.CredentialId }, result);
    }

    /// <summary>
    /// Get a credential by its ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult> GetById(string id)
    {
        var record = await _repository.GetByIdAsync(id);
        if (record == null)
            return NotFound(new { error = $"Credential {id} not found" });

        return Ok(new
        {
            record.Id,
            record.IssuerDid,
            record.SubjectDid,
            record.CredentialType,
            record.SerializedCredential,
            record.IssuedAt,
            record.ExpiresAt,
            record.IsRevoked,
            record.RevokedAt,
            Format = "vc+sd-jwt"
        });
    }

    /// <summary>
    /// Verify a presented credential (SD-JWT format).
    /// </summary>
    [HttpPost("verify")]
    public async Task<ActionResult<CredentialVerificationResult>> Verify([FromBody] VerifyRequest request)
    {
        if (string.IsNullOrEmpty(request.Credential))
            return BadRequest(new { error = "credential is required" });

        var result = await _issuer.VerifyAsync(request.Credential);
        return Ok(result);
    }

    /// <summary>
    /// Get all credentials issued to a specific subject (student DID).
    /// </summary>
    [HttpGet("subject/{subjectDid}")]
    public async Task<ActionResult> GetBySubject(string subjectDid)
    {
        subjectDid = Uri.UnescapeDataString(subjectDid);
        var credentials = await _repository.GetBySubjectAsync(subjectDid);
        return Ok(credentials.Select(c => new
        {
            c.Id,
            c.IssuerDid,
            c.SubjectDid,
            c.CredentialType,
            c.IssuedAt,
            c.ExpiresAt,
            c.IsRevoked
        }));
    }

    /// <summary>
    /// Revoke a credential by ID.
    /// </summary>
    [HttpPost("{id}/revoke")]
    public async Task<ActionResult> Revoke(string id)
    {
        var revoked = await _repository.RevokeAsync(id);
        if (!revoked)
            return NotFound(new { error = $"Credential {id} not found or already revoked" });

        return Ok(new { message = $"Credential {id} has been revoked" });
    }

    /// <summary>
    /// Get the Bitstring Status List for an issuer (for credential revocation checks).
    /// See: https://www.w3.org/TR/vc-bitstring-status-list/
    /// </summary>
    [HttpGet("status-list/{issuerDid}")]
    public async Task<ActionResult> GetStatusList(string issuerDid)
    {
        issuerDid = Uri.UnescapeDataString(issuerDid);
        var statusList = await _repository.GetStatusListAsync(issuerDid);
        if (statusList == null)
            return NotFound(new { error = $"No status list found for issuer: {issuerDid}" });

        return Ok(new
        {
            statusList.IssuerDid,
            statusList.EncodedList,
            statusList.Size,
            statusList.LastUpdated,
            type = "BitstringStatusListCredential"
        });
    }

    /// <summary>
    /// List available credential schemas.
    /// </summary>
    [HttpGet("schemas")]
    public ActionResult GetSchemas()
    {
        return Ok(CredentialSchemas.GetAllSchemas());
    }
}

public class VerifyRequest
{
    public string Credential { get; set; } = string.Empty;
}
