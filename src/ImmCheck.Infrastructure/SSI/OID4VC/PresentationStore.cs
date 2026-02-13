using System.Collections.Concurrent;
using System.Security.Cryptography;
using ImmCheck.Core.SSI.OID4VC;

namespace ImmCheck.Infrastructure.SSI.OID4VC;

/// <summary>
/// In-memory store for OID4VP presentation requests and their verification results.
/// </summary>
public class PresentationStore
{
    private readonly ConcurrentDictionary<string, PendingPresentation> _requests = new();

    public (string state, string nonce) CreateRequest(PresentationDefinition definition)
    {
        var state = Guid.NewGuid().ToString("N");
        var nonce = Convert.ToBase64String(RandomNumberGenerator.GetBytes(16))
            .TrimEnd('=').Replace('+', '-').Replace('/', '_');

        _requests[state] = new PendingPresentation
        {
            State = state,
            Nonce = nonce,
            Definition = definition,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(10)
        };

        return (state, nonce);
    }

    public PendingPresentation? GetRequest(string state)
    {
        if (_requests.TryGetValue(state, out var pending) && pending.ExpiresAt > DateTime.UtcNow)
            return pending;
        return null;
    }

    public void CompleteRequest(string state, PresentationVerificationResult result)
    {
        if (_requests.TryGetValue(state, out var pending))
        {
            pending.Result = result;
            pending.CompletedAt = DateTime.UtcNow;
        }
    }
}

public class PendingPresentation
{
    public string State { get; set; } = string.Empty;
    public string Nonce { get; set; } = string.Empty;
    public PresentationDefinition Definition { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public PresentationVerificationResult? Result { get; set; }
}
