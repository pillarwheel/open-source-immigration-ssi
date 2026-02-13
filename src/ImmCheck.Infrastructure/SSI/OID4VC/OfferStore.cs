using System.Collections.Concurrent;
using System.Security.Cryptography;
using ImmCheck.Core.SSI.Credentials;

namespace ImmCheck.Infrastructure.SSI.OID4VC;

/// <summary>
/// In-memory store for OID4VCI credential offers and pre-authorized codes.
/// Maps pre-auth codes to pending issuance requests, and access tokens to authorized sessions.
/// </summary>
public class OfferStore
{
    private readonly ConcurrentDictionary<string, PendingOffer> _offers = new();
    private readonly ConcurrentDictionary<string, AuthorizedSession> _sessions = new();

    public (string offerId, string preAuthCode) CreateOffer(CredentialIssuanceRequest request, string credentialConfigId)
    {
        var offerId = Guid.NewGuid().ToString("N");
        var preAuthCode = GenerateCode();

        _offers[preAuthCode] = new PendingOffer
        {
            OfferId = offerId,
            Request = request,
            CredentialConfigurationId = credentialConfigId,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(10)
        };

        return (offerId, preAuthCode);
    }

    public (string accessToken, AuthorizedSession session)? ExchangePreAuthCode(string preAuthCode)
    {
        if (!_offers.TryRemove(preAuthCode, out var offer))
            return null;

        if (offer.ExpiresAt < DateTime.UtcNow)
            return null;

        var accessToken = GenerateCode();
        var nonce = GenerateCode();

        var session = new AuthorizedSession
        {
            Request = offer.Request,
            CredentialConfigurationId = offer.CredentialConfigurationId,
            CNonce = nonce,
            ExpiresAt = DateTime.UtcNow.AddHours(1)
        };

        _sessions[accessToken] = session;
        return (accessToken, session);
    }

    public AuthorizedSession? GetSession(string accessToken)
    {
        if (_sessions.TryGetValue(accessToken, out var session) && session.ExpiresAt > DateTime.UtcNow)
            return session;
        return null;
    }

    public void RemoveSession(string accessToken)
    {
        _sessions.TryRemove(accessToken, out _);
    }

    private static string GenerateCode() =>
        Convert.ToBase64String(RandomNumberGenerator.GetBytes(24))
            .TrimEnd('=').Replace('+', '-').Replace('/', '_');
}

public class PendingOffer
{
    public string OfferId { get; set; } = string.Empty;
    public CredentialIssuanceRequest Request { get; set; } = new();
    public string CredentialConfigurationId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
}

public class AuthorizedSession
{
    public CredentialIssuanceRequest Request { get; set; } = new();
    public string CredentialConfigurationId { get; set; } = string.Empty;
    public string CNonce { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}
