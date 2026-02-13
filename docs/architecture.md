# Architecture Decision Records

## Overview

ImmCheck follows a **Clean Architecture** pattern with four projects:

```
ImmCheck.Core           (innermost — no dependencies)
    ^
ImmCheck.Infrastructure (depends on Core)
    ^
ImmCheck.Api            (depends on Core + Infrastructure)
    ^
ImmCheck.Api.Tests      (depends on Api)
```

The Angular frontend (`imm-check-app-example/`) communicates with the API exclusively via HTTP.

---

## ADR-1: Clean Architecture over Monolithic Controller

**Context:** The original codebase had a single `DatabaseController.cs` with raw SQL queries against SQLite, all 7 entity types in one file, and no separation of concerns.

**Decision:** Restructure into Clean Architecture with Core (domain), Infrastructure (data access, external integrations), and Api (HTTP layer) projects.

**Rationale:**
- SSI features (DID resolution, credential issuance, OID4VC protocols) require well-defined interfaces that can be swapped (e.g., different DID methods, different credential formats)
- Testability — Core interfaces can be mocked; Infrastructure uses InMemory EF Core for testing
- Each project has a focused responsibility and minimal coupling

**Consequences:** More files and projects, but each is small and focused. New features plug in via interface implementations registered in DI.

---

## ADR-2: SD-JWT Custom Implementation over Library

**Context:** At the time of implementation, .NET SD-JWT libraries from the OpenWallet Foundation were either pre-release or had heavy dependency chains. The SD-JWT spec itself is relatively simple.

**Decision:** Implement SD-JWT issuance and verification directly in `SdJwtIssuer.cs`.

**Implementation:**
- JWT header/payload encoded as base64url with `{"alg":"HS256","typ":"vc+sd-jwt"}` header
- Selective disclosure claims removed from payload, replaced with `_sd` array of SHA-256 hashes
- Each disclosure is a base64url-encoded JSON array: `[salt, claim_name, claim_value]`
- Disclosures appended to JWT separated by `~` characters
- Verification: decode JWT, verify HMAC signature, hash each disclosure and match against `_sd` array

**Signing:** Uses HMAC-SHA256 with per-issuer symmetric keys stored in SQLite. This is suitable for the demo/prototype context. Production deployments should use Ed25519 or ES256 asymmetric keys.

**Consequences:** Full control over the SD-JWT format. Easy to debug and extend. Must be validated against the IETF SD-JWT specification for production use.

---

## ADR-3: DID Method Architecture

**Context:** The system needs to support multiple DID methods: did:key (simple, local), did:web (HTTP-based), did:prism (Cardano blockchain).

**Decision:** Define `IDidResolver` and `IDidManager` interfaces in Core. Each DID method is a separate class in Infrastructure. `UniversalDidResolver` aggregates all resolvers and falls back to the DIF Universal Resolver HTTP API.

**Interface design:**
```csharp
public interface IDidResolver
{
    string Method { get; }          // "key", "web", "prism"
    bool CanResolve(string did);
    Task<DidDocument?> ResolveAsync(string did);
}

public interface IDidManager
{
    string Method { get; }
    Task<DidDocument> CreateDidAsync(DidCreationOptions options);
}
```

**Implementations:**
| Class | Method | Notes |
|-------|--------|-------|
| `DidKeyResolver` | did:key | Local resolution, Ed25519/X25519, includes Base58 codec |
| `DidWebResolver` | did:web | Fetches `did.json` from HTTPS endpoints per spec |
| `DidPrismResolver` | did:prism | REST client for Hyperledger Identus Cloud Agent |
| `DidKeyManager` | did:key | Generates Ed25519 key pairs, returns did:key identifiers |
| `UniversalDidResolver` | * | Aggregates resolvers, DIF Universal Resolver fallback |

**Consequences:** Adding a new DID method requires implementing one class and registering it in DI. No changes to controllers or domain logic.

---

## ADR-4: OID4VC Protocol Implementation

**Context:** OID4VCI and OID4VP are the emerging standards for credential exchange, replacing proprietary wallet protocols.

**Decision:** Implement OID4VCI (pre-authorized code flow) and OID4VP (presentation exchange) as REST controllers backed by in-memory stores.

**OID4VCI Flow:**
1. `POST /api/oid4vci/credential-offer` — Creates offer with pre-authorized code
2. `POST /api/oid4vci/token` — Exchanges pre-auth code for Bearer token (form-encoded)
3. `POST /api/oid4vci/credential` — Issues SD-JWT VC using Bearer token

**OID4VP Flow:**
1. `POST /api/oid4vp/request` — Creates presentation request with nonce/state
2. `POST /api/oid4vp/response` — Receives VP token, verifies against definition
3. `GET /api/oid4vp/status/{state}` — Poll for verification result

**In-memory stores:** `OfferStore` and `PresentationStore` use `ConcurrentDictionary` for session state. Suitable for single-instance demos. Production would use Redis or a database.

**Predefined scenarios:** Two built-in presentation definitions:
- `f1-status` — Requires I20Credential with sevisId, studentName, programStatus, institutionName
- `financial-support` — Requires FinancialSupportCredential with studentName, totalExpenses, totalFunding

---

## ADR-5: Credential Revocation via Bitstring Status List

**Context:** Revocation is essential for immigration credentials (e.g., when a student's program ends or they transfer institutions).

**Decision:** Implement W3C Bitstring Status List v1.1. Each issuer gets a status list (16,384 entries). Each credential gets a status list index. Revoking a credential sets the corresponding bit.

**Implementation:**
- `StatusListRecord` stores the base64-encoded bitstring per issuer DID
- `IssuedCredentialRecord.StatusListIndex` tracks each credential's position
- `CredentialRepository.RevokeAsync()` sets the bit and updates the record
- `GET /api/credential/status-list/{issuerDid}` serves the encoded list for external verifiers

**Consequences:** Efficient O(1) revocation check. Status list can be published as a static resource. Scales to 16K credentials per issuer (configurable).

---

## ADR-6: SQLite for All Persistence

**Context:** The original project used SQLite with raw SQL. The system needs to store identity data, immigration documents, signing keys, credential records, and status lists.

**Decision:** Continue using SQLite via EF Core for all persistence, including ASP.NET Core Identity tables.

**Tables:**
- Identity: AspNetUsers, AspNetRoles, AspNetUserRoles, etc. (6 tables)
- Documents: i20info, ds2019info, i94info, passportinfo, visainfo, sponsoredstudents, financial_support (7 tables)
- Institutions (1 table)
- SSI: IssuedCredentials, StatusLists, SigningKeys (3 tables)

**Migration strategy:** `InitialCreate` migration uses `CREATE TABLE IF NOT EXISTS` for the 7 pre-existing document tables (they existed before EF Core was added) and standard `CreateTable` for new tables. This handles both fresh databases and upgrades from the original raw-SQL schema.

---

## ADR-7: Testing Strategy

**Context:** The system has complex flows (OID4VCI multi-step, SD-JWT crypto, DID resolution) that need automated verification.

**Decision:** Two categories of tests:

1. **Integration tests** (37 tests) — Use `WebApplicationFactory` with InMemory EF Core to test full HTTP request/response cycles through controllers
2. **Unit tests** (31 tests) — Test infrastructure components directly: DID resolution, SD-JWT issuance/verification, Base58 codec, credential schemas, key store, revocation

**Test isolation:** Each test class/instance gets a unique InMemory database (`"TestDb_" + Guid.NewGuid()`) to prevent cross-test contamination.

**Coverage focus areas:**
- SD-JWT: issuance, verification, selective disclosure, tampered signatures, expiry
- DID: did:key resolution (Ed25519), did:web URL conversion, invalid input handling
- Revocation: flag toggling, idempotency, bitstring status list updates
- OID4VCI: full pre-authorized code flow end-to-end
- OID4VP: full presentation exchange with issued credentials
- Ed25519: signing, verification, backward compat with HS256, key pair generation
- did:cheqd: resolution with mocked HTTP, error handling

---

## ADR-8: Ed25519 Asymmetric Signing (Sprint 8)

**Context:** The original SD-JWT implementation used HMAC-SHA256 (symmetric signing). This means the verifier needs the same secret key as the issuer to verify a credential — defeating the purpose of blockchain-anchored DIDs where any third party should verify with just the public key from the DID document.

**Decision:** Upgrade to Ed25519 (EdDSA) asymmetric signing via [NSec.Cryptography](https://nsec.rocks/) (MIT license, wraps libsodium). New issuers automatically get Ed25519 key pairs. Existing HS256 keys continue to work for backward compatibility.

**Implementation:**
- `SqliteKeyStore`: generates Ed25519 key pairs (32-byte private + 32-byte public) via `NSec.Cryptography`
- `SdJwtIssuer`: reads algorithm from `IKeyStore.GetAlgorithmAsync()`, signs with Ed25519 or HMAC accordingly
- JWT header `alg` field: `"EdDSA"` for Ed25519, `"HS256"` for legacy symmetric keys
- `SigningKeyRecord`: new `PublicKeyMaterial` column stores the Ed25519 public key separately
- `DidKeyManager`: replaced broken P-256 ECDSA stub with real Ed25519 key generation via NSec
- Ed25519 signatures are 64 bytes (vs 32 for HMAC-SHA256)

**Backward compatibility:** The JWT header's `alg` field determines the verification algorithm. `GetPublicKeyAsync()` returns the public key for EdDSA or the symmetric key for HS256. Existing credentials signed with HS256 will still verify.

**Consequences:** Third-party verifiers can now verify credentials using only the issuer's public key from their DID document, enabling true decentralized verification. NSec adds ~1MB to the binary (libsodium native dependency).

---

## ADR-9: Multi-Blockchain DID Support (Sprint 8)

**Context:** The system only supported did:key (local), did:web (HTTPS), and did:prism (Cardano). For 2026 immigration compliance and broader interoperability, additional blockchain DID methods are needed.

**Naming note:** The `did:prism` method is implemented via **Hyperledger Identus**, formerly known as **Atala PRISM**. IOG (Input Output Global) originally developed Atala PRISM as Cardano's decentralized identity solution. In 2023, IOG contributed the codebase to the Hyperledger Foundation under the Linux Foundation. The project was promoted from Hyperledger Labs to full project status as **Hyperledger Identus** in April 2024. In October 2024, IOG reduced the dedicated Identus team, with community maintenance continuing under Hyperledger governance. References to "Atala PRISM" in external documentation refer to the same technology now maintained as Hyperledger Identus.

**Decision:** Add `did:cheqd` resolver and DIF Universal Resolver fallback. Complete the Cardano lifecycle with DID publication endpoints.

**New components:**
| Component | Purpose |
|-----------|---------|
| `DidCheqdResolver` | Resolves did:cheqd via cheqd's public REST API |
| `IDidPublisher` | Interface for publishing DIDs to blockchains |
| `DidPrismResolver.PublishAsync()` | Publishes did:prism to Cardano via Identus Agent |
| `DidPrismResolver.GetStatusAsync()` | Checks DID lifecycle status (CREATED → PUBLISHED) |
| `DidController.Publish/GetStatus` | API endpoints for DID publication and status |
| Universal Resolver container | Docker service for resolving any DID method |

**API changes:**
- `POST /api/did/publish/{did}` — Publish a did:prism DID to Cardano
- `GET /api/did/status/{did}` — Get DID publication status
- `GET /api/did/methods` — Now includes `"cheqd"` in resolve list

**Consequences:** The system now supports 4 DID methods natively and any method via the Universal Resolver. Cardano DIDs can be fully created, published, and resolved — completing the Catalyst Fund 8 obligation.

---

## ADR-10: Credential Schema Expansion (Sprint 9)

**Context:** Only 2 of 7 immigration document types had credential schemas (I20, FinancialSupport). The tech specs identify passport MRZ data and visa information as prime candidates for verifiable credentials. Without schemas for all document types, the system cannot fulfill its goal of comprehensive immigration credential coverage.

**Decision:** Add credential schemas for all remaining document types: PassportCredential, VisaCredential, DS2019Credential, I94Credential.

**Design principles:**
- **ICAO 9303 mapping** for PassportCredential — required claims mirror the machine-readable zone (MRZ) fields: holderName, nationality, issuingState, documentNumber, dateOfBirth, expirationDate, sex
- **Per-document selective disclosure** — each schema identifies which claims are sensitive and should be hidden by default:
  - Passport: documentNumber, dateOfBirth, MRZ lines (prove nationality without passport number)
  - Visa: stampNumber, controlNumber (prove visa type without control numbers)
  - DS-2019: sevisId, participantName (prove program status without personal identifiers)
  - I-94: i94Number, holderName (prove admission class without I-94 number)
- **OID4VP scenarios** expanded from 2 to 5 — each new credential type has a corresponding verification scenario
- **Format requirements** updated to `["EdDSA", "HS256"]` (Sprint 8 Ed25519 upgrade reflected in OID4VP)

**Consequences:** All 6 credential-eligible document types now have schemas. The frontend issuance form offers all 6 types. The OID4VP scenarios cover the main immigration verification use cases: F-1 status, J-1 status, financial support, passport identity, and admission status.

---

## ADR-11: Midnight Blockchain Integration (Sprint 9)

**Context:** Midnight is IOG's privacy-focused Cardano partner chain, launching mainnet late March 2026. It uses `did:midnight` identifiers and ZK-SNARKs for privacy-preserving identity proofs. For immigration credentials, this enables proving nationality or enrollment status without revealing any PII — a stronger privacy guarantee than SD-JWT selective disclosure alone.

**Decision:** Pre-build a `DidMidnightResolver` following the exact pattern of `DidCheqdResolver` (DIF DID Resolution API format). Configure with a default testnet URL that can be updated to mainnet when available.

**Implementation:**
- `DidMidnightResolver` implements `IDidResolver` with `Method = "midnight"` and prefix `did:midnight:`
- Calls `{resolverUrl}/1.0/identifiers/{did}` (DIF format) — same as cheqd and universal resolvers
- Config key: `Midnight:ResolverUrl` (default: `https://indexer.testnet.midnight.network`)
- Graceful degradation: returns null on 404 or network error (testnet may not be available)
- Registered in DI alongside other resolvers — appears in `GET /api/did/methods` resolve list

**ZK-proof privacy model (future):** When Midnight mainnet launches, the system could leverage ZK-SNARKs to:
1. Prove nationality from a PassportCredential without revealing the passport number, name, or date of birth
2. Prove enrollment status without revealing any claims — just a boolean "is valid F-1 student"
3. Prove financial sufficiency without revealing amounts — just "meets minimum threshold"

This goes beyond SD-JWT selective disclosure (which still reveals individual claim values) to full zero-knowledge proofs.

**Consequences:** The resolver is ready for Midnight mainnet. When launched, only the config URL needs to change. The ZK-proof integration is a future sprint — Sprint 9 provides the DID infrastructure foundation.
