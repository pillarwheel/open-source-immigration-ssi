namespace ImmCheck.Core.SSI;

/// <summary>
/// Manages DID creation and lifecycle operations.
/// </summary>
public interface IDidManager
{
    /// <summary>
    /// Creates a new DID of the specified method.
    /// </summary>
    /// <param name="method">The DID method to use (e.g., "key", "web", "prism")</param>
    /// <param name="options">Method-specific creation options</param>
    /// <returns>The created DID Document.</returns>
    Task<DidDocument> CreateDidAsync(string method, DidCreationOptions? options = null);
}

public class DidCreationOptions
{
    /// <summary>For did:web — the domain to host the DID document.</summary>
    public string? Domain { get; set; }

    /// <summary>For did:web — optional path component.</summary>
    public string? Path { get; set; }

    /// <summary>For did:prism — the Identus Cloud Agent API URL.</summary>
    public string? AgentApiUrl { get; set; }

    /// <summary>For did:prism — optional API key for the Cloud Agent.</summary>
    public string? ApiKey { get; set; }
}
