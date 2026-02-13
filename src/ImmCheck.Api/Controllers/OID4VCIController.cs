using Microsoft.AspNetCore.Mvc;
using ImmCheck.Core.SSI.Credentials;
using ImmCheck.Core.SSI.OID4VC;
using ImmCheck.Infrastructure.SSI.OID4VC;

namespace ImmCheck.Api.Controllers;

/// <summary>
/// OpenID for Verifiable Credential Issuance (OID4VCI) endpoints.
/// Implements the pre-authorized code flow for credential issuance.
/// </summary>
[Route("api/oid4vci")]
[ApiController]
public class OID4VCIController : ControllerBase
{
    private readonly OfferStore _offerStore;
    private readonly ICredentialIssuer _issuer;

    public OID4VCIController(OfferStore offerStore, ICredentialIssuer issuer)
    {
        _offerStore = offerStore;
        _issuer = issuer;
    }

    /// <summary>
    /// Credential Issuer Metadata endpoint.
    /// GET /.well-known/openid-credential-issuer
    /// </summary>
    [HttpGet("/.well-known/openid-credential-issuer")]
    [HttpGet("metadata")]
    public ActionResult<CredentialIssuerMetadata> GetMetadata()
    {
        var baseUrl = $"{Request.Scheme}://{Request.Host}";

        var metadata = new CredentialIssuerMetadata
        {
            CredentialIssuer = baseUrl,
            CredentialEndpoint = $"{baseUrl}/api/oid4vci/credential",
            TokenEndpoint = $"{baseUrl}/api/oid4vci/token",
            CredentialConfigurationsSupported = new Dictionary<string, CredentialConfiguration>
            {
                ["I20Credential"] = new()
                {
                    Format = "vc+sd-jwt",
                    Scope = "i20_credential",
                    CredentialDefinition = new CredentialDefinition
                    {
                        Type = new List<string> { "VerifiableCredential", "I20Credential" }
                    },
                    Display = new List<CredentialDisplay>
                    {
                        new()
                        {
                            Name = "I-20 Certificate of Eligibility",
                            Locale = "en-US",
                            Description = "Proof of F-1 student enrollment status"
                        }
                    }
                },
                ["FinancialSupportCredential"] = new()
                {
                    Format = "vc+sd-jwt",
                    Scope = "financial_credential",
                    CredentialDefinition = new CredentialDefinition
                    {
                        Type = new List<string> { "VerifiableCredential", "FinancialSupportCredential" }
                    },
                    Display = new List<CredentialDisplay>
                    {
                        new()
                        {
                            Name = "Financial Support Attestation",
                            Locale = "en-US",
                            Description = "Proof of financial support for F-1 student visa"
                        }
                    }
                }
            }
        };

        return Ok(metadata);
    }

    /// <summary>
    /// Create a credential offer (pre-authorized code flow).
    /// DSO triggers this to start the issuance process.
    /// Returns a credential offer that can be sent to the student's wallet (QR code / deep link).
    /// </summary>
    [HttpPost("credential-offer")]
    public ActionResult<CredentialOffer> CreateOffer([FromBody] CreateOfferRequest request)
    {
        if (string.IsNullOrEmpty(request.CredentialConfigurationId))
            return BadRequest(new { error = "credentialConfigurationId is required" });

        var issuanceRequest = new CredentialIssuanceRequest
        {
            IssuerDid = request.IssuerDid ?? "",
            SubjectDid = request.SubjectDid ?? "",
            CredentialType = request.CredentialConfigurationId,
            Claims = request.Claims ?? new Dictionary<string, object>(),
            ValidityDays = request.ValidityDays
        };

        var (offerId, preAuthCode) = _offerStore.CreateOffer(issuanceRequest, request.CredentialConfigurationId);
        var baseUrl = $"{Request.Scheme}://{Request.Host}";

        var offer = new CredentialOffer
        {
            CredentialIssuer = baseUrl,
            CredentialConfigurationIds = new List<string> { request.CredentialConfigurationId },
            Grants = new OfferGrants
            {
                PreAuthorizedCode = new PreAuthorizedCodeGrant
                {
                    PreAuthorizedCodeValue = preAuthCode
                }
            }
        };

        return Ok(offer);
    }

    /// <summary>
    /// Token endpoint — exchange pre-authorized code for access token.
    /// </summary>
    [HttpPost("token")]
    public ActionResult<TokenResponse> ExchangeToken([FromForm] TokenRequest request)
    {
        if (request.GrantType != "urn:ietf:params:oauth:grant-type:pre-authorized_code")
            return BadRequest(new { error = "unsupported_grant_type" });

        if (string.IsNullOrEmpty(request.PreAuthorizedCode))
            return BadRequest(new { error = "invalid_grant", error_description = "pre-authorized_code is required" });

        var result = _offerStore.ExchangePreAuthCode(request.PreAuthorizedCode);
        if (result == null)
            return BadRequest(new { error = "invalid_grant", error_description = "Invalid or expired pre-authorized code" });

        var (accessToken, session) = result.Value;

        return Ok(new TokenResponse
        {
            AccessToken = accessToken,
            TokenType = "Bearer",
            ExpiresIn = 3600,
            CNonce = session.CNonce,
            CNonceExpiresIn = 300
        });
    }

    /// <summary>
    /// Credential endpoint — issue the credential using the access token.
    /// </summary>
    [HttpPost("credential")]
    public async Task<ActionResult<CredentialResponse>> IssueCredential([FromBody] CredentialRequest request)
    {
        // Extract access token from Authorization header
        var authHeader = Request.Headers.Authorization.FirstOrDefault();
        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            return Unauthorized(new { error = "invalid_token" });

        var accessToken = authHeader["Bearer ".Length..];
        var session = _offerStore.GetSession(accessToken);
        if (session == null)
            return Unauthorized(new { error = "invalid_token", error_description = "Token expired or invalid" });

        // Issue the credential
        var issued = await _issuer.IssueAsync(session.Request);

        // Clean up the session (single use)
        _offerStore.RemoveSession(accessToken);

        return Ok(new CredentialResponse
        {
            Format = "vc+sd-jwt",
            Credential = issued.SerializedCredential
        });
    }
}

public class CreateOfferRequest
{
    public string? IssuerDid { get; set; }
    public string? SubjectDid { get; set; }
    public string? CredentialConfigurationId { get; set; }
    public Dictionary<string, object>? Claims { get; set; }
    public int? ValidityDays { get; set; }
}

public class TokenRequest
{
    [Microsoft.AspNetCore.Mvc.FromForm(Name = "grant_type")]
    public string? GrantType { get; set; }

    [Microsoft.AspNetCore.Mvc.FromForm(Name = "pre-authorized_code")]
    public string? PreAuthorizedCode { get; set; }
}
