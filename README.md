# Flagship

An ASP.NET Core Web API backend built with a layered architecture: a thin API
layer, an application/service layer, a core domain layer, and a set of
infrastructure projects (data access, caching, cross-cutting concerns). Data
access is plain ADO.NET (`Microsoft.Data.SqlClient`) against SQL Server stored
procedures — there's no ORM.

## Solution layout

| Project | Purpose |
|---|---|
| `Flagship.Core` | Domain entities, view models, DTOs, and the interfaces everything else implements/consumes. No dependencies on other Flagship projects. |
| `Flagship.Application` | Application services (`UserService`, `TerritoryService`, `LoginLogService`, `RefreshTokenService`) — the orchestration layer between the API and the repositories. |
| `Flagship.Infrastructure.Persistance` | Repository implementations (`UserRepository`, `TerritoryRepository`, `LoginLogRepository`, `RefreshTokenRepository`, `BaseRepository`) that call SQL Server stored procedures via ADO.NET. |
| `Flagship.Infrastructure.Common` | Small dependency-free helpers shared across layers: `Conversion` (safe `DBNull`-aware type conversions for reading `SqlDataReader` values) and `PasswordHasher` (PBKDF2 password hashing). |
| `Flagship.Infrastructure.Caching` | `IMemoryCacheProvider` wrapper around `IMemoryCache`. |
| `Flagship.Infrastructure.Extension` | Cross-cutting API concerns: JWT authentication setup (`Security/Authentication.cs`), the global exception-handling middleware, Serilog-based logging (`IApplogger`), and the DI composition root (`Container/DependencyContainer.cs`). |
| `Flagship.API` | The ASP.NET Core Web API host — controllers, `Program.cs`, configuration. |

Target framework: **.NET 10**.

## Authentication

Login (`POST /api/v1/Account/Authenticate`) verifies credentials and returns a
short-lived JWT access token plus an opaque refresh token:

- Access tokens are signed JWTs (`TokenAuthentication:SecretKey`,
  HMAC-SHA256), expiring after `JWTTokenExpirationTimeInMinutes` minutes.
- Refresh tokens are random 256-bit values (never JWTs) — only their SHA-256
  hash is stored server-side. `POST /api/v1/Account/RefreshToken` exchanges a
  still-valid refresh token for a new access token *and* a new refresh token
  (rotation). Presenting an already-used/revoked refresh token is treated as a
  possible theft/replay and revokes every refresh token for that user.
- `POST /api/v1/Account/Logout` revokes all of a user's refresh tokens.
- Passwords are verified server-side in application code against a PBKDF2
  hash (`PasswordHasher`), not compared in SQL.

See `db/migrations/README.md` for the schema this depends on (draft SQL,
**not yet applied** to any database — review before running).

## Running locally

Prerequisites: .NET 10 SDK, a reachable SQL Server instance with the expected
stored procedures/schema.

Required configuration is intentionally left blank in `appsettings.json` /
`appsettings.Development.json` — set the real values via
[.NET User Secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets)
(or environment variables in non-dev environments) so nothing sensitive ends
up committed:

```
cd src/Flagship.API
dotnet user-secrets init   # only needed once
dotnet user-secrets set "ConnectionStrings:FlagshipConnectionString" "<connection string>"
dotnet user-secrets set "TokenAuthentication:SecretKey" "<a long random value>"
```

Then run the API:

```
dotnet run --project src/Flagship.API
```

In the `Development` environment, Swagger UI is available at `/swagger`.

## License

MIT — see `LICENSE`.
