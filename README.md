# League Manager

A football league management platform — teams, members, matches and championships — built with a
**.NET 8 Web API** and an **Angular** frontend.

[![CI](https://github.com/Marian421/angular-dotnet-league-manager/actions/workflows/ci.yml/badge.svg)](https://github.com/Marian421/angular-dotnet-league-manager/actions/workflows/ci.yml)

> **Status: in development.** Built to practise layered .NET architecture and testing rather than
> to ship a product. The Users slice is complete and tested end to end; Teams is functional;
> Matches and Championships are partially built. See [Current state](#current-state) for exactly
> what works — nothing below is aspirational.

## Why this project exists

I wanted to build a .NET API the way a real one is structured — controllers that do no business
logic, a service layer, repositories behind interfaces, explicit DTOs at the boundary, and tests
at both the unit and integration level — rather than putting queries in controllers and calling
it done.

## Current state

| Area | State |
| --- | --- |
| **Users** | ✅ Complete vertical slice: controller → service → repository → mapper → DTO. Registration with hashed passwords, login, list, get by id. Unit **and** integration tested. |
| **Teams** | ✅ Functional: create, list, get by id, through the full service/repository stack. Not yet covered by tests. |
| **Matches** | ⚠️ Controller and model only — no service or repository layer yet. |
| **Championships** | ⚠️ Domain model and database schema only — no API surface yet. |
| **Authentication** | ⚠️ Passwords are hashed and login validates credentials, but **JWT issuing is not wired up** — the packages are referenced and the middleware is not yet configured. Login returns a user id, not a token. |
| **Frontend** | ⚠️ Login, register and landing pages; team list, create, details and manage pages. Not wired to auth. |

## Architecture

```
Angular client  ──HTTP──▶  Controllers      thin — no business logic
                               │
                               ▼
                           Services         business rules, DTO mapping
                               │
                               ▼
                         Repositories       data access behind interfaces
                               │
                               ▼
                        EF Core DbContext ──▶ PostgreSQL
```

Dependencies are registered in `Program.cs` and injected by interface, so services can be unit
tested against fakes without touching a database.

## Domain model

- **User** — owns teams and championships, belongs to teams through `TeamMember`
- **Team** — has an owner, members, and matches (as either side)
- **TeamMember** — join entity carrying role and status, so a member need not be a registered user
- **Match** — two teams, a date, a location, optionally part of a championship
- **Championship** — owner, date range, participating teams, applications, matches

Modelled with EF Core across four migrations.

## Tech stack

**Backend** · .NET 8 · ASP.NET Core Web API · EF Core · PostgreSQL · Swagger/OpenAPI
**Frontend** · Angular · TypeScript
**Testing** · xUnit · integration tests via `WebApplicationFactory`
**CI** · GitHub Actions

## Testing

Two levels, both in `backend.Tests`:

- **Unit tests** — `UserService` tested in isolation against fake repositories, using a
  `UserServiceBuilder` to assemble the subject under test and a `UserFactory` for test data.
- **Integration tests** — the API exercised over real HTTP through a `CustomWebApplicationFactory`,
  covering registration, login, list and get-by-id.

```bash
dotnet test
```

Integration tests are the part I most wanted practice with: they catch routing, model binding and
serialisation problems that unit tests cannot see.

## Running locally

**Requirements:** .NET 8 SDK, Node.js, PostgreSQL.

```bash
# Backend
cd backend
# Connection string is read from the environment (see Program.cs):
#   ConnectionStrings__DefaultConnection=Host=localhost;Database=league;Username=...;Password=...
dotnet ef database update
dotnet run
```

Swagger UI is served at `/swagger` in development.

```bash
# Frontend
cd client/league-manager
npm install
npm start        # http://localhost:4200
```

The API allows CORS from `http://localhost:4200` for the Angular dev server.

## Continuous integration

`.github/workflows/ci.yml` restores, builds in Release and runs the test suite on every push and
pull request to `main`.

## Known gaps

Listed deliberately rather than left to be discovered:

- JWT authentication is not finished — no token issuing, no `[Authorize]` enforcement
- `MatchesController` bypasses the service/repository layers the rest of the API uses
- Championships have no endpoints
- Teams and Matches have no test coverage
- The default `WeatherForecast` scaffolding has not been removed
- Test failures do not currently block CI
