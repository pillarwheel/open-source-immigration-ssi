# Contributing to ImmCheck

Thank you for your interest in contributing to the Open Source Immigration SSI project. This guide covers development setup, code conventions, and contribution guidelines.

## Development Setup

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 18+](https://nodejs.org/) (for Angular frontend)
- [Docker](https://docs.docker.com/get-docker/) (optional, for Cardano/Identus integration)

### Build & Run

```bash
# Backend
cd src
dotnet restore
dotnet build ImmCheck.sln
dotnet run --project ImmCheck.Api    # API at https://localhost:5001

# Frontend
cd imm-check-app-example
npm install
ng serve                             # App at http://localhost:4200

# Docker (full stack)
docker-compose up
```

### Run Tests

```bash
cd src
dotnet test ImmCheck.Api.Tests       # 119 tests
```

## Architecture

ImmCheck follows **Clean Architecture** with four projects:

```
ImmCheck.Core           (innermost — no external dependencies)
    ↑
ImmCheck.Infrastructure (depends on Core — EF Core, HTTP clients, crypto)
    ↑
ImmCheck.Api            (depends on Core + Infrastructure — controllers, middleware)
    ↑
ImmCheck.Api.Tests      (depends on Api — xUnit integration + unit tests)
```

- **Core** contains domain models, interfaces, DTOs, and SSI type definitions. It has no NuGet dependencies beyond the .NET BCL.
- **Infrastructure** implements Core interfaces: EF Core repositories, DID resolvers, SD-JWT issuer, key stores, OID4VC stores.
- **Api** is the ASP.NET Core host with controllers, middleware, and seed data.
- **Tests** use `WebApplicationFactory` with InMemory EF Core for integration tests and direct class instantiation for unit tests.

## Code Conventions

### C# / .NET

- **EF Core annotations** for model configuration. Primary key is `recnum` on all immigration document tables.
- **API routes** follow the pattern `api/documents/{type}` for document CRUD, `api/did/...` for DID operations, `api/credential/...` for VC operations.
- **Clean Architecture boundaries:** Controllers depend only on Core interfaces. Infrastructure implements those interfaces. Never reference Infrastructure directly from Controllers.
- **DI registration:** New services are registered in `Program.cs`. DID resolvers use `AddHttpClient<T>()` + `AddScoped<IDidResolver, T>()`.
- **Database:** SQLite with EF Core migrations. Connection string in `appsettings.json`. Migrations use `CREATE TABLE IF NOT EXISTS` for pre-existing tables.
- **Signing:** Ed25519 (EdDSA) by default via NSec.Cryptography. HS256 backward compatibility maintained.

### Angular Frontend

- **Standalone components** (Angular 17+ pattern). No NgModules.
- One feature directory per document type under `src/app/modules/`.
- Services in `src/app/core/services/`.
- **CORS:** Backend locked to `http://localhost:4200` in development.

### General

- Keep PRs focused — one feature or fix per PR.
- Follow existing naming conventions in the file you're editing.
- No unnecessary dependencies. Prefer the standard library where possible.

## Testing Guidelines

### Test Structure

Tests are organized in `src/ImmCheck.Api.Tests/`:

- **Integration tests** use `WebApplicationFactory<Program>` with InMemory EF Core to test full HTTP request/response cycles
- **Unit tests** test infrastructure components directly (DID resolvers, SD-JWT, crypto, schemas)

### Key Patterns

- **Unique InMemory DB per test class:** Each `WebApplicationFactory` instance uses `"TestDb_" + Guid.NewGuid()` to prevent cross-test data contamination.
- **InMemory provider guard:** `MigrateAsync()` is guarded with a provider check — it fails with InMemory EF Core, so tests skip migration.
- **Mocked HTTP:** DID resolver tests that call external APIs (cheqd, midnight, universal) use mocked `HttpMessageHandler` instances.

### Running Tests

```bash
cd src
dotnet test ImmCheck.Api.Tests                          # All 119 tests
dotnet test ImmCheck.Api.Tests --filter "ClassName~Did"  # Filter by class name
dotnet test ImmCheck.Api.Tests --filter "Name~SdJwt"     # Filter by test name
```

## Adding a New DID Method

1. Create a resolver class in `ImmCheck.Infrastructure/SSI/` implementing `IDidResolver`:
   ```csharp
   public class DidExampleResolver : IDidResolver
   {
       public string Method => "example";
       public bool CanResolve(string did) => did.StartsWith("did:example:");
       public Task<DidDocument?> ResolveAsync(string did) { ... }
   }
   ```
2. Register in `Program.cs`:
   ```csharp
   builder.Services.AddHttpClient<DidExampleResolver>();
   builder.Services.AddScoped<IDidResolver, DidExampleResolver>();
   ```
3. Add tests in `ImmCheck.Api.Tests/SSI/`.

## Adding a New Credential Schema

1. Add the schema definition in `ImmCheck.Core/SSI/Credentials/CredentialSchemas.cs`
2. Include it in `GetAllSchemas()` return list
3. Add integration tests for issuance and verification
4. Add an OID4VP scenario if applicable

## Pull Request Guidelines

1. **Branch naming:** `feature/short-description` or `fix/short-description`
2. **Tests pass:** All 119 existing tests must pass. Add new tests for new functionality.
3. **No secrets:** Never commit API keys, connection strings with credentials, or private keys.
4. **Describe changes:** PR description should explain what changed and why.
5. **Small PRs:** Prefer multiple small PRs over one large PR.

## Reporting Issues

Open an issue on GitHub with:
- Steps to reproduce
- Expected behavior
- Actual behavior
- Environment (OS, .NET version, Node version)

## License

By contributing, you agree that your contributions will be licensed under the same license as the project. See [LICENSE](LICENSE) for details.
