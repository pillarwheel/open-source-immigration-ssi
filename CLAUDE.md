# Open Source Immigration SSI — Project Conventions

## Overview
Blockchain-based international student immigration document management system.
Funded by Cardano Catalyst Fund 9. Targets W3C Verifiable Credentials, SD-JWT,
OID4VC protocols, and did:prism (Cardano) DID resolution.

## Tech Stack
- **Backend:** ASP.NET Core 8.0, EF Core 8.0 + SQLite
- **Frontend:** Angular 19, Bootstrap 5.3, TypeScript 5.7
- **Database:** SQLite (imm-doc-check.db)
- **Containerization:** Docker + docker-compose

## Project Structure
```
open-source-immigration-ssi/
├── imm-check-server-example/    # .NET backend
│   ├── Controllers/             # API controllers
│   ├── Data/                    # EF Core DbContext
│   ├── Models/                  # Domain models
│   └── Program.cs               # App entry point
├── imm-check-app-example/       # Angular frontend
│   └── src/app/
│       ├── modules/             # Feature modules (document types)
│       └── shared/models/       # TypeScript model classes
├── research/                    # SQL schemas, document examples
└── docker-compose.yml           # Full stack orchestration
```

## Build & Run
```bash
# Backend
cd imm-check-server-example
dotnet restore
dotnet build
dotnet run    # API at https://localhost:7272

# Frontend
cd imm-check-app-example
npm install
ng serve      # App at http://localhost:4200

# Docker (full stack)
docker-compose up
```

## Key Conventions
- **C# Models:** Use EF Core annotations. Primary key is `recnum` on all document tables.
- **API Routes:** `api/documents/{type}` for document CRUD.
- **Angular:** Standalone components (Angular 17+ pattern). One module per document type.
- **Database:** SQLite with EF Core migrations. Connection string in appsettings.
- **CORS:** Locked to `http://localhost:4200` in development.

## Testing
```bash
dotnet test                    # Backend tests
cd imm-check-app-example && ng test   # Frontend tests
```

## Immigration Document Types
1. **I-20** — Student visa sponsorship form (SEVIS)
2. **DS-2019** — Exchange visitor sponsorship
3. **I-94** — Arrival/Departure record
4. **Passport** — Identity document
5. **Visa** — Visa stamp information
6. **Sponsored Student Information** — Sponsorship details
7. **Financial Support** — Bank/scholarship attestation (planned)
