using System.Security.Cryptography;
using ImmCheck.Core.SSI;

namespace ImmCheck.Infrastructure.SSI;

/// <summary>
/// Resolves did:key identifiers locally by decoding the multibase-encoded public key.
/// Supports Ed25519 (0xed prefix) and X25519 (0xec prefix) key types.
/// See: https://w3c-ccg.github.io/did-method-key/
/// </summary>
public class DidKeyResolver : IDidResolver
{
    public string Method => "key";

    public bool CanResolve(string did) => did.StartsWith("did:key:");

    public Task<DidDocument?> ResolveAsync(string did)
    {
        if (!CanResolve(did))
            return Task.FromResult<DidDocument?>(null);

        var methodSpecificId = did["did:key:".Length..];

        // Decode multibase (z = base58btc)
        if (!methodSpecificId.StartsWith('z'))
            return Task.FromResult<DidDocument?>(null);

        byte[] decoded;
        try
        {
            decoded = Base58.Decode(methodSpecificId[1..]);
        }
        catch
        {
            return Task.FromResult<DidDocument?>(null);
        }

        if (decoded.Length < 2)
            return Task.FromResult<DidDocument?>(null);

        // Read multicodec varint prefix
        var (codec, keyBytes) = ReadMulticodec(decoded);

        var keyType = codec switch
        {
            0xed => "Ed25519VerificationKey2020",
            0xec => "X25519KeyAgreementKey2020",
            _ => null
        };

        if (keyType == null)
            return Task.FromResult<DidDocument?>(null);

        var verificationMethodId = $"{did}#{methodSpecificId}";
        var publicKeyMultibase = methodSpecificId; // z + base58btc encoded

        var doc = new DidDocument
        {
            Context = new List<string>
            {
                "https://www.w3.org/ns/did/v1",
                "https://w3id.org/security/suites/ed2519-2020/v1"
            },
            Id = did,
            VerificationMethod = new List<VerificationMethod>
            {
                new()
                {
                    Id = verificationMethodId,
                    Type = keyType,
                    Controller = did,
                    PublicKeyMultibase = $"z{Base58.Encode(keyBytes)}"
                }
            },
            Authentication = new List<string> { verificationMethodId },
            AssertionMethod = new List<string> { verificationMethodId }
        };

        if (codec == 0xec)
        {
            doc.KeyAgreement = new List<string> { verificationMethodId };
        }

        return Task.FromResult<DidDocument?>(doc);
    }

    private static (int codec, byte[] keyBytes) ReadMulticodec(byte[] data)
    {
        // Simple varint decoding for common DID key codecs
        if (data.Length >= 2 && data[0] == 0xed && data[1] == 0x01)
            return (0xed, data[2..]);
        if (data.Length >= 2 && data[0] == 0xec && data[1] == 0x01)
            return (0xec, data[2..]);
        return (data[0], data[1..]);
    }
}

/// <summary>
/// Minimal Base58 (Bitcoin alphabet) encoder/decoder.
/// </summary>
internal static class Base58
{
    private const string Alphabet = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";

    public static byte[] Decode(string input)
    {
        var result = new byte[input.Length];
        var resultLength = 0;

        foreach (var c in input)
        {
            var carry = Alphabet.IndexOf(c);
            if (carry < 0)
                throw new FormatException($"Invalid Base58 character: {c}");

            for (var i = 0; i < resultLength; i++)
            {
                carry += 58 * result[i];
                result[i] = (byte)(carry & 0xFF);
                carry >>= 8;
            }

            while (carry > 0)
            {
                result[resultLength++] = (byte)(carry & 0xFF);
                carry >>= 8;
            }
        }

        // Count leading '1's (which represent leading zero bytes)
        var leadingZeros = 0;
        foreach (var c in input)
        {
            if (c != '1') break;
            leadingZeros++;
        }

        var decoded = new byte[leadingZeros + resultLength];
        Array.Copy(result, 0, decoded, leadingZeros, resultLength);
        Array.Reverse(decoded, leadingZeros, resultLength);
        return decoded;
    }

    public static string Encode(byte[] data)
    {
        // Count leading zeros
        var leadingZeros = 0;
        foreach (var b in data)
        {
            if (b != 0) break;
            leadingZeros++;
        }

        // Convert to base58
        var temp = new byte[data.Length * 2];
        var tempLength = 0;

        foreach (var b in data)
        {
            var carry = (int)b;
            for (var i = 0; i < tempLength; i++)
            {
                carry += 256 * temp[i];
                temp[i] = (byte)(carry % 58);
                carry /= 58;
            }

            while (carry > 0)
            {
                temp[tempLength++] = (byte)(carry % 58);
                carry /= 58;
            }
        }

        var result = new char[leadingZeros + tempLength];
        for (var i = 0; i < leadingZeros; i++)
            result[i] = '1';

        for (var i = 0; i < tempLength; i++)
            result[leadingZeros + i] = Alphabet[temp[tempLength - 1 - i]];

        return new string(result);
    }
}
