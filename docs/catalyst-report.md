# Cardano Catalyst Fund 9 — Deliverables Report

**Project:** Open Source Immigration SSI
**Fund:** Catalyst Fund 9
**Budget:** $1,800
**Repository:** https://github.com/nicholasgasior/open-source-immigration-ssi

---

## Executive Summary

This project delivers an open-source system for managing international student immigration documents using Self-Sovereign Identity (SSI) and Verifiable Credentials, with Cardano blockchain integration via `did:prism` (Hyperledger Identus, formerly Atala PRISM).

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

Credential schemas:
- `I20Credential`: 8 required claims, 8 optional, 5 selectively disclosable
- `FinancialSupportCredential`: 5 required claims, 12 optional, 8 selectively disclosable

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
- Predefined verification scenarios: "Prove F-1 status", "Prove financial support"

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

88 automated tests:
- 10 document endpoint integration tests
- 12 DID resolution/publication integration tests
- 12 credential issuance/verification integration tests
- 7 OID4VCI/OID4VP flow integration tests
- 12 DID resolver + Base58 unit tests
- 16 SD-JWT + credential schema unit tests
- 10 Ed25519 signing tests
- 6 did:cheqd resolver tests
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
DID operations are anchored to Cardano's preprod testnet via Hyperledger Identus, fulfilling the Catalyst Fund 9 obligation to demonstrate Cardano utility in immigration document management.

### Ed25519 Asymmetric Signing (Sprint 8)
Credentials now use Ed25519 (EdDSA) asymmetric key pairs by default, replacing the prototype HMAC-SHA256 symmetric signing. This means any third party with the issuer's public key (from their DID document) can verify a credential without needing the issuer's secret key — a prerequisite for blockchain-anchored DIDs. Backward compatibility is maintained: existing HS256-signed credentials continue to verify.

### Cardano DID Publication (Sprint 8)
Full DID lifecycle on Cardano: create → publish → resolve. The `POST /api/did/publish/{did}` endpoint triggers publication of a did:prism DID to Cardano's preprod testnet via the Identus Cloud Agent. `GET /api/did/status/{did}` tracks publication status (CREATED → PUBLICATION_PENDING → PUBLISHED).

### Multi-Blockchain Support (Sprint 8)
Added `did:cheqd` resolver for the cheqd Cosmos-based blockchain, demonstrating multi-blockchain DID resolution. Added DIF Universal Resolver container for fallback resolution of any DID method.

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
