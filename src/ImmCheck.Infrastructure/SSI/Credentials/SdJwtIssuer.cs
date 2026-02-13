using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using ImmCheck.Core.SSI.Credentials;
using Microsoft.Extensions.Logging;
using NSec.Cryptography;

namespace ImmCheck.Infrastructure.SSI.Credentials;

/// <summary>
/// Issues W3C Verifiable Credentials in SD-JWT (Selective Disclosure JWT) format.
/// Supports Ed25519 (EdDSA) signing by default with HS256 backward compatibility.
/// See: https://www.ietf.org/archive/id/draft-ietf-oauth-selective-disclosure-jwt-13.html
/// </summary>
public class SdJwtIssuer : ICredentialIssuer
{
    private readonly IKeyStore _keyStore;
    private readonly ICredentialRepository _credentialRepo;
    private readonly ILogger<SdJwtIssuer> _logger;

    public SdJwtIssuer(
        IKeyStore keyStore,
        ICredentialRepository credentialRepo,
        ILogger<SdJwtIssuer> logger)
    {
        _keyStore = keyStore;
        _credentialRepo = credentialRepo;
        _logger = logger;
    }

    public async Task<IssuedCredential> IssueAsync(CredentialIssuanceRequest request)
    {
        var credentialId = $"urn:uuid:{Guid.NewGuid()}";
        var statusIndex = await _credentialRepo.AllocateStatusListIndexAsync(request.IssuerDid);

        // Build the VC
        var vc = new VerifiableCredential
        {
            Context = new List<string> { "https://www.w3.org/ns/credentials/v2" },
            Id = credentialId,
            Type = new List<string> { "VerifiableCredential", request.CredentialType },
            Issuer = request.IssuerDid,
            ValidFrom = DateTime.UtcNow,
            ValidUntil = request.ValidityDays.HasValue
                ? DateTime.UtcNow.AddDays(request.ValidityDays.Value)
                : null,
            CredentialSubject = new Dictionary<string, object>(request.Claims)
            {
                ["id"] = request.SubjectDid
            },
            CredentialStatus = new CredentialStatus
            {
                Id = $"{request.IssuerDid}#status-{statusIndex}",
                Type = "BitstringStatusListEntry",
                StatusPurpose = "revocation",
                StatusListIndex = statusIndex,
                StatusListCredential = $"/api/credentials/status-list/{Uri.EscapeDataString(request.IssuerDid)}"
            }
        };

        // Determine which claims get selective disclosure
        var sdClaims = request.SelectiveDisclosureClaims ?? new List<string>();
        var schema = CredentialSchemas.GetSchema(request.CredentialType);
        if (!sdClaims.Any() && schema != null)
        {
            sdClaims = schema.DefaultSelectiveDisclosure.ToList();
        }

        // Get algorithm for this issuer
        var algorithm = await _keyStore.GetAlgorithmAsync(request.IssuerDid);

        // Build SD-JWT
        var (jwt, disclosures) = BuildSdJwt(vc, sdClaims, request.IssuerDid, algorithm);

        // Compose the SD-JWT compact serialization
        var disclosureStr = string.Join("~", disclosures);
        var serialized = disclosures.Any() ? $"{jwt}~{disclosureStr}~" : jwt;

        // Store the issued credential
        var record = new IssuedCredentialRecord
        {
            Id = credentialId.Replace("urn:uuid:", ""),
            IssuerDid = request.IssuerDid,
            SubjectDid = request.SubjectDid,
            CredentialType = request.CredentialType,
            SerializedCredential = serialized,
            IssuedAt = vc.ValidFrom,
            ExpiresAt = vc.ValidUntil,
            StatusListIndex = statusIndex
        };
        await _credentialRepo.StoreAsync(record);

        _logger.LogInformation("Issued {Type} credential {Id} from {Issuer} to {Subject} (alg={Alg})",
            request.CredentialType, credentialId, request.IssuerDid, request.SubjectDid, algorithm);

        return new IssuedCredential
        {
            CredentialId = record.Id,
            SerializedCredential = serialized,
            Format = "vc+sd-jwt",
            Credential = vc
        };
    }

    public async Task<CredentialVerificationResult> VerifyAsync(string serializedCredential)
    {
        try
        {
            var parts = serializedCredential.Split('~');
            var jwt = parts[0];
            var disclosures = parts.Skip(1).Where(d => !string.IsNullOrEmpty(d)).ToList();

            // Decode JWT (header.payload.signature)
            var jwtParts = jwt.Split('.');
            if (jwtParts.Length != 3)
                return new CredentialVerificationResult { IsValid = false, Error = "Invalid JWT format" };

            var headerJson = Base64UrlDecode(jwtParts[0]);
            var payloadJson = Base64UrlDecode(jwtParts[1]);

            var header = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(headerJson);
            var payload = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(payloadJson);

            if (payload == null)
                return new CredentialVerificationResult { IsValid = false, Error = "Cannot parse JWT payload" };

            // Determine algorithm from header
            var alg = header?.TryGetValue("alg", out var algElem) == true ? algElem.GetString() : "HS256";

            // Verify signature
            var issuerDid = payload.TryGetValue("iss", out var issElem) ? issElem.GetString() : null;
            if (issuerDid == null)
                return new CredentialVerificationResult { IsValid = false, Error = "Missing issuer" };

            var verifyKey = await _keyStore.GetPublicKeyAsync(issuerDid);
            if (verifyKey == null)
                return new CredentialVerificationResult { IsValid = false, Error = "Unknown issuer key" };

            var dataToVerify = Encoding.UTF8.GetBytes($"{jwtParts[0]}.{jwtParts[1]}");
            var signature = Base64UrlDecodeBytes(jwtParts[2]);

            bool signatureValid;
            if (alg == "EdDSA")
            {
                signatureValid = VerifyEd25519(verifyKey, dataToVerify, signature);
            }
            else
            {
                // HS256 backward compatibility
                using var hmac = new HMACSHA256(verifyKey);
                var expectedSig = hmac.ComputeHash(dataToVerify);
                signatureValid = CryptographicOperations.FixedTimeEquals(signature, expectedSig);
            }

            if (!signatureValid)
                return new CredentialVerificationResult { IsValid = false, Error = "Invalid signature" };

            // Process disclosures
            var disclosedClaims = new Dictionary<string, object>();
            foreach (var disclosure in disclosures)
            {
                var disclosureJson = Base64UrlDecode(disclosure);
                var disclosureArray = JsonSerializer.Deserialize<JsonElement[]>(disclosureJson);
                if (disclosureArray is { Length: 3 })
                {
                    var claimName = disclosureArray[1].GetString() ?? "";
                    disclosedClaims[claimName] = disclosureArray[2];
                }
            }

            // Add non-SD claims from payload
            if (payload.TryGetValue("vc", out var vcElem))
            {
                var vcObj = vcElem.Deserialize<Dictionary<string, JsonElement>>();
                if (vcObj?.TryGetValue("credentialSubject", out var subjectElem) == true)
                {
                    var subject = subjectElem.Deserialize<Dictionary<string, JsonElement>>();
                    if (subject != null)
                    {
                        foreach (var (key, value) in subject)
                        {
                            if (key != "_sd" && !disclosedClaims.ContainsKey(key))
                                disclosedClaims[key] = value;
                        }
                    }
                }
            }

            var subjectDid = disclosedClaims.TryGetValue("id", out var subId) ? subId.ToString() : null;

            DateTime? validFrom = payload.TryGetValue("iat", out var iatElem)
                ? DateTimeOffset.FromUnixTimeSeconds(iatElem.GetInt64()).UtcDateTime
                : null;
            DateTime? validUntil = payload.TryGetValue("exp", out var expElem)
                ? DateTimeOffset.FromUnixTimeSeconds(expElem.GetInt64()).UtcDateTime
                : null;

            // Check expiry
            if (validUntil.HasValue && validUntil.Value < DateTime.UtcNow)
                return new CredentialVerificationResult
                {
                    IsValid = false,
                    Error = "Credential expired",
                    IssuerDid = issuerDid,
                    SubjectDid = subjectDid
                };

            return new CredentialVerificationResult
            {
                IsValid = true,
                IssuerDid = issuerDid,
                SubjectDid = subjectDid,
                DisclosedClaims = disclosedClaims,
                ValidFrom = validFrom,
                ValidUntil = validUntil
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Credential verification failed");
            return new CredentialVerificationResult { IsValid = false, Error = ex.Message };
        }
    }

    private (string jwt, List<string> disclosures) BuildSdJwt(
        VerifiableCredential vc,
        List<string> sdClaimNames,
        string issuerDid,
        string algorithm)
    {
        var disclosures = new List<string>();
        var sdHashes = new List<string>();
        var visibleClaims = new Dictionary<string, object>();

        // Separate claims into visible and selectively disclosable
        foreach (var (key, value) in vc.CredentialSubject)
        {
            if (sdClaimNames.Contains(key))
            {
                // Create disclosure: [salt, claim_name, claim_value]
                var salt = Convert.ToBase64String(RandomNumberGenerator.GetBytes(16));
                var disclosureArray = new object[] { salt, key, value };
                var disclosureJson = JsonSerializer.Serialize(disclosureArray);
                var disclosureB64 = Base64UrlEncode(disclosureJson);
                disclosures.Add(disclosureB64);

                // Hash the disclosure for the _sd array
                var hash = SHA256.HashData(Encoding.ASCII.GetBytes(disclosureB64));
                sdHashes.Add(Base64UrlEncodeBytes(hash));
            }
            else
            {
                visibleClaims[key] = value;
            }
        }

        // Add _sd array if there are selective disclosures
        if (sdHashes.Any())
        {
            visibleClaims["_sd"] = sdHashes;
        }

        // Build JWT payload
        var payload = new Dictionary<string, object>
        {
            ["iss"] = issuerDid,
            ["iat"] = new DateTimeOffset(vc.ValidFrom).ToUnixTimeSeconds(),
            ["jti"] = vc.Id ?? "",
            ["vc"] = new Dictionary<string, object>
            {
                ["@context"] = vc.Context,
                ["type"] = vc.Type,
                ["credentialSubject"] = visibleClaims,
                ["credentialStatus"] = vc.CredentialStatus != null
                    ? new Dictionary<string, object>
                    {
                        ["id"] = vc.CredentialStatus.Id,
                        ["type"] = vc.CredentialStatus.Type,
                        ["statusPurpose"] = vc.CredentialStatus.StatusPurpose,
                        ["statusListIndex"] = vc.CredentialStatus.StatusListIndex,
                        ["statusListCredential"] = vc.CredentialStatus.StatusListCredential
                    }
                    : new Dictionary<string, object>()
            }
        };

        if (vc.ValidUntil.HasValue)
        {
            payload["exp"] = new DateTimeOffset(vc.ValidUntil.Value).ToUnixTimeSeconds();
        }

        // Build JWT header with algorithm
        var header = new Dictionary<string, object>
        {
            ["alg"] = algorithm,
            ["typ"] = "vc+sd-jwt"
        };

        // Sign
        var headerB64 = Base64UrlEncode(JsonSerializer.Serialize(header));
        var payloadB64 = Base64UrlEncode(JsonSerializer.Serialize(payload));
        var dataToSign = Encoding.UTF8.GetBytes($"{headerB64}.{payloadB64}");

        var signingKey = _keyStore.GetSigningKeySync(issuerDid);
        string signatureB64;

        if (algorithm == "EdDSA")
        {
            signatureB64 = SignEd25519(signingKey, dataToSign);
        }
        else
        {
            // HS256 backward compatibility
            using var hmac = new HMACSHA256(signingKey);
            var signature = hmac.ComputeHash(dataToSign);
            signatureB64 = Base64UrlEncodeBytes(signature);
        }

        var jwt = $"{headerB64}.{payloadB64}.{signatureB64}";
        return (jwt, disclosures);
    }

    private static string SignEd25519(byte[] privateKeyBytes, byte[] data)
    {
        using var key = Key.Import(SignatureAlgorithm.Ed25519, privateKeyBytes, KeyBlobFormat.RawPrivateKey,
            new KeyCreationParameters { ExportPolicy = KeyExportPolicies.AllowPlaintextExport });
        var signature = SignatureAlgorithm.Ed25519.Sign(key, data);
        return Base64UrlEncodeBytes(signature);
    }

    private static bool VerifyEd25519(byte[] publicKeyBytes, byte[] data, byte[] signature)
    {
        try
        {
            var publicKey = PublicKey.Import(SignatureAlgorithm.Ed25519, publicKeyBytes, KeyBlobFormat.RawPublicKey);
            return SignatureAlgorithm.Ed25519.Verify(publicKey, data, signature);
        }
        catch
        {
            return false;
        }
    }

    // Base64url helpers
    private static string Base64UrlEncode(string input) =>
        Base64UrlEncodeBytes(Encoding.UTF8.GetBytes(input));

    private static string Base64UrlEncodeBytes(byte[] input) =>
        Convert.ToBase64String(input).TrimEnd('=').Replace('+', '-').Replace('/', '_');

    private static string Base64UrlDecode(string input)
    {
        var bytes = Base64UrlDecodeBytes(input);
        return Encoding.UTF8.GetString(bytes);
    }

    private static byte[] Base64UrlDecodeBytes(string input)
    {
        var padded = input.Replace('-', '+').Replace('_', '/');
        switch (padded.Length % 4)
        {
            case 2: padded += "=="; break;
            case 3: padded += "="; break;
        }
        return Convert.FromBase64String(padded);
    }
}
