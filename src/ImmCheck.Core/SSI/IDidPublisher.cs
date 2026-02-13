namespace ImmCheck.Core.SSI;

/// <summary>
/// Publishes DIDs to a blockchain (e.g., Cardano for did:prism).
/// </summary>
public interface IDidPublisher
{
    Task<DidPublicationResult> PublishAsync(string did);
    Task<DidStatusResult> GetStatusAsync(string did);
}

public class DidPublicationResult
{
    public bool Success { get; set; }
    public string Status { get; set; } = string.Empty;  // "publication_pending", "published"
    public string? ScheduledOperation { get; set; }
    public string? Error { get; set; }
}

public class DidStatusResult
{
    public string Did { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;  // "CREATED", "PUBLICATION_PENDING", "PUBLISHED"
    public string? Blockchain { get; set; }  // "cardano"
    public string? Network { get; set; }     // "preprod"
}
