# ImmCheck: Open-Source Immigration Document Management with SSI

Open-source system for managing international student immigration documents using Self-Sovereign Identity (SSI) and Verifiable Credentials. Built for U.S. university immigration advisors (DSOs), students, and compliance workflows.

**Funded by Cardano Catalyst Fund 8** | **W3C Verifiable Credentials** | **SD-JWT Selective Disclosure** | **OID4VC Protocols** | **Cardano did:prism Support**

## Architecture

```
src/
  ImmCheck.Core/              Domain models, interfaces, DTOs
    Models/                   I-20, DS-2019, I-94, Passport, Visa, Financial Support
    Interfaces/               Repository contracts
    DTOs/                     Auth request/response types
    SSI/                      DID Core (DidDocument, IDidResolver, IDidManager)
      Credentials/            VC models, issuer/repository/keystore interfaces, schemas
      OID4VC/                 OID4VCI + OID4VP protocol models

  ImmCheck.Infrastructure/    External integrations, data access
    Data/                     AppDbContext (EF Core + SQLite)
    Repositories/             Generic document repository
    Migrations/               EF Core migrations
    SSI/                      DID resolvers (did:key, did:web, did:prism, universal)
      Credentials/            SD-JWT issuer, SQLite key store, credential repository
      OID4VC/                 In-memory offer/presentation stores

  ImmCheck.Api/               ASP.NET Core host
    Controllers/              Documents, Auth, DID, Credential, OID4VCI, OID4VP
    Middleware/               Global exception handling
    Data/                     Seed data for development

  ImmCheck.Api.Tests/         xUnit test suite (119 tests)
    SSI/                      Unit tests for DID resolution, SD-JWT, Ed25519, Base58

imm-check-app-example/        Angular 19 frontend
  src/app/
    modules/
      credentials/            Credential dashboard, issuance, verification components
      documents/              Immigration document display components
    core/services/            API client services
```

## Features

### Immigration Documents
- **I-20** (Certificate of Eligibility for F-1 status)
- **DS-2019** (Certificate of Eligibility for J-1 status)
- **I-94** (Arrival/Departure Record)
- **Passport** records
- **Visa** information
- **Financial Support** documentation (tuition, living expenses, funding sources)

### Self-Sovereign Identity (SSI)
- **W3C Verifiable Credentials 2.0** for all 6 immigration document types (I-20, Financial Support, Passport, Visa, DS-2019, I-94)
- **SD-JWT** (Selective Disclosure JWT) — prove enrollment status without revealing SEVIS ID
- **Bitstring Status List v1.1** for credential revocation
- **Ed25519 (EdDSA)** asymmetric signing — verifiers only need the public key (no shared secrets)
- **DID Resolution**: did:key (local Ed25519), did:web (HTTPS), did:prism (Cardano/Identus), did:cheqd (Cosmos), did:midnight (Midnight/ZK)
- **DID Publication**: Full Cardano DID lifecycle — create, publish to preprod, track status
- **DIF Universal Resolver** fallback for additional DID methods

### OID4VC Protocols
- **OID4VCI** (OpenID for Verifiable Credential Issuance) — pre-authorized code flow
- **OID4VP** (OpenID for Verifiable Presentations) — presentation exchange with predefined scenarios:
  - Prove valid F-1 status (I-20 credential)
  - Prove financial support (Financial Support credential)
  - Verify passport identity (Passport credential — nationality without document number)
  - Prove J-1 status (DS-2019 credential)
  - Verify admission status (I-94 credential)

### Authentication & Roles
- ASP.NET Core Identity with JWT Bearer authentication
- Roles: `Student`, `DSO` (Designated School Official), `Advisor`, `Admin`

## Quick Start

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 18+](https://nodejs.org/) (for Angular frontend)
- [Docker](https://docs.docker.com/get-docker/) (optional, for Cardano/Identus)

### Run the API

```bash
cd src
dotnet run --project ImmCheck.Api
```

The API starts at `https://localhost:5001` (or `http://localhost:5000`).
Swagger UI available at `/swagger`.

### Run Tests

```bash
cd src
dotnet test ImmCheck.Api.Tests
```

119 tests covering: document endpoints, DID resolution/publication, credential issuance/verification for all 6 credential types, OID4VCI/OID4VP flows (5 scenarios), SD-JWT selective disclosure, Ed25519 signing, did:cheqd resolution, did:midnight resolution, revocation, key management, and schema validation.

### Run the Frontend

```bash
cd imm-check-app-example
npm install
ng serve
```

Angular app available at `http://localhost:4200`.

### Docker (Full Stack)

```bash
docker-compose up
```

Services:
| Service | Port | Description |
|---------|------|-------------|
| api | 5000 | .NET API |
| frontend | 4200 | Angular app |
| identus-cloud-agent | 8085 | Hyperledger Identus (did:prism / Cardano) |
| identus-db | 5432 | PostgreSQL for Identus |
| universal-resolver | 8080 | DIF Universal Resolver (multi-method) |

## Project Background

This project was funded by **Cardano Catalyst Fund 8** ($1,800) to research and prototype blockchain-based immigration document management for U.S. educational institutions. The goal is to provide an open-source system that universities can use and contribute back to, enabling:

1. International students to hold verifiable credentials for their immigration documents
2. DSOs (Designated School Officials) to issue and verify these credentials
3. Selective disclosure — students prove enrollment status without revealing sensitive information like SEVIS IDs
4. Cardano blockchain anchoring for DID persistence and auditability

## Regulatory & Compliance Context

Immigration credential technology operates within a complex regulatory landscape. See [docs/compliance.md](docs/compliance.md) for comprehensive coverage.

### SEVP/SEVIS & Current Verification Gaps

The Student and Exchange Visitor Program (SEVP) manages over 1.1 million active F, J, and M visa holders through SEVIS — the federal database tracking all international students. Designated School Officials (DSOs) at certified institutions create I-20 forms, report enrollment changes, and verify financial documentation through SEVIS. The current system relies on centralized batch processing with no standards-based credential exchange. Verifiable Credentials issued when SEVIS generates an I-20 enable instant, cryptographic verification — eliminating manual verification calls between institutions.

### DHS SVIP & Federal Alignment

The DHS Science & Technology Directorate's Silicon Valley Innovation Program (SVIP) has funded 60+ companies since 2018 to build blockchain and digital credential solutions for immigration. Key examples include Danube Tech (digital Permanent Resident Card), MATTR (VC issuance for USCIS), and SpruceID ($42M raised, direct DHS contract). USCIS and CBP are actively pursuing W3C VC/DID standards. This project implements the same standards that DHS is funding commercially — as an open-source reference implementation.

### FERPA & Privacy

All personal data is stored off-chain. No PII exists on any blockchain — only cryptographic hashes, credential schemas, and revocation registries are anchored on-chain. SD-JWT selective disclosure requires explicit student consent for each presentation, aligning with FERPA's consent framework. Institutions retain full data control with the ability to revoke credentials when status changes.

### Standards Alignment

| Standard | Status |
|----------|--------|
| W3C Verifiable Credentials 2.0 | Recommendation (May 2025) |
| W3C DID Core v1.0 | Recommendation |
| IETF SD-JWT | Draft |
| ICAO 9303 | PassportCredential MRZ field mapping |
| NIST SP 800-63-4 | Digital wallets in federal identity framework (July 2025) |
| eIDAS 2.0 | EU digital wallets mandatory (Dec 2026) — W3C VC interoperability |
| OID4VCI / OID4VP | OpenID Foundation Drafts |

## Immigration Document Verification Maturity

Each immigration document type has different current verification maturity, creating different opportunities for VC impact:

| Document | Current Verification | VC Opportunity |
|----------|---------------------|----------------|
| **I-20** | Paper document backed by SEVIS; manual phone/email verification between institutions | VC issued when SEVIS generates I-20 enables instant cryptographic verification |
| **Financial Support** | Lowest verification maturity — manual consular review of bank statements and affidavits | Highest VC impact — cryptographically signed attestations from banks/institutions |
| **Passport** | Mature — ICAO PKI with machine-readable zone (MRZ) | VC adds selective disclosure (prove nationality without revealing passport number) |
| **Visa** | Medium — stamp-based, verified against CBP systems | VC standardizes verification across agencies and institutions |
| **DS-2019** | Similar to I-20 — SEVIS-backed paper document | VC enables verifiable J-1 exchange visitor status |
| **I-94** | Electronic — CBP generates online, but verification requires CBP portal access | VC enables portable proof of admission class and dates |

## API Reference

### Documents
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/documents/i20` | List all I-20 records |
| GET | `/api/documents/i20/{id}` | Get I-20 by ID |
| GET | `/api/documents/ds2019` | List DS-2019 records |
| GET | `/api/documents/i94` | List I-94 records |
| GET | `/api/documents/passport` | List passport records |
| GET | `/api/documents/visa` | List visa records |
| GET | `/api/documents/financial-support` | List financial support records |
| GET | `/api/documents/sponsored-students` | List sponsored students |

### DID Operations
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/did/resolve/{did}` | Resolve a DID to its DID Document |
| POST | `/api/did/create` | Create a new DID (key or prism) |
| POST | `/api/did/publish/{did}` | Publish a did:prism DID to Cardano |
| GET | `/api/did/status/{did}` | Get DID publication status |
| GET | `/api/did/methods` | List supported DID methods |

### Verifiable Credentials
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/credential/issue` | Issue an SD-JWT Verifiable Credential |
| GET | `/api/credential/{id}` | Get credential by ID |
| POST | `/api/credential/verify` | Verify a presented SD-JWT |
| GET | `/api/credential/subject/{subjectDid}` | List credentials for a subject |
| POST | `/api/credential/{id}/revoke` | Revoke a credential |
| GET | `/api/credential/status-list/{issuerDid}` | Get issuer's status list bitstring |
| GET | `/api/credential/schemas` | List supported credential schemas |

### OID4VCI (Credential Issuance)
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/oid4vci/metadata` | Issuer metadata (credential configurations) |
| POST | `/api/oid4vci/credential-offer` | Create credential offer with pre-auth code |
| POST | `/api/oid4vci/token` | Exchange pre-auth code for access token |
| POST | `/api/oid4vci/credential` | Issue credential (requires Bearer token) |

### OID4VP (Credential Presentation)
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/oid4vp/scenarios` | List predefined verification scenarios |
| POST | `/api/oid4vp/request` | Create presentation request |
| POST | `/api/oid4vp/response` | Submit and verify presentation |
| GET | `/api/oid4vp/status/{state}` | Check presentation request status |

### Authentication
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/register` | Register new user |
| POST | `/api/auth/login` | Login and receive JWT |

## Credential Schemas

### I20Credential
Maps I-20 (Certificate of Eligibility) fields to verifiable claims:
- **Required**: sevisId, studentName, programStatus, educationLevel, primaryMajor, programStartDate, programEndDate, institutionName
- **Selective Disclosure**: sevisId, educationLevel, primaryMajor, programStartDate, programEndDate (hidden by default, disclosed on request)

### FinancialSupportCredential
Maps financial support documentation to verifiable claims:
- **Required**: sevisId, studentName, academicTerm, totalExpenses, totalFunding
- **Selective Disclosure**: personalFunds, schoolFundsAmount, employmentFunds, otherFundsAmount, tuition, livingExpenses, dependentExpenses, sponsorName

### PassportCredential
Maps ICAO 9303 MRZ (machine-readable zone) fields to verifiable claims:
- **Required**: holderName, nationality, issuingState, documentNumber, dateOfBirth, expirationDate, sex
- **Selective Disclosure**: documentNumber, dateOfBirth, mrzLine1, mrzLine2 (hidden by default — prove nationality without revealing passport number)

### VisaCredential
Maps U.S. visa stamp information to verifiable claims:
- **Required**: holderName, visaType, issuingPost, issueDate, expirationDate
- **Selective Disclosure**: stampNumber, controlNumber, nationality

### DS2019Credential
Maps DS-2019 (Certificate of Eligibility for J-1 Exchange Visitor Status) to verifiable claims:
- **Required**: sevisId, participantName, programSponsor, programNumber, categoryCode, programStartDate, programEndDate
- **Selective Disclosure**: sevisId, participantName, programStartDate, programEndDate

### I94Credential
Maps I-94 Arrival/Departure Record admission status to verifiable claims:
- **Required**: holderName, i94Number, classOfAdmission, admissionDate, admittedUntil
- **Selective Disclosure**: i94Number, holderName

## Blockchain Integration

### Cardano (did:prism) via Hyperledger Identus

This project integrates with Cardano via `did:prism` through [Hyperledger Identus](https://github.com/hyperledger/identus), formerly known as **Atala PRISM**. IOG (Input Output Global) originally developed Atala PRISM as Cardano's decentralized identity solution, then contributed the codebase to the Hyperledger Foundation in 2023. The project was promoted to full Hyperledger project status as **Hyperledger Identus** in April 2024.

The Identus Cloud Agent connects to Cardano's `preprod` testnet to anchor DID operations on-chain:
1. Start the Docker stack: `docker-compose up`
2. The Identus Cloud Agent connects to Cardano preprod automatically
3. Create a did:prism DID: `POST /api/did/create` with `{ "method": "prism" }`
4. Publish to Cardano: `POST /api/did/publish/{did}` — anchors the DID on-chain
5. Track status: `GET /api/did/status/{did}` — shows CREATED → PUBLICATION_PENDING → PUBLISHED
6. Resolve any did:prism identifier: `GET /api/did/resolve/did:prism:{identifier}`

### Midnight (did:midnight) — Zero-Knowledge Privacy

[Midnight](https://midnight.network/) is IOG's privacy-focused Cardano partner chain, using **ZK-SNARKs** (Zero-Knowledge Succinct Non-Interactive Arguments of Knowledge) for confidential smart contracts. Mainnet is planned for late March 2026.

For immigration credentials, Midnight enables proving facts without revealing underlying data — going beyond SD-JWT selective disclosure:
- Prove "is a valid F-1 student" without revealing any claim values (boolean proof)
- Prove "financial support exceeds threshold" without revealing the amount (range proof)
- Prove nationality without revealing passport number, name, or date of birth (set membership proof)

The `did:midnight` resolver is pre-built with a configurable endpoint (`Midnight:ResolverUrl`), currently targeting Midnight's testnet. When mainnet launches, only the configuration URL needs to change.

### cheqd (did:cheqd)

[cheqd](https://cheqd.io/) is a Cosmos-based blockchain purpose-built for decentralized identity. The `did:cheqd` resolver communicates with cheqd's public REST API following the DIF DID Resolution specification.

### Chain-Agnostic Design

The system is not locked to any single blockchain:
- **did:key** for local Ed25519 DIDs (no blockchain dependency)
- **did:web** for institutional issuers (HTTPS-based, self-hosted)
- **DIF Universal Resolver** fallback for any DID method not natively supported
- New DID methods can be added by implementing the `IDidResolver` interface and registering in DI

## Technology Stack

| Layer | Technology |
|-------|-----------|
| Backend | ASP.NET Core 8.0, C# 12 |
| Frontend | Angular 19, TypeScript, Bootstrap 5 |
| Database | SQLite (via EF Core 8.0) |
| Identity | ASP.NET Core Identity + JWT Bearer |
| Credentials | SD-JWT with Ed25519 (EdDSA) signing, W3C VC 2.0 |
| Crypto | Ed25519 via NSec.Cryptography (libsodium), HS256 backward compat |
| DID Methods | did:key, did:web, did:prism, did:cheqd, did:midnight |
| Protocols | OID4VCI, OID4VP |
| Blockchain | Cardano (via Hyperledger Identus), Midnight (ZK-SNARKs, planned) |
| Testing | xUnit, WebApplicationFactory, InMemory EF Core |
| Containerization | Docker, docker-compose |

## Credits

**Steven W. Sevic** — Project lead, architecture, implementation

**Pillarwheel Studios** — Development and design

Built with support from the **Cardano Catalyst Fund 8** community.

### Key Dependencies

- [NSec.Cryptography](https://nsec.rocks/) — Ed25519 signing (MIT, libsodium)
- [Hyperledger Identus](https://github.com/hyperledger/identus) — Cardano DID infrastructure (formerly Atala PRISM; IOG contributed to Hyperledger Foundation 2023, promoted April 2024)
- [Midnight](https://midnight.network/) — IOG's privacy-focused Cardano partner chain (ZK-SNARKs)
- [DIF Universal Resolver](https://github.com/decentralized-identity/universal-resolver) — Multi-method DID resolution
- [cheqd](https://cheqd.io/) — Cosmos-based DID network

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md) for development setup, code conventions, and contribution guidelines.

## Documentation

- [Architecture Decision Records](docs/architecture.md)
- [Regulatory Compliance & Standards](docs/compliance.md)
- [Catalyst Fund 8 Deliverables Report](docs/catalyst-report.md)

## License

Open source. See [LICENSE](LICENSE) for details.
