namespace ImmCheck.Core.SSI;

/// <summary>
/// Resolves a DID to its DID Document.
/// Each implementation handles a specific DID method (did:web, did:key, did:prism, etc.).
/// </summary>
public interface IDidResolver
{
    /// <summary>The DID method this resolver handles (e.g., "web", "key", "prism").</summary>
    string Method { get; }

    /// <summary>
    /// Resolves the given DID to its DID Document.
    /// </summary>
    /// <param name="did">A fully-qualified DID string (e.g., "did:key:z6Mkf...")</param>
    /// <returns>The resolved DID Document, or null if not found.</returns>
    Task<DidDocument?> ResolveAsync(string did);

    /// <summary>
    /// Returns true if this resolver can handle the given DID.
    /// </summary>
    bool CanResolve(string did);
}
