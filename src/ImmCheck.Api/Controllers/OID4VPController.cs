using Microsoft.AspNetCore.Mvc;
using ImmCheck.Core.SSI.Credentials;
using ImmCheck.Core.SSI.OID4VC;
using ImmCheck.Infrastructure.SSI.OID4VC;

namespace ImmCheck.Api.Controllers;

/// <summary>
/// OpenID for Verifiable Presentations (OID4VP) endpoints.
/// Allows verifiers to request and receive credential presentations.
/// </summary>
[Route("api/oid4vp")]
[ApiController]
public class OID4VPController : ControllerBase
{
    private readonly PresentationStore _presentationStore;
    private readonly ICredentialIssuer _issuer;

    public OID4VPController(PresentationStore presentationStore, ICredentialIssuer issuer)
    {
        _presentationStore = presentationStore;
        _issuer = issuer;
    }

    /// <summary>
    /// Create a presentation request.
    /// The verifier specifies what credentials they want to see.
    /// Returns a request that can be sent to the holder's wallet.
    /// </summary>
    [HttpPost("request")]
    public ActionResult<PresentationRequest> CreateRequest([FromBody] CreatePresentationRequest request)
    {
        var definition = request.PresentationDefinition;

        // Use predefined definitions if a scenario is specified
        if (!string.IsNullOrEmpty(request.Scenario))
        {
            definition = request.Scenario switch
            {
                "f1-status" => PredefinedPresentations.ProveF1Status(),
                "financial-support" => PredefinedPresentations.ProveFinancialSupport(),
                _ => definition
            };
        }

        if (definition == null)
            return BadRequest(new { error = "Either scenario or presentationDefinition is required" });

        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var (state, nonce) = _presentationStore.CreateRequest(definition);

        var presentationRequest = new PresentationRequest
        {
            ResponseType = "vp_token",
            PresentationDefinition = definition,
            Nonce = nonce,
            ResponseUri = $"{baseUrl}/api/oid4vp/response",
            State = state
        };

        return Ok(presentationRequest);
    }

    /// <summary>
    /// Receive and verify a presentation response from the holder's wallet.
    /// </summary>
    [HttpPost("response")]
    public async Task<ActionResult<PresentationVerificationResult>> ReceiveResponse(
        [FromBody] PresentationResponse response)
    {
        if (string.IsNullOrEmpty(response.VpToken))
            return BadRequest(new { error = "vp_token is required" });

        // Verify the presented credential
        var verificationResult = await _issuer.VerifyAsync(response.VpToken);

        var result = new PresentationVerificationResult
        {
            IsValid = verificationResult.IsValid,
            Error = verificationResult.Error,
            IssuerDid = verificationResult.IssuerDid,
            SubjectDid = verificationResult.SubjectDid,
            DisclosedClaims = verificationResult.DisclosedClaims,
            PresentationDefinitionId = response.PresentationSubmission?.DefinitionId
        };

        // Store the result
        if (!string.IsNullOrEmpty(response.State))
        {
            _presentationStore.CompleteRequest(response.State, result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Check the status of a presentation request.
    /// </summary>
    [HttpGet("status/{state}")]
    public ActionResult GetStatus(string state)
    {
        var pending = _presentationStore.GetRequest(state);
        if (pending == null)
            return NotFound(new { error = "Presentation request not found or expired" });

        return Ok(new
        {
            state = pending.State,
            status = pending.Result != null ? "completed" : "pending",
            result = pending.Result
        });
    }

    /// <summary>
    /// List predefined presentation scenarios.
    /// </summary>
    [HttpGet("scenarios")]
    public ActionResult GetScenarios()
    {
        return Ok(new[]
        {
            new { id = "f1-status", name = "Prove F-1 Student Status", description = "Verify valid F-1 enrollment via I-20 credential" },
            new { id = "financial-support", name = "Prove Financial Support", description = "Verify sufficient funding for studies" }
        });
    }
}

public class CreatePresentationRequest
{
    /// <summary>Use a predefined scenario: "f1-status" or "financial-support".</summary>
    public string? Scenario { get; set; }

    /// <summary>Custom presentation definition (if not using a predefined scenario).</summary>
    public PresentationDefinition? PresentationDefinition { get; set; }
}

/// <summary>
/// Predefined presentation definitions for common verification scenarios.
/// </summary>
public static class PredefinedPresentations
{
    public static PresentationDefinition ProveF1Status() => new()
    {
        Id = "f1-status-verification",
        Name = "F-1 Student Status Verification",
        Purpose = "Verify that the holder has a valid F-1 student enrollment status",
        InputDescriptors = new List<InputDescriptor>
        {
            new()
            {
                Id = "i20-credential",
                Name = "I-20 Credential",
                Purpose = "Proof of enrollment in a U.S. educational institution",
                Format = new Dictionary<string, FormatRequirement>
                {
                    ["vc+sd-jwt"] = new() { Alg = new List<string> { "HS256" } }
                },
                Constraints = new Constraints
                {
                    Fields = new List<FieldConstraint>
                    {
                        new()
                        {
                            Path = new List<string> { "$.vc.type" },
                            Filter = new FilterConstraint { Type = "string", Const = "I20Credential" }
                        },
                        new()
                        {
                            Path = new List<string> { "$.vc.credentialSubject.programStatus" }
                        },
                        new()
                        {
                            Path = new List<string> { "$.vc.credentialSubject.institutionName" }
                        }
                    }
                }
            }
        }
    };

    public static PresentationDefinition ProveFinancialSupport() => new()
    {
        Id = "financial-support-verification",
        Name = "Financial Support Verification",
        Purpose = "Verify that the holder has sufficient financial support for studies",
        InputDescriptors = new List<InputDescriptor>
        {
            new()
            {
                Id = "financial-credential",
                Name = "Financial Support Credential",
                Purpose = "Proof of funding for academic expenses",
                Format = new Dictionary<string, FormatRequirement>
                {
                    ["vc+sd-jwt"] = new() { Alg = new List<string> { "HS256" } }
                },
                Constraints = new Constraints
                {
                    Fields = new List<FieldConstraint>
                    {
                        new()
                        {
                            Path = new List<string> { "$.vc.type" },
                            Filter = new FilterConstraint { Type = "string", Const = "FinancialSupportCredential" }
                        },
                        new()
                        {
                            Path = new List<string> { "$.vc.credentialSubject.totalFunding" }
                        },
                        new()
                        {
                            Path = new List<string> { "$.vc.credentialSubject.totalExpenses" }
                        }
                    }
                }
            }
        }
    };
}
