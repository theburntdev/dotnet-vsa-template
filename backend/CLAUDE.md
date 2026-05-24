# Backend — C# / .NET

See root `CLAUDE.md` for project vocabulary and cross-cutting rules.

## Tech choices (decided)
- **Framework**: ASP.NET Core minimal APIs (not controller-based)
- **Language**: C# 13, .NET 10
- **ORM**: Entity Framework Core with Postgres
- **Validation**: FluentValidation
- **Mapping**: Mapperly (source-generated, compile-time safe — no AutoMapper)
- **Logging**: Serilog (configured in `BackendTemplate.Api` host only — all other projects inject `ILogger<T>`, never reference Serilog directly)
- **OpenAPI**: `Microsoft.AspNetCore.OpenApi` (built-in, no Swashbuckle) + Scalar UI
- **DI scanning**: Scrutor (assembly scanning for handler and service registration)
- **Testing**: xUnit + Testcontainers (integration) + NSubstitute (mocks where needed with strict behavior)

## Architecture
Vertical Slice Architecture (VSA). Each feature operation is a self-contained slice: endpoint, handler, validator, request/response types, and mapper co-located in one folder. No shared application layer — slices own their logic. Cross-cutting infrastructure (DbContext, external services) lives in `Infrastructure`; shared domain primitives live in `Domain`.

## Solution structure (target — not yet created)
```
backend/
  BackendTemplate.sln
  src/
    BackendTemplate.Api/             # ASP.NET Core host, feature slices, endpoint registration
    BackendTemplate.Domain/          # Entities, value objects, domain logic, port interfaces — no EF or framework references
    BackendTemplate.Infrastructure/  # EF DbContext, migrations, entity config, external service implementations
  tests/
    BackendTemplate.Api.Tests/
    BackendTemplate.Domain.Tests/
    BackendTemplate.Infrastructure.Tests/
    BackendTemplate.Testing.Common/  # Shared builders, recipes, fixtures — no production code
```

## Dependency direction
```
Api → Domain
Api → Infrastructure
Infrastructure → Domain
```
`Domain` has zero references to any other project in this solution.

## Essential commands
```powershell
# Build
dotnet build backend/BackendTemplate.sln

# Run API (hot reload)
dotnet watch --project backend/src/BackendTemplate.Api

# Run all tests
dotnet test backend/BackendTemplate.sln

# Run a single test project
dotnet test backend/tests/BackendTemplate.Api.Tests

# Add a migration
dotnet ef migrations add <MigrationName> --project backend/src/BackendTemplate.Infrastructure --startup-project backend/src/BackendTemplate.Api

# Apply migrations
dotnet ef database update --project backend/src/BackendTemplate.Infrastructure --startup-project backend/src/BackendTemplate.Api
```

## API style
- RESTful resource URLs — nouns, not verbs (e.g., `GET /todo-items`, `POST /todo-items`, `PATCH /todo-items/{id}`)
- Standard HTTP verbs: `GET` read, `POST` create, `PUT` full replace, `PATCH` partial update, `DELETE` remove
- 4xx/5xx error bodies follow RFC 7807 Problem Details (`application/problem+json`) — always populate `type`, `title`, `status`, and `detail`; `detail` carries the specific error message from `Result<T>.Failure`
- `201 Created` with `Location` header on successful resource creation
- `204 No Content` on successful delete
- Collection endpoints return arrays wrapped in a paged envelope: `{ "items": [...], "total": n, "page": n, "pageSize": n }`
- Pagination via query params: `?page=1&pageSize=20` (1-based, offset model)
- `PATCH` uses JSON Merge Patch (RFC 7396) — nullable properties only; `null` is always a validation error. Clients omit fields they want to leave unchanged and provide a value for fields they want to update. Field clearing (setting to null) is not supported.
- Authentication deferred — do not add auth middleware, JWT, or authorization attributes until explicitly tasked

## OpenAPI
- JSON document served at `/openapi/v1.json` via `Microsoft.AspNetCore.OpenApi`
- Scalar UI served at `/scalar/v1` via `Scalar.AspNetCore`

## Health checks
- `/health/live` — basic liveness (process is running)
- `/health/ready` — readiness with EF Core DB ping (`AddDbContextCheck<AppDbContext>()`)

## Feature slice structure
Commands and queries are co-located by feature under `Commands/` and `Queries/` subfolders:
```
Api/
  Features/
    TodoItems/
      Commands/
        CreateTodoItem/
          Endpoint.cs       # IEndpoint implementation, endpoint registration
          Handler.cs        # Plain class with HandleAsync method
          Validator.cs      # IValidator<CreateTodoItemRequest> — commands; queries with non-trivial params
          Request.cs        # Only if operation has a body or non-trivial query params
          Response.cs       # Only if operation returns a mapped DTO
      Queries/
        GetTodoItem/
          Endpoint.cs
          Handler.cs
          Response.cs
        ListTodoItems/
          Endpoint.cs
          Handler.cs
          Response.cs
  Common/
    IEndpoint.cs            # Marker interface for auto-discovery
    IScopedService.cs       # Lifetime marker interfaces
    ISingletonService.cs
    ITransientService.cs
    Page.cs                 # Page<T> response envelope
    ValidationFilter.cs     # Generic endpoint filter for FluentValidation
    ResultExtensions.cs     # Result<T>.ToHttpResult() extension methods
```

File presence rules:
- `Endpoint.cs` + `Handler.cs` — always present
- `Request.cs` — only if operation has a request body (POST/PUT/PATCH) or non-trivial query params
- `Response.cs` — only if operation returns a domain-mapped DTO (omit for `204 No Content`)
- `Validator.cs` — always for commands; also for queries that have a `Request.cs` with non-trivial params (e.g., pagination bounds); omit for queries with no `Request.cs`

Shared mapper: if 3+ slices map the same **single entity** identically, promote to `Api/Features/TodoItems/TodoItemMapper.cs` at the feature root. Never create cross-feature mappers.

## Endpoint registration
Each `Endpoint.cs` implements `IEndpoint` and self-registers via assembly scan at startup:
```csharp
// Api/Common/IEndpoint.cs
public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}

// Program.cs — scans and registers all IEndpoint implementations
app.MapEndpoints(typeof(Program).Assembly);
```
Adding a new slice requires no changes to `Program.cs`.

`IEndpoint` implementations must have parameterless constructors — they are stateless configuration objects, not services. All dependencies (handlers, etc.) are injected at request time via route handler parameters, not into the `IEndpoint` class itself.

## Handler structure
All handlers — command and query — return `Result<T>`. No exceptions:
```csharp
public sealed class CreateTodoItemHandler(AppDbContext db) : IScopedService
{
    public async Task<Result<TodoItem>> HandleAsync(
        CreateTodoItemRequest request, CancellationToken ct = default)
    {
        // ... domain logic, db operations
        await db.SaveChangesAsync(ct);
        return Result<TodoItem>.Success(entity);
    }
}
```
- Command handlers return `Result<TEntity>` — endpoint maps entity to response DTO via `ToHttpResult`
- Query handlers return `Result<TResponse>` — endpoint forwards via `ToHttpResult`
- Delete handlers return `Result<Unit>` — endpoint returns `204 No Content` via `ToHttpResult`
- Every `HandleAsync` accepts `CancellationToken ct = default` as last parameter and forwards it to all async calls
- EF Core exceptions from `SaveChangesAsync` are not expected failures — catch them explicitly in handlers only where a specific exception is anticipated (e.g., `DbUpdateConcurrencyException` on entities with concurrency tokens → `Result<T>.Failure(..., ErrorKind.Conflict)`). Do not wrap every `SaveChangesAsync` call defensively. All other EF exceptions bubble to the global handler → `500`.

## Validation
FluentValidation runs via a generic endpoint filter — this is the **only** validation error path. Endpoints with a request body opt in:
```csharp
// In Endpoint.cs
app.MapPost("/todo-items", handler)
   .WithValidation<CreateTodoItemRequest>();

// Api/Common/ValidationFilter.cs — shared, ~20 lines
// Resolves IValidator<TRequest> from DI, runs validation, returns 422 Problem Details on failure
```
Validators are registered automatically via `AddValidatorsFromAssembly`. If `.WithValidation<TRequest>()` is called but no `IValidator<TRequest>` is registered, an `InvalidOperationException` is thrown at startup — this is intentional to catch missing validators early.

Never call `validator.ValidateAndThrow()` — always use the filter. The global exception handler does **not** handle `ValidationException`; it handles only unexpected exceptions → `500`.

## Logging
`UseSerilogRequestLogging()` in `Program.cs` handles all HTTP request logging — method, path, status code, elapsed ms. No per-slice logging behavior. Never log request/response body content (PII risk).

## DI registration
`Program.cs` registers Api-layer concerns via Scrutor. Infrastructure registers its own services via `AddInfrastructure()`:
```csharp
// Program.cs
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.Scan(scan => scan
    .FromAssemblyOf<Program>()
    .AddClasses(c => c.AssignableTo<IScopedService>()).AsSelf().WithScopedLifetime()
    .AddClasses(c => c.AssignableTo<ISingletonService>()).AsSelf().WithSingletonLifetime()
    .AddClasses(c => c.AssignableTo<ITransientService>()).AsSelf().WithTransientLifetime());

builder.Services.AddInfrastructure(builder.Configuration);
```

`AddInfrastructure()` is an extension method in `BackendTemplate.Infrastructure` that registers `AppDbContext`, external service implementations, and anything else Infrastructure owns. It may use Scrutor internally.

- Handlers → `IScopedService`
- External service implementations → whichever lifetime suits the integration
- Mapperly mappers are `static partial class` — not registered in DI at all

## Code conventions
- Domain entities are plain C# classes with no EF attributes — use fluent config in `Infrastructure` via one `IEntityTypeConfiguration<TEntity>` class per entity.
- Entity primary keys are strongly-typed IDs defined as `record struct` in `BackendTemplate.Domain.Common`:
  ```csharp
  record struct TodoItemId(Guid Value);
  ```
  Never use raw `Guid` or `int` as entity ID types.
- EF Core value conversion for strongly-typed IDs is registered via a bulk convention in `AppDbContext.ConfigureConventions()` using a single generic `StronglyTypedIdConverter<TId>`. The convention auto-discovers all `record struct` types with a single `Guid Value` property — adding a new ID type requires no changes anywhere.
- Return `TypedResults.*` from endpoints, not raw status codes.
- Use `record` types for DTOs (request/response shapes).
- Never return domain entities from endpoints — always map to a response DTO.
- **Mapper selection rule:**
  - Single entity → response DTO: use **Mapperly** (`static partial class`, compile-time safe, called as a static method — not injected via DI)
  - Multi-entity / denormalized response: use **inline EF Core `Select()` projection** (DB-level, fetches only required columns). Do not use Mapperly for cross-entity projections — it cannot be translated to SQL.
- Mapper placement: Mapperly mapper lives in the slice folder that owns the mapping. Never put mappers in `Domain`.
- Static mapper call site example:
  ```csharp
  // Endpoint.cs route handler — mapper called as static method, not injected
  result.ToHttpResult(TodoItemMapper.ToResponse, location: r => $"/todo-items/{r.Id}")
  ```
- Async all the way down — every method touching I/O returns `Task<T>`.
- Nullable reference types enabled (`<Nullable>enable</Nullable>`) — no `#nullable disable`, no `!` suppression without a comment explaining why.
- Never throw exceptions for expected domain errors; use `Result<T>` (defined in `BackendTemplate.Domain.Common`).
  ```csharp
  // BackendTemplate.Domain.Common.Result<T>
  Result<T>.Success(T value)
  Result<T>.Failure(string error, ErrorKind kind = ErrorKind.Validation)
  bool IsSuccess / bool IsFailure
  T Value          // throws if failure
  string Error     // throws if success
  ErrorKind Kind   // throws if success

  enum ErrorKind { Validation, NotFound, Conflict }
  // ToHttpResult() maps: Validation → 422, NotFound → 404, Conflict → 409

  // When to use each kind:
  // Validation — domain business rule violated (structurally valid request, semantically wrong — e.g., "title must be unique")
  // NotFound   — entity does not exist
  // Conflict   — entity exists but is in the wrong state for the operation (e.g., "cannot delete a completed item")

  // Unit — for void operations (delete)
  Result<Unit>.Success(Unit.Value)
  ```
- Endpoints unwrap `Result<T>` via extension methods in `Api/Common/ResultExtensions.cs` — never inline `.IsSuccess` branches in endpoint bodies:
  ```csharp
  // Returns 200 OK with mapped response body
  result.ToHttpResult(mapper.ToResponse)

  // Returns 201 Created with Location header and mapped response body
  result.ToHttpResult(mapper.ToResponse, location: r => $"/todo-items/{r.Id}")

  // Returns 204 No Content (for Result<Unit>)
  result.ToHttpResult()
  ```
  All overloads return RFC 7807 Problem Details on failure. `IResult` (ASP.NET Core) is an endpoint concern only — never use it in `Domain` or `Infrastructure`.
- Handlers call `await db.SaveChangesAsync(ct)` directly — no repository pattern, no `IUnitOfWork`.
- `Page<T>` is defined in `Api/Common/`. Pagination is handled via a `ToPagedAsync` extension on `IQueryable<T>` in `Infrastructure`:
  ```csharp
  // Infrastructure/Extensions/QueryableExtensions.cs
  public static async Task<(IReadOnlyList<T> Items, int Total)> ToPagedAsync<T>(
      this IQueryable<T> query, int page, int pageSize, CancellationToken ct = default)
  ```
  Handlers build a filtered `IQueryable`, call `ToPagedAsync`, then wrap the result into `Page<T>`.
  ```csharp
  // Api/Common/Page.cs
  record Page<T>(IReadOnlyList<T> Items, int Total, int Page, int PageSize);
  ```
- `page` is 1-based at the API layer — `ToPagedAsync` translates to 0-based EF `Skip()` via `(page - 1) * pageSize`.
- Global exception handler implemented as `IExceptionHandler` in `BackendTemplate.Api` — maps unhandled exceptions → `500` Problem Details only. Does not handle `ValidationException` — validation errors are handled exclusively by `ValidationFilter`.

## External service ports (in `BackendTemplate.Domain`)
Domain defines port interfaces for external capabilities it needs. `Infrastructure` implements them. Handlers inject the domain interface. `Infrastructure` registers the implementation via `AddInfrastructure()`. Lifetime set via marker interface on the implementation class.

## Test data builders (in `BackendTemplate.Testing.Common`)
- One builder per domain entity, named `<Entity>Builder` (e.g., `TodoItemBuilder`).
- Each `WithX(...)` method sets one property and returns `this` for chaining.
- `Build()` returns a fully constructed, valid domain entity.
- Recipes are static factory methods on the builder that return a preconfigured builder for a named scenario:
  ```csharp
  // Generic
  new TodoItemBuilder().WithTitle("Buy milk").WithStatus(TodoStatus.Pending).Build();

  // Recipe
  TodoItemBuilder.InProgress().Build();
  TodoItemBuilder.Done().WithTitle("Archived task").Build();
  ```
- Default values in constructors must produce a valid entity — recipes override only what the scenario needs.

## Test structure
Tests mirror the feature slice structure:
```
BackendTemplate.Api.Tests/
  Features/
    TodoItems/
      Commands/
        CreateTodoItemHandlerTests.cs
      Queries/
        GetTodoItemHandlerTests.cs
```
Handler tests are integration tests (Testcontainers + real DB). No `Application.Tests` project — there is no Application layer.

`BackendTemplate.Domain.Tests` contains pure unit tests for domain logic — no DB, no DI, fast.
`BackendTemplate.Infrastructure.Tests` contains tests for Infrastructure-specific concerns (e.g., complex EF query helpers).

## Test isolation (integration tests)
- One Testcontainers Postgres instance shared across all test classes in a project via `ICollectionFixture<DatabaseFixture>`.
- `DatabaseFixture` applies EF Core migrations on startup — the test schema matches what production runs.
- Each test wraps its DbContext operations in a transaction rolled back in `Dispose` — zero data leakage between tests.
- Never reset state via migrations or container restarts between individual tests.

## Test conventions
- Test method naming: `{MethodUnderTest}_Given{Scenario}_Then{Assertion}`
  ```csharp
  HandleAsync_GivenValidCommand_ThenReturnsCreatedTodoItem()
  ToHttpResult_GivenNotFoundResult_ThenReturns404()
  ```
- One assertion concept per test — split multiple outcomes into separate methods. The only exception is if the scenarios are as simple as an InlineData set up with an input and expected output.

## What NOT to do
- Do not create a `BackendTemplate.Application` project — there is no application layer in this architecture.
- Do not use MediatR — handlers are plain classes injected directly.
- Do not use the repository pattern or `IUnitOfWork` — handlers inject `AppDbContext` and call `SaveChangesAsync` directly.
- Do not put business logic in endpoints or DbContext — it belongs in `Domain`.
- Do not use `[ApiController]` or MVC controllers — minimal APIs only.
- Do not seed test data via migrations — use test fixtures or DbContext seeding in tests.
- Do not construct domain entities directly in tests — use builders from `BackendTemplate.Testing.Common`.
- Do not use raw `Guid` or `int` as entity ID types — use the strongly-typed ID `record struct` from `BackendTemplate.Domain.Common`.
- Do not create cross-feature mappers — each slice owns its mapping; promote to feature root only when 3+ slices share identical mapping of the **same single entity**.
- Do not implement `IResult` (ASP.NET Core) in `Domain` or `Infrastructure` — it is an endpoint concern only.
- Do not call `validator.ValidateAndThrow()` — use the `ValidationFilter` exclusively.
- Do not handle `ValidationException` in the global exception handler — the filter is the only validation error path.
- Do not use Mapperly for multi-entity / denormalized projections — use inline EF Core `Select()` instead.
- Do not register Mapperly mappers in DI or inject them as dependencies — they are `static partial class` and called as static methods.
- Do not add constructor dependencies to `IEndpoint` implementations — they must have parameterless constructors; all dependencies are resolved at request time via route handler parameters.

## Environment / secrets
- Connection string key: `ConnectionStrings:Default`
- Store locally with: `dotnet user-secrets set "ConnectionStrings:Default" "..."` inside `BackendTemplate.Api`
- Never commit `appsettings.Development.json` with real values.
