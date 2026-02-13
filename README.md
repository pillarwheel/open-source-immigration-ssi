# ImmCheck: Open-Source Immigration Document Management with SSI

Open-source system for managing international student immigration documents using Self-Sovereign Identity (SSI) and Verifiable Credentials. Built for U.S. university immigration advisors (DSOs), students, and compliance workflows.

**Funded by Cardano Catalyst Fund 9** | **W3C Verifiable Credentials** | **SD-JWT Selective Disclosure** | **OID4VC Protocols** | **Cardano did:prism Support**

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

  ImmCheck.Api.Tests/         xUnit test suite (88 tests)
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
- **W3C Verifiable Credentials 2.0** for I-20 and Financial Support documents
- **SD-JWT** (Selective Disclosure JWT) — prove enrollment status without revealing SEVIS ID
- **Bitstring Status List v1.1** for credential revocation
- **Ed25519 (EdDSA)** asymmetric signing — verifiers only need the public key (no shared secrets)
- **DID Resolution**: did:key (local Ed25519), did:web (HTTPS), did:prism (Cardano/Identus), did:cheqd (Cosmos)
- **DID Publication**: Full Cardano DID lifecycle — create, publish to preprod, track status
- **DIF Universal Resolver** fallback for additional DID methods

### OID4VC Protocols
- **OID4VCI** (OpenID for Verifiable Credential Issuance) — pre-authorized code flow
- **OID4VP** (OpenID for Verifiable Presentations) — presentation exchange with predefined scenarios:
  - Prove valid F-1 status (I-20 credential)
  - Prove financial support (Financial Support credential)

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

88 tests covering: document endpoints, DID resolution/publication, credential issuance/verification, OID4VCI/OID4VP flows, SD-JWT selective disclosure, Ed25519 signing, did:cheqd resolution, revocation, key management, and schema validation.

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

## Cardano Integration

This project integrates with Cardano via `did:prism` through [Hyperledger Identus](https://github.com/hyperledger/identus) (formerly Atala PRISM). The Identus Cloud Agent connects to Cardano's `preprod` testnet to anchor DID operations on-chain.

To enable Cardano integration:
1. Start the Docker stack: `docker-compose up`
2. The Identus Cloud Agent will connect to Cardano preprod automatically
3. Create a did:prism DID: `POST /api/did/create` with `{ "method": "prism" }`
4. Publish to Cardano: `POST /api/did/publish/{did}` — anchors the DID on-chain
5. Track status: `GET /api/did/status/{did}` — shows CREATED → PUBLICATION_PENDING → PUBLISHED
6. Resolve any did:prism identifier: `GET /api/did/resolve/did:prism:{identifier}`

## Technology Stack

| Layer | Technology |
|-------|-----------|
| Backend | ASP.NET Core 8.0, C# 12 |
| Frontend | Angular 19, TypeScript, Bootstrap 5 |
| Database | SQLite (via EF Core 8.0) |
| Identity | ASP.NET Core Identity + JWT Bearer |
| Credentials | SD-JWT with Ed25519 (EdDSA) signing, W3C VC 2.0 |
| Crypto | Ed25519 via NSec.Cryptography (libsodium), HS256 backward compat |
| DID Methods | did:key, did:web, did:prism, did:cheqd |
| Protocols | OID4VCI, OID4VP |
| Blockchain | Cardano (via Hyperledger Identus Cloud Agent) |
| Testing | xUnit, WebApplicationFactory, InMemory EF Core |
| Containerization | Docker, docker-compose |

## Project Background

This project was funded by **Cardano Catalyst Fund 9** ($1,800) to research and prototype blockchain-based immigration document management for U.S. educational institutions. The goal is to provide an open-source system that universities like MIT can use and contribute back to, enabling:

1. International students to hold verifiable credentials for their immigration documents
2. DSOs (Designated School Officials) to issue and verify these credentials
3. Selective disclosure — students prove enrollment status without revealing sensitive information like SEVIS IDs
4. Cardano blockchain anchoring for DID persistence and auditability

## Credits

**Steven W. Sevic** — Project lead, architecture, implementation

**Pillarwheel Studios** — Development and design

Built with support from the **Cardano Catalyst Fund 9** community.

### Key Dependencies

- [NSec.Cryptography](https://nsec.rocks/) — Ed25519 signing (MIT, libsodium)
- [Hyperledger Identus](https://github.com/hyperledger/identus) — Cardano DID infrastructure
- [DIF Universal Resolver](https://github.com/decentralized-identity/universal-resolver) — Multi-method DID resolution
- [cheqd](https://cheqd.io/) — Cosmos-based DID network

## License

Open source. See [LICENSE](LICENSE) for details.
