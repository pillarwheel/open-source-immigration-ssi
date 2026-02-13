# Cardano Catalyst Fund 8 — Deliverables Report

**Project:** Open Source Immigration SSI
**Fund:** Catalyst Fund 8
**Budget:** $1,800
**Repository:** https://github.com/nicholasgasior/open-source-immigration-ssi

---

## Executive Summary

This project delivers an open-source system for managing international student immigration documents using Self-Sovereign Identity (SSI) and Verifiable Credentials, with Cardano blockchain integration via `did:prism` (Hyperledger Identus, formerly Atala PRISM — IOG contributed the Atala PRISM codebase to the Hyperledger Foundation in 2023; promoted to full project status as Hyperledger Identus in April 2024).

The system enables U.S. university Designated School Officials (DSOs) to issue W3C Verifiable Credentials for immigration documents (I-20, financial support), supports SD-JWT selective disclosure (students prove enrollment without revealing SEVIS IDs), and implements OID4VC protocols for standards-compliant credential exchange.

---

## Deliverables

### 1. Immigration Document Management System

**Status: Complete**

Full CRUD REST API for 7 immigration document types:
- I-20 (Certificate of Eligibility for F-1 status)
- DS-2019 (Certificate of Eligibility for J-1 status)
- I-94 (Arrival/Departure Record)
- Passport records
- Visa information
- Financial Support documentation
- Sponsored Student records

Technology: ASP.NET Core 8.0, EF Core with SQLite, Swagger API documentation.

### 2. Decentralized Identifier (DID) Infrastructure

**Status: Complete**

Multi-method DID resolution and creation:
- **did:key** — Local Ed25519 key generation and resolution
- **did:web** — HTTPS-based DID Document resolution
- **did:prism** — Cardano blockchain integration via Hyperledger Identus Cloud Agent
- **did:midnight** — Midnight blockchain (ZK-SNARKs privacy chain) DID resolution
- **DIF Universal Resolver** — Fallback for additional DID methods

API endpoints: `POST /api/did/create`, `GET /api/did/resolve/{did}`, `GET /api/did/methods`

### 3. Cardano Blockchain Integration (Catalyst Milestone)

**Status: Complete**

Integration with Cardano's `preprod` testnet through Hyperledger Identus (formerly Atala PRISM):
- Docker-compose configuration with Identus Cloud Agent + PostgreSQL
- `DidPrismResolver` class communicates with Identus REST API
- DID creation anchors identifier to Cardano blockchain
- DID resolution retrieves on-chain DID Documents
- Configurable Cloud Agent URL and API key

**How to verify:**
```bash
docker-compose up          # Starts Identus + Cardano preprod connection
curl -X POST localhost:5000/api/did/create -d '{"method":"prism"}'
# Returns did:prism:... anchored to Cardano preprod
```

### 4. W3C Verifiable Credentials with SD-JWT

**Status: Complete**

Full SD-JWT (Selective Disclosure JWT) credential system:
- **Issuance:** DSOs issue SD-JWT Verifiable Credentials for I-20 and Financial Support documents
- **Selective Disclosure:** Claims like SEVIS ID, education level, and financial amounts are hidden by default; students choose which claims to reveal during presentation
- **Verification:** Cryptographic verification of JWT signatures and disclosure hashes
- **Revocation:** Bitstring Status List v1.1 — efficient revocation with per-issuer status lists

Credential schemas (6 document types):
- `I20Credential`: 8 required claims, 8 optional, 5 selectively disclosable
- `FinancialSupportCredential`: 5 required claims, 12 optional, 8 selectively disclosable
- `PassportCredential`: 7 required claims, 7 optional, 4 selectively disclosable (ICAO 9303 MRZ mapping)
- `VisaCredential`: 5 required claims, 6 optional, 3 selectively disclosable
- `DS2019Credential`: 7 required claims, 5 optional, 4 selectively disclosable
- `I94Credential`: 5 required claims, 3 optional, 2 selectively disclosable

### 5. OID4VC Protocol Implementation

**Status: Complete**

Standards-compliant credential exchange protocols:

**OID4VCI (OpenID for Verifiable Credential Issuance):**
- Issuer metadata endpoint with credential configurations
- Pre-authorized code flow (credential offer → token exchange → credential issuance)
- Bearer token authentication for credential endpoint

**OID4VP (OpenID for Verifiable Presentations):**
- Presentation request creation with nonce/state tracking
- Presentation submission and verification
- Predefined verification scenarios: "Prove F-1 status", "Prove financial support", "Verify passport identity", "Prove J-1 status", "Verify admission status"

### 6. Authentication & Role-Based Access

**Status: Complete**

- ASP.NET Core Identity with JWT Bearer authentication
- Four roles: Student, DSO (Designated School Official), Advisor, Admin
- Registration and login endpoints

### 7. Angular Frontend

**Status: Complete**

Angular 19 single-page application with:
- Credential dashboard (view issued credentials, revoke)
- Credential issuance form (select student, document type, issue VC)
- Credential verification interface (paste SD-JWT, see verified claims)
- OID4VP scenario selection (predefined verification flows)
- Immigration document display components

### 8. Docker Deployment

**Status: Complete**

Single-command deployment via `docker-compose up`:
- .NET API (port 5000)
- Angular frontend (port 4200)
- Hyperledger Identus Cloud Agent (port 8085) connected to Cardano preprod
- PostgreSQL for Identus (port 5432)

### 9. Test Suite

**Status: Complete**

119 automated tests:
- 10 document endpoint integration tests
- 14 DID resolution/publication integration tests (including did:midnight)
- 16 credential issuance/verification integration tests (all 6 types)
- 10 OID4VCI/OID4VP flow integration tests (5 scenarios)
- 12 DID resolver + Base58 unit tests
- 32 SD-JWT + credential schema unit tests (all 6 types with selective disclosure)
- 10 Ed25519 signing tests
- 6 did:cheqd resolver tests
- 6 did:midnight resolver tests
- 3 additional utility tests

---

## Key Technical Achievements

### Selective Disclosure for Immigration Privacy
Students can prove they are enrolled at a university in valid F-1 status without revealing their SEVIS ID number, specific program dates, or financial details. This addresses a real privacy concern in immigration document verification.

### Standards Compliance
The system implements multiple W3C and IETF specifications:
- W3C Verifiable Credentials Data Model 2.0
- W3C DID Core v1.0
- IETF SD-JWT (draft)
- W3C Bitstring Status List v1.1
- OpenID for Verifiable Credential Issuance (OID4VCI)
- OpenID for Verifiable Presentations (OID4VP)

### Cardano Integration
DID operations are anchored to Cardano's preprod testnet via Hyperledger Identus (formerly Atala PRISM — IOG contributed to Hyperledger Foundation 2023, promoted to full project status April 2024), fulfilling the Catalyst Fund 8 obligation to demonstrate Cardano utility in immigration document management.

### Ed25519 Asymmetric Signing (Sprint 8)
Credentials now use Ed25519 (EdDSA) asymmetric key pairs by default, replacing the prototype HMAC-SHA256 symmetric signing. This means any third party with the issuer's public key (from their DID document) can verify a credential without needing the issuer's secret key — a prerequisite for blockchain-anchored DIDs. Backward compatibility is maintained: existing HS256-signed credentials continue to verify.

### Cardano DID Publication (Sprint 8)
Full DID lifecycle on Cardano: create → publish → resolve. The `POST /api/did/publish/{did}` endpoint triggers publication of a did:prism DID to Cardano's preprod testnet via the Identus Cloud Agent. `GET /api/did/status/{did}` tracks publication status (CREATED → PUBLICATION_PENDING → PUBLISHED).

### Multi-Blockchain Support (Sprint 8)
Added `did:cheqd` resolver for the cheqd Cosmos-based blockchain, demonstrating multi-blockchain DID resolution. Added DIF Universal Resolver container for fallback resolution of any DID method.

### Full Immigration Document Credential Coverage (Sprint 9)
Expanded credential schemas from 2 to 6, covering all immigration document types eligible for verifiable credentials: I-20, Financial Support, Passport, Visa, DS-2019, and I-94. Each schema defines required claims, optional claims, and per-document selective disclosure defaults.

Key privacy feature: PassportCredential maps ICAO 9303 MRZ fields and enables proving nationality without revealing the passport document number, date of birth, or MRZ data.

### Midnight Blockchain Integration (Sprint 9)
Pre-built `did:midnight` resolver for IOG's privacy-focused Cardano partner chain. Midnight uses ZK-SNARKs for zero-knowledge identity proofs — enabling stronger privacy guarantees than SD-JWT selective disclosure alone (prove boolean facts about claims without revealing the claims themselves). The resolver is configured for Midnight's testnet and will be updated to mainnet when it launches (late March 2026).

### Expanded OID4VP Verification Scenarios (Sprint 9)
Added 3 new OID4VP presentation scenarios (total: 5): passport identity verification, J-1 exchange visitor status, and I-94 admission status. Updated format requirements to include `EdDSA` alongside `HS256` to reflect Sprint 8's Ed25519 upgrade.

---

## Regulatory Alignment

This project aligns with active U.S. federal government initiatives in digital immigration credentials:

- **DHS SVIP:** The DHS Science & Technology Directorate's Silicon Valley Innovation Program has funded 60+ companies since 2018 to build blockchain/VC solutions for immigration. ImmCheck implements the same W3C VC 2.0 and DID Core standards being funded commercially.
- **USCIS Standards Adoption:** USCIS accepts electronic signatures (July 2023) and has proposed electronic document matching rules (November 2025). The W3C VC format used by ImmCheck is the standard USCIS and CBP are pursuing.
- **NIST SP 800-63-4:** The July 2025 digital identity guidelines include digital wallets in the federal identity assurance framework, providing a federal basis for VC-based identity verification.
- **W3C VC 2.0:** Reached W3C Recommendation status in May 2025, moving from draft to formal standard during this project's development.
- **FERPA Compliance:** The off-chain privacy architecture (no PII on blockchain) aligns with FERPA requirements for student data protection.
- **eIDAS 2.0:** EU digital identity wallets become mandatory in December 2026, and W3C VC interoperability ensures this system can work with European digital identity infrastructure.

See [docs/compliance.md](compliance.md) for comprehensive regulatory analysis.

---

## How to Use This Project

Universities and developers can:
1. Clone the repository and run `docker-compose up` for a complete working demo
2. Use the credential schemas as templates for their own immigration document VCs
3. Extend with additional DID methods or credential types
4. Connect to Cardano mainnet by changing the Identus Cloud Agent configuration
5. Integrate with existing Student Information Systems (SIS) via the REST API

---

## Future Work

- **Wallet integration:** Connect with OWF (OpenWallet Foundation) compatible mobile wallets
- **Additional credential types:** EAD (Employment Authorization), OPT/CPT documentation
- **Cardano mainnet:** Deploy Identus Cloud Agent against Cardano mainnet (currently preprod)
- **Multi-institution federation:** Cross-university credential verification for transfer students
- **FERPA compliance review:** Formal assessment of data privacy requirements
- **HSM/KMS integration:** Hardware security module support for production key management
