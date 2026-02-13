using System.Text.Json.Serialization;

namespace ImmCheck.Core.SSI.OID4VC;

/// <summary>
/// OID4VP Presentation Definition.
/// See: https://openid.net/specs/openid-4-verifiable-presentations-1_0.html
/// Based on DIF Presentation Exchange v2.0.
/// </summary>
public class PresentationDefinition
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("purpose")]
    public string? Purpose { get; set; }

    [JsonPropertyName("input_descriptors")]
    public List<InputDescriptor> InputDescriptors { get; set; } = new();
}

public class InputDescriptor
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("purpose")]
    public string? Purpose { get; set; }

    [JsonPropertyName("format")]
    public Dictionary<string, FormatRequirement>? Format { get; set; }

    [JsonPropertyName("constraints")]
    public Constraints Constraints { get; set; } = new();
}

public class FormatRequirement
{
    [JsonPropertyName("alg")]
    public List<string>? Alg { get; set; }
}

public class Constraints
{
    [JsonPropertyName("fields")]
    public List<FieldConstraint> Fields { get; set; } = new();
}

public class FieldConstraint
{
    [JsonPropertyName("path")]
    public List<string> Path { get; set; } = new();

    [JsonPropertyName("filter")]
    public FilterConstraint? Filter { get; set; }
}

public class FilterConstraint
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "string";

    [JsonPropertyName("const")]
    public string? Const { get; set; }

    [JsonPropertyName("pattern")]
    public string? Pattern { get; set; }
}

/// <summary>
/// Authorization request for OID4VP (sent to wallet).
/// </summary>
public class PresentationRequest
{
    [JsonPropertyName("response_type")]
    public string ResponseType { get; set; } = "vp_token";

    [JsonPropertyName("presentation_definition")]
    public PresentationDefinition PresentationDefinition { get; set; } = new();

    [JsonPropertyName("nonce")]
    public string Nonce { get; set; } = string.Empty;

    [JsonPropertyName("response_uri")]
    public string? ResponseUri { get; set; }

    [JsonPropertyName("state")]
    public string? State { get; set; }
}

/// <summary>
/// Response from the wallet containing the VP token.
/// </summary>
public class PresentationResponse
{
    [JsonPropertyName("vp_token")]
    public string VpToken { get; set; } = string.Empty;

    [JsonPropertyName("presentation_submission")]
    public PresentationSubmission? PresentationSubmission { get; set; }

    [JsonPropertyName("state")]
    public string? State { get; set; }
}

public class PresentationSubmission
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("definition_id")]
    public string DefinitionId { get; set; } = string.Empty;

    [JsonPropertyName("descriptor_map")]
    public List<DescriptorMap> DescriptorMap { get; set; } = new();
}

public class DescriptorMap
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("format")]
    public string Format { get; set; } = "vc+sd-jwt";

    [JsonPropertyName("path")]
    public string Path { get; set; } = "$";
}

/// <summary>
/// Result of verifying a VP token presentation.
/// </summary>
public class PresentationVerificationResult
{
    public bool IsValid { get; set; }
    public string? Error { get; set; }
    public string? PresentationDefinitionId { get; set; }
    public Dictionary<string, object>? DisclosedClaims { get; set; }
    public string? IssuerDid { get; set; }
    public string? SubjectDid { get; set; }
}
