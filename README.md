# dotnet-vsa-template

Modern Vertical Slice Architecture backend template — .NET 10, minimal APIs, EF Core + Postgres.

See [`backend/CLAUDE.md`](backend/CLAUDE.md) for full architecture and coding conventions.

## Using this template

Install the template from a local clone of this repo:

```powershell
dotnet new install .
```

Scaffold a new project (replace `Acme.Orders` with your project name):

```powershell
dotnet new vsa-api -n Acme.Orders
cd Acme.Orders
```

All occurrences of `BackendTemplate` in file names, namespaces, and content are replaced with your project name automatically. Follow the [Local setup](#local-setup) steps below to get the generated project running.

To uninstall the template:

```powershell
dotnet new uninstall .
```

## Prerequisites

| Tool | Version | Install |
|------|---------|---------|
| .NET SDK | 10.0+ | [dot.net](https://dot.net) |
| Docker Desktop | any | [docker.com](https://www.docker.com/products/docker-desktop) |
| dotnet-ef (global tool) | 10.0+ | `dotnet tool install -g dotnet-ef` |

Docker is required for both local Postgres and the Testcontainers-based integration tests.

## Local setup

### 1. Start Postgres

```bash
docker compose up -d
```

This starts a Postgres 16 container on port `5432` with:
- Database: `backendtemplate`
- Username: `postgres`
- Password: `postgres`

### 2. Set the connection string

```powershell
dotnet user-secrets set "ConnectionStrings:Default" "Host=localhost;Port=5432;Database=backendtemplate;Username=postgres;Password=postgres" --project backend/src/BackendTemplate.Api
```

### 3. Apply migrations

```powershell
dotnet ef database update --project backend/src/BackendTemplate.Infrastructure --startup-project backend/src/BackendTemplate.Api
```

### 4. Run the API

```powershell
dotnet watch --project backend/src/BackendTemplate.Api --launch-profile https
```

First-time only — trust the dev certificate:

```powershell
dotnet dev-certs https --trust
```

API available at `https://localhost:5001`

| Endpoint | URL |
|----------|-----|
| Scalar UI | `https://localhost:5001/scalar/v1` |
| OpenAPI JSON | `https://localhost:5001/openapi/v1.json` |
| Liveness | `https://localhost:5001/health/live` |
| Readiness | `https://localhost:5001/health/ready` |

## Running tests

Tests use Testcontainers — Docker must be running. No manual Postgres setup needed for tests.

```bash
dotnet test backend/
```

## Solution structure

```
backend/
  src/
    BackendTemplate.Api/          # ASP.NET Core host, feature slices, endpoint registration
    BackendTemplate.Domain/       # Entities, value objects, domain logic, port interfaces
    BackendTemplate.Infrastructure/  # EF DbContext, migrations, external service implementations
  tests/
    BackendTemplate.Api.Tests/
    BackendTemplate.Domain.Tests/
    BackendTemplate.Infrastructure.Tests/
    BackendTemplate.Testing.Common/   # Shared builders, fixtures
```

## Stopping Postgres

```bash
docker compose down          # stop container, keep data
docker compose down -v       # stop container and delete data
```
