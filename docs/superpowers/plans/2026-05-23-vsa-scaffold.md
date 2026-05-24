# VSA Solution Scaffold Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Scaffold a complete .NET 10 Vertical Slice Architecture backend solution per `backend/CLAUDE.md`, including all CRUD feature slices for TodoItems, with a passing test suite.

**Architecture:** VSA with Domain / Infrastructure / Api separation. Each feature operation is a self-contained slice (Endpoint + Handler + optional Request/Response/Validator). Cross-cutting infrastructure lives in `BackendTemplate.Infrastructure`; domain primitives in `BackendTemplate.Domain`. No MediatR, no repositories.

**Tech Stack:** ASP.NET Core 10 minimal APIs, EF Core 10 + Npgsql, FluentValidation 11, Mapperly 4, Serilog, Scrutor, Scalar UI, xUnit + Testcontainers 4, NSubstitute 5

---

## File Map

### BackendTemplate.Domain
- `backend/src/BackendTemplate.Domain/BackendTemplate.Domain.csproj`
- `backend/src/BackendTemplate.Domain/Common/Result.cs` — `Result<T>`, `Unit`, `ErrorKind`
- `backend/src/BackendTemplate.Domain/Common/TodoItemId.cs` — strongly-typed ID `record struct`
- `backend/src/BackendTemplate.Domain/Entities/TodoItem.cs` — entity + `TodoStatus` enum
- `backend/src/BackendTemplate.Domain/Ports/IEmailSender.cs` — example port interface

### BackendTemplate.Infrastructure
- `backend/src/BackendTemplate.Infrastructure/BackendTemplate.Infrastructure.csproj`
- `backend/src/BackendTemplate.Infrastructure/Persistence/AppDbContext.cs` — EF DbContext with bulk strongly-typed ID convention
- `backend/src/BackendTemplate.Infrastructure/Persistence/Converters/StronglyTypedIdConverter.cs` — generic `ValueConverter<TId, Guid>`
- `backend/src/BackendTemplate.Infrastructure/Persistence/Configurations/TodoItemConfiguration.cs` — `IEntityTypeConfiguration<TodoItem>`
- `backend/src/BackendTemplate.Infrastructure/Extensions/QueryableExtensions.cs` — `ToPagedAsync<T>`
- `backend/src/BackendTemplate.Infrastructure/Extensions/ServiceCollectionExtensions.cs` — `AddInfrastructure()`

### BackendTemplate.Api — Common
- `backend/src/BackendTemplate.Api/BackendTemplate.Api.csproj`
- `backend/src/BackendTemplate.Api/Common/IEndpoint.cs`
- `backend/src/BackendTemplate.Api/Common/IScopedService.cs`
- `backend/src/BackendTemplate.Api/Common/ISingletonService.cs`
- `backend/src/BackendTemplate.Api/Common/ITransientService.cs`
- `backend/src/BackendTemplate.Api/Common/Page.cs`
- `backend/src/BackendTemplate.Api/Common/ValidationFilter.cs`
- `backend/src/BackendTemplate.Api/Common/ResultExtensions.cs`
- `backend/src/BackendTemplate.Api/Common/GlobalExceptionHandler.cs`
- `backend/src/BackendTemplate.Api/Common/EndpointExtensions.cs`
- `backend/src/BackendTemplate.Api/Program.cs`
- `backend/src/BackendTemplate.Api/appsettings.json`
- `backend/src/BackendTemplate.Api/appsettings.Development.json`

### BackendTemplate.Api — Features/TodoItems (shared)
- `backend/src/BackendTemplate.Api/Features/TodoItems/TodoItemResponse.cs` — shared DTO (promoted to feature root because 3+ slices share same mapping)
- `backend/src/BackendTemplate.Api/Features/TodoItems/TodoItemMapper.cs` — Mapperly `static partial class`

### BackendTemplate.Api — Features/TodoItems slices
- `backend/src/BackendTemplate.Api/Features/TodoItems/Commands/CreateTodoItem/Request.cs`
- `backend/src/BackendTemplate.Api/Features/TodoItems/Commands/CreateTodoItem/Validator.cs`
- `backend/src/BackendTemplate.Api/Features/TodoItems/Commands/CreateTodoItem/Handler.cs`
- `backend/src/BackendTemplate.Api/Features/TodoItems/Commands/CreateTodoItem/Endpoint.cs`
- `backend/src/BackendTemplate.Api/Features/TodoItems/Commands/UpdateTodoItem/Request.cs`
- `backend/src/BackendTemplate.Api/Features/TodoItems/Commands/UpdateTodoItem/Validator.cs`
- `backend/src/BackendTemplate.Api/Features/TodoItems/Commands/UpdateTodoItem/Handler.cs`
- `backend/src/BackendTemplate.Api/Features/TodoItems/Commands/UpdateTodoItem/Endpoint.cs`
- `backend/src/BackendTemplate.Api/Features/TodoItems/Commands/DeleteTodoItem/Handler.cs`
- `backend/src/BackendTemplate.Api/Features/TodoItems/Commands/DeleteTodoItem/Endpoint.cs`
- `backend/src/BackendTemplate.Api/Features/TodoItems/Queries/GetTodoItem/Handler.cs`
- `backend/src/BackendTemplate.Api/Features/TodoItems/Queries/GetTodoItem/Endpoint.cs`
- `backend/src/BackendTemplate.Api/Features/TodoItems/Queries/ListTodoItems/Request.cs`
- `backend/src/BackendTemplate.Api/Features/TodoItems/Queries/ListTodoItems/Validator.cs`
- `backend/src/BackendTemplate.Api/Features/TodoItems/Queries/ListTodoItems/Handler.cs`
- `backend/src/BackendTemplate.Api/Features/TodoItems/Queries/ListTodoItems/Endpoint.cs`

### Test Projects
- `backend/tests/BackendTemplate.Testing.Common/BackendTemplate.Testing.Common.csproj`
- `backend/tests/BackendTemplate.Testing.Common/DatabaseFixture.cs` — Testcontainers Postgres fixture + collection definition
- `backend/tests/BackendTemplate.Testing.Common/Builders/TodoItemBuilder.cs`
- `backend/tests/BackendTemplate.Api.Tests/BackendTemplate.Api.Tests.csproj`
- `backend/tests/BackendTemplate.Api.Tests/Features/TodoItems/Commands/CreateTodoItemHandlerTests.cs`
- `backend/tests/BackendTemplate.Api.Tests/Features/TodoItems/Commands/UpdateTodoItemHandlerTests.cs`
- `backend/tests/BackendTemplate.Api.Tests/Features/TodoItems/Commands/DeleteTodoItemHandlerTests.cs`
- `backend/tests/BackendTemplate.Api.Tests/Features/TodoItems/Queries/GetTodoItemHandlerTests.cs`
- `backend/tests/BackendTemplate.Api.Tests/Features/TodoItems/Queries/ListTodoItemsHandlerTests.cs`
- `backend/tests/BackendTemplate.Domain.Tests/BackendTemplate.Domain.Tests.csproj`
- `backend/tests/BackendTemplate.Domain.Tests/Common/ResultTests.cs`
- `backend/tests/BackendTemplate.Infrastructure.Tests/BackendTemplate.Infrastructure.Tests.csproj`
- `backend/tests/BackendTemplate.Infrastructure.Tests/Extensions/QueryableExtensionsTests.cs`

---

## Task 1: Solution and project scaffolding

**Files:**
- Create: `backend/BackendTemplate.sln`
- Create: all 7 `.csproj` files

- [ ] **Step 1: Create directory tree**

```powershell
New-Item -ItemType Directory -Force backend/src/BackendTemplate.Api
New-Item -ItemType Directory -Force backend/src/BackendTemplate.Domain
New-Item -ItemType Directory -Force backend/src/BackendTemplate.Infrastructure
New-Item -ItemType Directory -Force backend/tests/BackendTemplate.Api.Tests
New-Item -ItemType Directory -Force backend/tests/BackendTemplate.Domain.Tests
New-Item -ItemType Directory -Force backend/tests/BackendTemplate.Infrastructure.Tests
New-Item -ItemType Directory -Force backend/tests/BackendTemplate.Testing.Common
```

- [ ] **Step 2: Create project files**

`backend/src/BackendTemplate.Domain/BackendTemplate.Domain.csproj`:
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <LangVersion>13</LangVersion>
  </PropertyGroup>
</Project>
```

`backend/src/BackendTemplate.Infrastructure/BackendTemplate.Infrastructure.csproj`:
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <LangVersion>13</LangVersion>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="10.0.*" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="10.0.*">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="10.0.*" />
    <PackageReference Include="Scrutor" Version="5.*" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\BackendTemplate.Domain\BackendTemplate.Domain.csproj" />
  </ItemGroup>
</Project>
```

`backend/src/BackendTemplate.Api/BackendTemplate.Api.csproj`:
```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <LangVersion>13</LangVersion>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="FluentValidation.DependencyInjectionExtensions" Version="11.*" />
    <PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="10.0.*" />
    <PackageReference Include="Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore" Version="10.0.*" />
    <PackageReference Include="Riok.Mapperly" Version="4.*" />
    <PackageReference Include="Scalar.AspNetCore" Version="2.*" />
    <PackageReference Include="Scrutor" Version="5.*" />
    <PackageReference Include="Serilog.AspNetCore" Version="9.*" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\BackendTemplate.Domain\BackendTemplate.Domain.csproj" />
    <ProjectReference Include="..\BackendTemplate.Infrastructure\BackendTemplate.Infrastructure.csproj" />
  </ItemGroup>
</Project>
```

`backend/tests/BackendTemplate.Testing.Common/BackendTemplate.Testing.Common.csproj`:
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <LangVersion>13</LangVersion>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore.Relational" Version="10.0.*" />
    <PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="10.0.*" />
    <PackageReference Include="Testcontainers.PostgreSql" Version="4.*" />
    <PackageReference Include="xunit" Version="2.*" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\..\src\BackendTemplate.Domain\BackendTemplate.Domain.csproj" />
    <ProjectReference Include="..\..\src\BackendTemplate.Infrastructure\BackendTemplate.Infrastructure.csproj" />
  </ItemGroup>
</Project>
```

`backend/tests/BackendTemplate.Api.Tests/BackendTemplate.Api.Tests.csproj`:
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <LangVersion>13</LangVersion>
    <IsPackable>false</IsPackable>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.*" />
    <PackageReference Include="NSubstitute" Version="5.*" />
    <PackageReference Include="xunit" Version="2.*" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.*">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\..\src\BackendTemplate.Api\BackendTemplate.Api.csproj" />
    <ProjectReference Include="..\BackendTemplate.Testing.Common\BackendTemplate.Testing.Common.csproj" />
  </ItemGroup>
</Project>
```

`backend/tests/BackendTemplate.Domain.Tests/BackendTemplate.Domain.Tests.csproj`:
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <LangVersion>13</LangVersion>
    <IsPackable>false</IsPackable>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.*" />
    <PackageReference Include="xunit" Version="2.*" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.*">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\..\src\BackendTemplate.Domain\BackendTemplate.Domain.csproj" />
  </ItemGroup>
</Project>
```

`backend/tests/BackendTemplate.Infrastructure.Tests/BackendTemplate.Infrastructure.Tests.csproj`:
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <LangVersion>13</LangVersion>
    <IsPackable>false</IsPackable>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.*" />
    <PackageReference Include="xunit" Version="2.*" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.*">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\..\src\BackendTemplate.Infrastructure\BackendTemplate.Infrastructure.csproj" />
    <ProjectReference Include="..\BackendTemplate.Testing.Common\BackendTemplate.Testing.Common.csproj" />
  </ItemGroup>
</Project>
```

- [ ] **Step 3: Create solution and add projects**

```powershell
dotnet new sln -n BackendTemplate -o backend
dotnet sln backend/BackendTemplate.sln add `
  backend/src/BackendTemplate.Api/BackendTemplate.Api.csproj `
  backend/src/BackendTemplate.Domain/BackendTemplate.Domain.csproj `
  backend/src/BackendTemplate.Infrastructure/BackendTemplate.Infrastructure.csproj `
  backend/tests/BackendTemplate.Api.Tests/BackendTemplate.Api.Tests.csproj `
  backend/tests/BackendTemplate.Domain.Tests/BackendTemplate.Domain.Tests.csproj `
  backend/tests/BackendTemplate.Infrastructure.Tests/BackendTemplate.Infrastructure.Tests.csproj `
  backend/tests/BackendTemplate.Testing.Common/BackendTemplate.Testing.Common.csproj
```

- [ ] **Step 4: Restore to catch package resolution errors early**

```powershell
dotnet restore backend/BackendTemplate.sln
```
Expected: all packages download without error.

- [ ] **Step 5: Commit**

```bash
git add backend/
git commit -m "chore: scaffold solution structure and project files"
```

---

## Task 2: Domain project

**Files:**
- Create: `backend/src/BackendTemplate.Domain/Common/Result.cs`
- Create: `backend/src/BackendTemplate.Domain/Common/TodoItemId.cs`
- Create: `backend/src/BackendTemplate.Domain/Entities/TodoItem.cs`
- Create: `backend/src/BackendTemplate.Domain/Ports/IEmailSender.cs`

- [ ] **Step 1: Write `Common/Result.cs`**

```csharp
namespace BackendTemplate.Domain.Common;

public enum ErrorKind { Validation, NotFound, Conflict }

public readonly struct Unit
{
    public static readonly Unit Value = default;
}

public sealed class Result<T>
{
    private readonly T? _value;
    private readonly string? _error;

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public ErrorKind Kind { get; }

    public T Value => IsSuccess ? _value! : throw new InvalidOperationException("Cannot access Value on a failed Result.");
    public string Error => IsFailure ? _error! : throw new InvalidOperationException("Cannot access Error on a successful Result.");

    private Result(T value) { IsSuccess = true; _value = value; }
    private Result(string error, ErrorKind kind) { IsSuccess = false; _error = error; Kind = kind; }

    public static Result<T> Success(T value) => new(value);
    public static Result<T> Failure(string error, ErrorKind kind = ErrorKind.Validation) => new(error, kind);
}
```

- [ ] **Step 2: Write `Common/TodoItemId.cs`**

```csharp
namespace BackendTemplate.Domain.Common;

public record struct TodoItemId(Guid Value);
```

- [ ] **Step 3: Write `Entities/TodoItem.cs`**

```csharp
using BackendTemplate.Domain.Common;

namespace BackendTemplate.Domain.Entities;

public enum TodoStatus { Pending, InProgress, Done }

public class TodoItem
{
    public TodoItemId Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public TodoStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private TodoItem() { }

    public static TodoItem Create(string title) => new()
    {
        Id = new TodoItemId(Guid.NewGuid()),
        Title = title,
        Status = TodoStatus.Pending,
        CreatedAt = DateTime.UtcNow
    };

    public void UpdateTitle(string title) => Title = title;
    public void UpdateStatus(TodoStatus status) => Status = status;
}
```

- [ ] **Step 4: Write `Ports/IEmailSender.cs`**

```csharp
namespace BackendTemplate.Domain.Ports;

public interface IEmailSender
{
    Task SendAsync(string to, string subject, string body, CancellationToken ct = default);
}
```

- [ ] **Step 5: Build domain project**

```powershell
dotnet build backend/src/BackendTemplate.Domain/BackendTemplate.Domain.csproj
```
Expected: `Build succeeded.`

- [ ] **Step 6: Commit**

```bash
git add backend/src/BackendTemplate.Domain/
git commit -m "feat: add domain entities, Result<T>, and port interfaces"
```

---

## Task 3: Infrastructure project

**Files:**
- Create: `backend/src/BackendTemplate.Infrastructure/Persistence/Converters/StronglyTypedIdConverter.cs`
- Create: `backend/src/BackendTemplate.Infrastructure/Persistence/AppDbContext.cs`
- Create: `backend/src/BackendTemplate.Infrastructure/Persistence/Configurations/TodoItemConfiguration.cs`
- Create: `backend/src/BackendTemplate.Infrastructure/Extensions/QueryableExtensions.cs`
- Create: `backend/src/BackendTemplate.Infrastructure/Extensions/ServiceCollectionExtensions.cs`

- [ ] **Step 1: Write `Persistence/Converters/StronglyTypedIdConverter.cs`**

```csharp
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BackendTemplate.Infrastructure.Persistence.Converters;

public sealed class StronglyTypedIdConverter<TId> : ValueConverter<TId, Guid>
    where TId : struct
{
    private static readonly Func<TId, Guid> ToGuid;
    private static readonly Func<Guid, TId> FromGuid;

    static StronglyTypedIdConverter()
    {
        var prop = typeof(TId).GetProperty("Value")
            ?? throw new InvalidOperationException($"{typeof(TId).Name} has no Value property.");
        ToGuid = id => (Guid)prop.GetValue(id)!;
        FromGuid = g => (TId)Activator.CreateInstance(typeof(TId), g)!;
    }

    public StronglyTypedIdConverter() : base(id => ToGuid(id), g => FromGuid(g)) { }
}
```

- [ ] **Step 2: Write `Persistence/AppDbContext.cs`**

```csharp
using BackendTemplate.Domain.Common;
using BackendTemplate.Domain.Entities;
using BackendTemplate.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;

namespace BackendTemplate.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<TodoItem> TodoItems => Set<TodoItem>();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        var idTypes = typeof(TodoItemId).Assembly
            .GetTypes()
            .Where(t => t.IsValueType
                && !t.IsEnum
                && !t.IsPrimitive
                && !t.IsGenericType
                && t.GetProperty("Value") is { PropertyType.FullName: "System.Guid" }
                && t.GetProperties().Length == 1);

        foreach (var idType in idTypes)
        {
            var converterType = typeof(StronglyTypedIdConverter<>).MakeGenericType(idType);
            configurationBuilder.Properties(idType).HaveConversion(converterType);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
```

- [ ] **Step 3: Write `Persistence/Configurations/TodoItemConfiguration.cs`**

```csharp
using BackendTemplate.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackendTemplate.Infrastructure.Persistence.Configurations;

public class TodoItemConfiguration : IEntityTypeConfiguration<TodoItem>
{
    public void Configure(EntityTypeBuilder<TodoItem> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Title).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Status).IsRequired()
            .HasConversion<string>();
        builder.Property(x => x.CreatedAt).IsRequired();
    }
}
```

- [ ] **Step 4: Write `Extensions/QueryableExtensions.cs`**

```csharp
using Microsoft.EntityFrameworkCore;

namespace BackendTemplate.Infrastructure.Extensions;

public static class QueryableExtensions
{
    public static async Task<(IReadOnlyList<T> Items, int Total)> ToPagedAsync<T>(
        this IQueryable<T> query,
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        var total = await query.CountAsync(ct);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }
}
```

- [ ] **Step 5: Write `Extensions/ServiceCollectionExtensions.cs`**

```csharp
using BackendTemplate.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BackendTemplate.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Default")));

        return services;
    }
}
```

- [ ] **Step 6: Build infrastructure project**

```powershell
dotnet build backend/src/BackendTemplate.Infrastructure/BackendTemplate.Infrastructure.csproj
```
Expected: `Build succeeded.`

- [ ] **Step 7: Commit**

```bash
git add backend/src/BackendTemplate.Infrastructure/
git commit -m "feat: add infrastructure — DbContext, EF config, ToPagedAsync, AddInfrastructure"
```

---

## Task 4: API Common layer

**Files:**
- Create: all files under `backend/src/BackendTemplate.Api/Common/`

- [ ] **Step 1: Write lifetime marker interfaces**

`backend/src/BackendTemplate.Api/Common/IScopedService.cs`:
```csharp
namespace BackendTemplate.Api.Common;

public interface IScopedService { }
```

`backend/src/BackendTemplate.Api/Common/ISingletonService.cs`:
```csharp
namespace BackendTemplate.Api.Common;

public interface ISingletonService { }
```

`backend/src/BackendTemplate.Api/Common/ITransientService.cs`:
```csharp
namespace BackendTemplate.Api.Common;

public interface ITransientService { }
```

- [ ] **Step 2: Write `Common/IEndpoint.cs`**

```csharp
namespace BackendTemplate.Api.Common;

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}
```

- [ ] **Step 3: Write `Common/EndpointExtensions.cs`**

```csharp
using System.Reflection;
using BackendTemplate.Api.Common;

namespace BackendTemplate.Api.Common;

public static class EndpointExtensions
{
    public static void MapEndpoints(this IEndpointRouteBuilder app, Assembly assembly)
    {
        var endpointTypes = assembly.GetTypes()
            .Where(t => typeof(IEndpoint).IsAssignableFrom(t)
                && t is { IsAbstract: false, IsInterface: false });

        foreach (var type in endpointTypes)
        {
            var endpoint = (IEndpoint)Activator.CreateInstance(type)!;
            endpoint.MapEndpoint(app);
        }
    }
}
```

- [ ] **Step 4: Write `Common/Page.cs`**

```csharp
namespace BackendTemplate.Api.Common;

public record Page<T>(IReadOnlyList<T> Items, int Total, int Page, int PageSize);
```

- [ ] **Step 5: Write `Common/ValidationFilter.cs`**

```csharp
using FluentValidation;

namespace BackendTemplate.Api.Common;

public sealed class ValidationFilter<TRequest> : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var validator = context.HttpContext.RequestServices.GetService<IValidator<TRequest>>();
        if (validator is null)
            throw new InvalidOperationException(
                $"No IValidator<{typeof(TRequest).Name}> registered. " +
                "Add a Validator class or remove .WithValidation<TRequest>().");

        var request = context.Arguments.OfType<TRequest>().FirstOrDefault();
        if (request is null)
            return await next(context);

        var result = await validator.ValidateAsync(request, context.HttpContext.RequestAborted);
        if (!result.IsValid)
            return TypedResults.ValidationProblem(result.ToDictionary());

        return await next(context);
    }
}

public static class ValidationExtensions
{
    public static RouteHandlerBuilder WithValidation<TRequest>(this RouteHandlerBuilder builder)
        => builder.AddEndpointFilter<ValidationFilter<TRequest>>();
}
```

- [ ] **Step 6: Write `Common/ResultExtensions.cs`**

```csharp
using BackendTemplate.Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace BackendTemplate.Api.Common;

public static class ResultExtensions
{
    public static IResult ToHttpResult<T, TResponse>(
        this Result<T> result,
        Func<T, TResponse> mapper,
        Func<TResponse, string>? location = null)
    {
        if (result.IsFailure)
            return ToProblem(result.Error, result.Kind);

        var response = mapper(result.Value);
        return location is not null
            ? TypedResults.Created(location(response), response)
            : TypedResults.Ok(response);
    }

    public static IResult ToHttpResult(this Result<Unit> result)
        => result.IsFailure
            ? ToProblem(result.Error, result.Kind)
            : TypedResults.NoContent();

    private static IResult ToProblem(string error, ErrorKind kind)
    {
        var (status, title) = kind switch
        {
            ErrorKind.NotFound => (StatusCodes.Status404NotFound, "Not Found"),
            ErrorKind.Conflict => (StatusCodes.Status409Conflict, "Conflict"),
            _ => (StatusCodes.Status422UnprocessableEntity, "Validation Error")
        };

        return TypedResults.Problem(
            detail: error,
            statusCode: status,
            title: title,
            type: "https://tools.ietf.org/html/rfc7807");
    }
}
```

- [ ] **Step 7: Write `Common/GlobalExceptionHandler.cs`**

```csharp
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace BackendTemplate.Api.Common;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext context,
        Exception exception,
        CancellationToken ct)
    {
        logger.LogError(exception, "Unhandled exception");

        var problem = new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7807",
            Title = "Internal Server Error",
            Status = StatusCodes.Status500InternalServerError,
            Detail = "An unexpected error occurred."
        };

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsJsonAsync(problem, ct);
        return true;
    }
}
```

- [ ] **Step 8: Commit (common layer — no build yet, Api needs Program.cs first)**

```bash
git add backend/src/BackendTemplate.Api/Common/
git commit -m "feat: add API common layer — endpoint infrastructure, validation filter, Result extensions"
```

---

## Task 5: Program.cs and app configuration

**Files:**
- Create: `backend/src/BackendTemplate.Api/Program.cs`
- Create: `backend/src/BackendTemplate.Api/appsettings.json`
- Create: `backend/src/BackendTemplate.Api/appsettings.Development.json`

- [ ] **Step 1: Write `appsettings.json`**

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      { "Name": "Console" }
    ]
  },
  "ConnectionStrings": {
    "Default": ""
  },
  "AllowedHosts": "*"
}
```

- [ ] **Step 2: Write `appsettings.Development.json`**

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Debug"
    }
  }
}
```

- [ ] **Step 3: Write `Program.cs`**

```csharp
using BackendTemplate.Api.Common;
using BackendTemplate.Infrastructure.Extensions;
using BackendTemplate.Infrastructure.Persistence;
using FluentValidation;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, config) =>
    config.ReadFrom.Configuration(ctx.Configuration));

builder.Services.AddOpenApi();

builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

builder.Services.Scan(scan => scan
    .FromAssemblyOf<Program>()
    .AddClasses(c => c.AssignableTo<IScopedService>()).AsSelf().WithScopedLifetime()
    .AddClasses(c => c.AssignableTo<ISingletonService>()).AsSelf().WithSingletonLifetime()
    .AddClasses(c => c.AssignableTo<ITransientService>()).AsSelf().WithTransientLifetime());

builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseExceptionHandler();

app.MapOpenApi();
app.MapScalarApiReference();

app.MapHealthChecks("/health/live");
app.MapHealthChecks("/health/ready");

app.MapEndpoints(typeof(Program).Assembly);

app.Run();

public partial class Program { }
```

- [ ] **Step 4: Build API project (will succeed even with no features yet)**

```powershell
dotnet build backend/src/BackendTemplate.Api/BackendTemplate.Api.csproj
```
Expected: `Build succeeded.`

- [ ] **Step 5: Commit**

```bash
git add backend/src/BackendTemplate.Api/Program.cs backend/src/BackendTemplate.Api/appsettings*.json
git commit -m "feat: wire up Program.cs with Serilog, OpenAPI, health checks, DI scanning"
```

---

## Task 6: TodoItems shared types

**Files:**
- Create: `backend/src/BackendTemplate.Api/Features/TodoItems/TodoItemResponse.cs`
- Create: `backend/src/BackendTemplate.Api/Features/TodoItems/TodoItemMapper.cs`

- [ ] **Step 1: Write `TodoItemResponse.cs`**

```csharp
namespace BackendTemplate.Api.Features.TodoItems;

public record TodoItemResponse(Guid Id, string Title, string Status, DateTime CreatedAt);
```

- [ ] **Step 2: Write `TodoItemMapper.cs`**

```csharp
using BackendTemplate.Domain.Common;
using BackendTemplate.Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace BackendTemplate.Api.Features.TodoItems;

[Mapper]
public static partial class TodoItemMapper
{
    [MapProperty($"{nameof(TodoItem.Id)}.{nameof(TodoItemId.Value)}", nameof(TodoItemResponse.Id))]
    public static partial TodoItemResponse ToResponse(TodoItem source);
}
```

Note: Mapperly maps `TodoStatus` → `string` automatically via enum name. The `[MapProperty]` flattens `Id.Value` → `Id`.

- [ ] **Step 3: Commit**

```bash
git add backend/src/BackendTemplate.Api/Features/
git commit -m "feat: add TodoItemResponse DTO and Mapperly mapper"
```

---

## Task 7: CreateTodoItem slice

**Files:**
- Create: `backend/src/BackendTemplate.Api/Features/TodoItems/Commands/CreateTodoItem/Request.cs`
- Create: `backend/src/BackendTemplate.Api/Features/TodoItems/Commands/CreateTodoItem/Validator.cs`
- Create: `backend/src/BackendTemplate.Api/Features/TodoItems/Commands/CreateTodoItem/Handler.cs`
- Create: `backend/src/BackendTemplate.Api/Features/TodoItems/Commands/CreateTodoItem/Endpoint.cs`

- [ ] **Step 1: Write `Request.cs`**

```csharp
namespace BackendTemplate.Api.Features.TodoItems.Commands.CreateTodoItem;

public record CreateTodoItemRequest(string Title);
```

- [ ] **Step 2: Write `Validator.cs`**

```csharp
using FluentValidation;

namespace BackendTemplate.Api.Features.TodoItems.Commands.CreateTodoItem;

public class CreateTodoItemValidator : AbstractValidator<CreateTodoItemRequest>
{
    public CreateTodoItemValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
    }
}
```

- [ ] **Step 3: Write `Handler.cs`**

```csharp
using BackendTemplate.Api.Common;
using BackendTemplate.Domain.Common;
using BackendTemplate.Domain.Entities;
using BackendTemplate.Infrastructure.Persistence;

namespace BackendTemplate.Api.Features.TodoItems.Commands.CreateTodoItem;

public sealed class CreateTodoItemHandler(AppDbContext db) : IScopedService
{
    public async Task<Result<TodoItem>> HandleAsync(
        CreateTodoItemRequest request,
        CancellationToken ct = default)
    {
        var item = TodoItem.Create(request.Title);
        db.TodoItems.Add(item);
        await db.SaveChangesAsync(ct);
        return Result<TodoItem>.Success(item);
    }
}
```

- [ ] **Step 4: Write `Endpoint.cs`**

```csharp
using BackendTemplate.Api.Common;
using BackendTemplate.Api.Features.TodoItems;

namespace BackendTemplate.Api.Features.TodoItems.Commands.CreateTodoItem;

public class CreateTodoItemEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/todo-items", async (
            CreateTodoItemRequest request,
            CreateTodoItemHandler handler,
            CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(request, ct);
            return result.ToHttpResult(
                TodoItemMapper.ToResponse,
                r => $"/todo-items/{r.Id}");
        })
        .WithValidation<CreateTodoItemRequest>()
        .WithTags("TodoItems")
        .WithName("CreateTodoItem");
    }
}
```

- [ ] **Step 5: Build**

```powershell
dotnet build backend/src/BackendTemplate.Api/BackendTemplate.Api.csproj
```
Expected: `Build succeeded.`

- [ ] **Step 6: Commit**

```bash
git add backend/src/BackendTemplate.Api/Features/TodoItems/Commands/CreateTodoItem/
git commit -m "feat: add CreateTodoItem slice"
```

---

## Task 8: GetTodoItem slice

**Files:**
- Create: `backend/src/BackendTemplate.Api/Features/TodoItems/Queries/GetTodoItem/Handler.cs`
- Create: `backend/src/BackendTemplate.Api/Features/TodoItems/Queries/GetTodoItem/Endpoint.cs`

- [ ] **Step 1: Write `Handler.cs`**

```csharp
using BackendTemplate.Api.Common;
using BackendTemplate.Api.Features.TodoItems;
using BackendTemplate.Domain.Common;
using BackendTemplate.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BackendTemplate.Api.Features.TodoItems.Queries.GetTodoItem;

public sealed class GetTodoItemHandler(AppDbContext db) : IScopedService
{
    public async Task<Result<TodoItemResponse>> HandleAsync(
        Guid id,
        CancellationToken ct = default)
    {
        var item = await db.TodoItems.FindAsync([new TodoItemId(id)], ct);
        if (item is null)
            return Result<TodoItemResponse>.Failure(
                $"Todo item {id} was not found.", ErrorKind.NotFound);

        return Result<TodoItemResponse>.Success(TodoItemMapper.ToResponse(item));
    }
}
```

- [ ] **Step 2: Write `Endpoint.cs`**

```csharp
using BackendTemplate.Api.Common;

namespace BackendTemplate.Api.Features.TodoItems.Queries.GetTodoItem;

public class GetTodoItemEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/todo-items/{id:guid}", async (
            Guid id,
            GetTodoItemHandler handler,
            CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(id, ct);
            return result.ToHttpResult(x => x);
        })
        .WithTags("TodoItems")
        .WithName("GetTodoItem");
    }
}
```

- [ ] **Step 3: Build and commit**

```powershell
dotnet build backend/src/BackendTemplate.Api/BackendTemplate.Api.csproj
```

```bash
git add backend/src/BackendTemplate.Api/Features/TodoItems/Queries/GetTodoItem/
git commit -m "feat: add GetTodoItem slice"
```

---

## Task 9: ListTodoItems slice

**Files:**
- Create: `backend/src/BackendTemplate.Api/Features/TodoItems/Queries/ListTodoItems/Request.cs`
- Create: `backend/src/BackendTemplate.Api/Features/TodoItems/Queries/ListTodoItems/Validator.cs`
- Create: `backend/src/BackendTemplate.Api/Features/TodoItems/Queries/ListTodoItems/Handler.cs`
- Create: `backend/src/BackendTemplate.Api/Features/TodoItems/Queries/ListTodoItems/Endpoint.cs`

- [ ] **Step 1: Write `Request.cs`**

```csharp
namespace BackendTemplate.Api.Features.TodoItems.Queries.ListTodoItems;

public record ListTodoItemsRequest(int Page = 1, int PageSize = 20);
```

- [ ] **Step 2: Write `Validator.cs`**

```csharp
using FluentValidation;

namespace BackendTemplate.Api.Features.TodoItems.Queries.ListTodoItems;

public class ListTodoItemsValidator : AbstractValidator<ListTodoItemsRequest>
{
    public ListTodoItemsValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
```

- [ ] **Step 3: Write `Handler.cs`**

```csharp
using BackendTemplate.Api.Common;
using BackendTemplate.Api.Features.TodoItems;
using BackendTemplate.Domain.Common;
using BackendTemplate.Infrastructure.Extensions;
using BackendTemplate.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BackendTemplate.Api.Features.TodoItems.Queries.ListTodoItems;

public sealed class ListTodoItemsHandler(AppDbContext db) : IScopedService
{
    public async Task<Result<Page<TodoItemResponse>>> HandleAsync(
        ListTodoItemsRequest request,
        CancellationToken ct = default)
    {
        var (items, total) = await db.TodoItems
            .OrderBy(x => x.CreatedAt)
            .Select(x => new TodoItemResponse(x.Id.Value, x.Title, x.Status.ToString(), x.CreatedAt))
            .ToPagedAsync(request.Page, request.PageSize, ct);

        return Result<Page<TodoItemResponse>>.Success(
            new Page<TodoItemResponse>(items, total, request.Page, request.PageSize));
    }
}
```

- [ ] **Step 4: Write `Endpoint.cs`**

```csharp
using BackendTemplate.Api.Common;

namespace BackendTemplate.Api.Features.TodoItems.Queries.ListTodoItems;

public class ListTodoItemsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/todo-items", async (
            [AsParameters] ListTodoItemsRequest request,
            ListTodoItemsHandler handler,
            CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(request, ct);
            return result.ToHttpResult(p => p);
        })
        .WithValidation<ListTodoItemsRequest>()
        .WithTags("TodoItems")
        .WithName("ListTodoItems");
    }
}
```

- [ ] **Step 5: Build and commit**

```powershell
dotnet build backend/src/BackendTemplate.Api/BackendTemplate.Api.csproj
```

```bash
git add backend/src/BackendTemplate.Api/Features/TodoItems/Queries/ListTodoItems/
git commit -m "feat: add ListTodoItems slice with pagination"
```

---

## Task 10: UpdateTodoItem (PATCH) slice

**Files:**
- Create: `backend/src/BackendTemplate.Api/Features/TodoItems/Commands/UpdateTodoItem/Request.cs`
- Create: `backend/src/BackendTemplate.Api/Features/TodoItems/Commands/UpdateTodoItem/Validator.cs`
- Create: `backend/src/BackendTemplate.Api/Features/TodoItems/Commands/UpdateTodoItem/Handler.cs`
- Create: `backend/src/BackendTemplate.Api/Features/TodoItems/Commands/UpdateTodoItem/Endpoint.cs`

- [ ] **Step 1: Write `Request.cs`**

JSON Merge Patch (RFC 7396): nullable fields; `null` sent by client = validation error; field omitted = unchanged.

```csharp
namespace BackendTemplate.Api.Features.TodoItems.Commands.UpdateTodoItem;

public record UpdateTodoItemRequest(string? Title, string? Status);
```

- [ ] **Step 2: Write `Validator.cs`**

```csharp
using BackendTemplate.Domain.Entities;
using FluentValidation;

namespace BackendTemplate.Api.Features.TodoItems.Commands.UpdateTodoItem;

public class UpdateTodoItemValidator : AbstractValidator<UpdateTodoItemRequest>
{
    public UpdateTodoItemValidator()
    {
        RuleFor(x => x)
            .Must(x => x.Title is not null || x.Status is not null)
            .WithName("request")
            .WithMessage("At least one field must be provided.");

        When(x => x.Title is not null, () =>
            RuleFor(x => x.Title!).NotEmpty().MaximumLength(200));

        When(x => x.Status is not null, () =>
            RuleFor(x => x.Status!)
                .Must(s => Enum.TryParse<TodoStatus>(s, ignoreCase: true, out _))
                .WithMessage($"Status must be one of: {string.Join(", ", Enum.GetNames<TodoStatus>())}"));
    }
}
```

- [ ] **Step 3: Write `Handler.cs`**

```csharp
using BackendTemplate.Api.Common;
using BackendTemplate.Domain.Common;
using BackendTemplate.Domain.Entities;
using BackendTemplate.Infrastructure.Persistence;

namespace BackendTemplate.Api.Features.TodoItems.Commands.UpdateTodoItem;

public sealed class UpdateTodoItemHandler(AppDbContext db) : IScopedService
{
    public async Task<Result<TodoItem>> HandleAsync(
        Guid id,
        UpdateTodoItemRequest request,
        CancellationToken ct = default)
    {
        var item = await db.TodoItems.FindAsync([new TodoItemId(id)], ct);
        if (item is null)
            return Result<TodoItem>.Failure(
                $"Todo item {id} was not found.", ErrorKind.NotFound);

        if (request.Title is not null)
            item.UpdateTitle(request.Title);

        if (request.Status is not null)
        {
            if (!Enum.TryParse<TodoStatus>(request.Status, ignoreCase: true, out var status))
                return Result<TodoItem>.Failure($"Invalid status: {request.Status}");
            item.UpdateStatus(status);
        }

        await db.SaveChangesAsync(ct);
        return Result<TodoItem>.Success(item);
    }
}
```

- [ ] **Step 4: Write `Endpoint.cs`**

```csharp
using BackendTemplate.Api.Common;
using BackendTemplate.Api.Features.TodoItems;

namespace BackendTemplate.Api.Features.TodoItems.Commands.UpdateTodoItem;

public class UpdateTodoItemEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("/todo-items/{id:guid}", async (
            Guid id,
            UpdateTodoItemRequest request,
            UpdateTodoItemHandler handler,
            CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(id, request, ct);
            return result.ToHttpResult(TodoItemMapper.ToResponse);
        })
        .WithValidation<UpdateTodoItemRequest>()
        .WithTags("TodoItems")
        .WithName("UpdateTodoItem");
    }
}
```

- [ ] **Step 5: Build and commit**

```powershell
dotnet build backend/src/BackendTemplate.Api/BackendTemplate.Api.csproj
```

```bash
git add backend/src/BackendTemplate.Api/Features/TodoItems/Commands/UpdateTodoItem/
git commit -m "feat: add UpdateTodoItem PATCH slice (JSON Merge Patch)"
```

---

## Task 11: DeleteTodoItem slice

**Files:**
- Create: `backend/src/BackendTemplate.Api/Features/TodoItems/Commands/DeleteTodoItem/Handler.cs`
- Create: `backend/src/BackendTemplate.Api/Features/TodoItems/Commands/DeleteTodoItem/Endpoint.cs`

- [ ] **Step 1: Write `Handler.cs`**

```csharp
using BackendTemplate.Api.Common;
using BackendTemplate.Domain.Common;
using BackendTemplate.Infrastructure.Persistence;

namespace BackendTemplate.Api.Features.TodoItems.Commands.DeleteTodoItem;

public sealed class DeleteTodoItemHandler(AppDbContext db) : IScopedService
{
    public async Task<Result<Unit>> HandleAsync(Guid id, CancellationToken ct = default)
    {
        var item = await db.TodoItems.FindAsync([new TodoItemId(id)], ct);
        if (item is null)
            return Result<Unit>.Failure(
                $"Todo item {id} was not found.", ErrorKind.NotFound);

        db.TodoItems.Remove(item);
        await db.SaveChangesAsync(ct);
        return Result<Unit>.Success(Unit.Value);
    }
}
```

- [ ] **Step 2: Write `Endpoint.cs`**

```csharp
using BackendTemplate.Api.Common;

namespace BackendTemplate.Api.Features.TodoItems.Commands.DeleteTodoItem;

public class DeleteTodoItemEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/todo-items/{id:guid}", async (
            Guid id,
            DeleteTodoItemHandler handler,
            CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(id, ct);
            return result.ToHttpResult();
        })
        .WithTags("TodoItems")
        .WithName("DeleteTodoItem");
    }
}
```

- [ ] **Step 3: Full solution build**

```powershell
dotnet build backend/BackendTemplate.sln
```
Expected: `Build succeeded.`

- [ ] **Step 4: Commit**

```bash
git add backend/src/BackendTemplate.Api/Features/TodoItems/Commands/DeleteTodoItem/
git commit -m "feat: add DeleteTodoItem slice"
```

---

## Task 12: Testing.Common — DatabaseFixture and TodoItemBuilder

**Files:**
- Create: `backend/tests/BackendTemplate.Testing.Common/DatabaseFixture.cs`
- Create: `backend/tests/BackendTemplate.Testing.Common/Builders/TodoItemBuilder.cs`

- [ ] **Step 1: Write `DatabaseFixture.cs`**

```csharp
using BackendTemplate.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Xunit;

namespace BackendTemplate.Testing.Common;

public class DatabaseFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .Build();

    public string ConnectionString { get; private set; } = string.Empty;

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        ConnectionString = _container.GetConnectionString();

        await using var db = CreateDbContext();
        await db.Database.MigrateAsync();
    }

    public async Task DisposeAsync() => await _container.DisposeAsync();

    public AppDbContext CreateDbContext()
        => new AppDbContext(new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(ConnectionString)
            .Options);
}

[CollectionDefinition("Database")]
public class DatabaseCollection : ICollectionFixture<DatabaseFixture> { }
```

- [ ] **Step 2: Write `Builders/TodoItemBuilder.cs`**

```csharp
using BackendTemplate.Domain.Common;
using BackendTemplate.Domain.Entities;

namespace BackendTemplate.Testing.Common.Builders;

public class TodoItemBuilder
{
    private string _title = "Default Todo Item";
    private TodoStatus _status = TodoStatus.Pending;

    public static TodoItemBuilder Default() => new();

    public static TodoItemBuilder InProgress() => new TodoItemBuilder()
        .WithStatus(TodoStatus.InProgress);

    public static TodoItemBuilder Done() => new TodoItemBuilder()
        .WithStatus(TodoStatus.Done);

    public TodoItemBuilder WithTitle(string title)
    {
        _title = title;
        return this;
    }

    public TodoItemBuilder WithStatus(TodoStatus status)
    {
        _status = status;
        return this;
    }

    public TodoItem Build()
    {
        var item = TodoItem.Create(_title);
        if (_status != TodoStatus.Pending)
            item.UpdateStatus(_status);
        return item;
    }
}
```

- [ ] **Step 3: Build Testing.Common**

```powershell
dotnet build backend/tests/BackendTemplate.Testing.Common/BackendTemplate.Testing.Common.csproj
```
Expected: `Build succeeded.`

- [ ] **Step 4: Commit**

```bash
git add backend/tests/BackendTemplate.Testing.Common/
git commit -m "feat: add DatabaseFixture and TodoItemBuilder in Testing.Common"
```

---

## Task 13: Domain unit tests

**Files:**
- Create: `backend/tests/BackendTemplate.Domain.Tests/Common/ResultTests.cs`

- [ ] **Step 1: Write `ResultTests.cs`**

```csharp
using BackendTemplate.Domain.Common;
using Xunit;

namespace BackendTemplate.Domain.Tests.Common;

public class ResultTests
{
    [Fact]
    public void Success_GivenValue_ThenIsSuccessTrue()
    {
        var result = Result<int>.Success(42);
        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Equal(42, result.Value);
    }

    [Fact]
    public void Failure_GivenError_ThenIsFailureTrue()
    {
        var result = Result<int>.Failure("Something went wrong");
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal("Something went wrong", result.Error);
    }

    [Fact]
    public void Failure_WhenNoKindProvided_ThenDefaultsToValidation()
    {
        var result = Result<int>.Failure("error");
        Assert.Equal(ErrorKind.Validation, result.Kind);
    }

    [Fact]
    public void Failure_GivenNotFoundKind_ThenKindIsNotFound()
    {
        var result = Result<int>.Failure("not found", ErrorKind.NotFound);
        Assert.Equal(ErrorKind.NotFound, result.Kind);
    }

    [Fact]
    public void Value_WhenFailure_ThenThrowsInvalidOperationException()
    {
        var result = Result<int>.Failure("error");
        Assert.Throws<InvalidOperationException>(() => _ = result.Value);
    }

    [Fact]
    public void Error_WhenSuccess_ThenThrowsInvalidOperationException()
    {
        var result = Result<int>.Success(42);
        Assert.Throws<InvalidOperationException>(() => _ = result.Error);
    }

    [Fact]
    public void Kind_WhenSuccess_ThenThrowsInvalidOperationException()
    {
        var result = Result<int>.Success(42);
        Assert.Throws<InvalidOperationException>(() => _ = result.Kind);
    }
}
```

Wait — the spec shows `Kind` throws on success but our `Result<T>` above doesn't enforce that. Update `Result<T>` in `Common/Result.cs` to add `Kind` guard:

The `Kind` property in `Result<T>` as written above returns `default(ErrorKind)` on success (since `Kind` is not set). To match the spec ("throws if success"), update `Result<T>`:

```csharp
public ErrorKind Kind => IsFailure ? _kind : throw new InvalidOperationException("Cannot access Kind on a successful Result.");

private readonly ErrorKind _kind;

private Result(string error, ErrorKind kind) { IsSuccess = false; _error = error; _kind = kind; }
```

Update `backend/src/BackendTemplate.Domain/Common/Result.cs` to use a private backing field `_kind` instead of the auto-property `Kind { get; }`.

- [ ] **Step 2: Update `Result<T>` to make `Kind` throw on success**

Edit `backend/src/BackendTemplate.Domain/Common/Result.cs` — replace the `Kind` property and add a `_kind` field:

```csharp
namespace BackendTemplate.Domain.Common;

public enum ErrorKind { Validation, NotFound, Conflict }

public readonly struct Unit
{
    public static readonly Unit Value = default;
}

public sealed class Result<T>
{
    private readonly T? _value;
    private readonly string? _error;
    private readonly ErrorKind _kind;

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    public T Value => IsSuccess ? _value! : throw new InvalidOperationException("Cannot access Value on a failed Result.");
    public string Error => IsFailure ? _error! : throw new InvalidOperationException("Cannot access Error on a successful Result.");
    public ErrorKind Kind => IsFailure ? _kind : throw new InvalidOperationException("Cannot access Kind on a successful Result.");

    private Result(T value) { IsSuccess = true; _value = value; }
    private Result(string error, ErrorKind kind) { IsSuccess = false; _error = error; _kind = kind; }

    public static Result<T> Success(T value) => new(value);
    public static Result<T> Failure(string error, ErrorKind kind = ErrorKind.Validation) => new(error, kind);
}
```

- [ ] **Step 3: Run domain tests**

```powershell
dotnet test backend/tests/BackendTemplate.Domain.Tests/BackendTemplate.Domain.Tests.csproj --logger "console;verbosity=normal"
```
Expected: all 7 tests pass.

- [ ] **Step 4: Commit**

```bash
git add backend/tests/BackendTemplate.Domain.Tests/ backend/src/BackendTemplate.Domain/Common/Result.cs
git commit -m "test: add Result<T> unit tests; fix Kind to throw on success"
```

---

## Task 14: Infrastructure tests (QueryableExtensions)

**Files:**
- Create: `backend/tests/BackendTemplate.Infrastructure.Tests/Extensions/QueryableExtensionsTests.cs`

- [ ] **Step 1: Write `QueryableExtensionsTests.cs`**

```csharp
using BackendTemplate.Infrastructure.Extensions;
using BackendTemplate.Infrastructure.Persistence;
using BackendTemplate.Testing.Common;
using BackendTemplate.Testing.Common.Builders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Xunit;

namespace BackendTemplate.Infrastructure.Tests.Extensions;

[Collection("Database")]
public class QueryableExtensionsTests : IAsyncDisposable
{
    private readonly AppDbContext _db;
    private readonly IDbContextTransaction _tx;

    public QueryableExtensionsTests(DatabaseFixture fixture)
    {
        _db = fixture.CreateDbContext();
        _tx = _db.Database.BeginTransaction();
    }

    [Fact]
    public async Task ToPagedAsync_GivenThreeItems_ReturnsCorrectTotalCount()
    {
        _db.TodoItems.AddRange(
            TodoItemBuilder.Default().WithTitle("A").Build(),
            TodoItemBuilder.Default().WithTitle("B").Build(),
            TodoItemBuilder.Default().WithTitle("C").Build());
        await _db.SaveChangesAsync();

        var (_, total) = await _db.TodoItems.ToPagedAsync(1, 10);

        Assert.Equal(3, total);
    }

    [Fact]
    public async Task ToPagedAsync_GivenPage1WithPageSize2_ReturnsTwoItems()
    {
        _db.TodoItems.AddRange(
            TodoItemBuilder.Default().WithTitle("A").Build(),
            TodoItemBuilder.Default().WithTitle("B").Build(),
            TodoItemBuilder.Default().WithTitle("C").Build());
        await _db.SaveChangesAsync();

        var (items, _) = await _db.TodoItems.OrderBy(x => x.Title).ToPagedAsync(1, 2);

        Assert.Equal(2, items.Count);
        Assert.Equal("A", items[0].Title);
        Assert.Equal("B", items[1].Title);
    }

    [Fact]
    public async Task ToPagedAsync_GivenPage2WithPageSize2_ReturnsLastItem()
    {
        _db.TodoItems.AddRange(
            TodoItemBuilder.Default().WithTitle("A").Build(),
            TodoItemBuilder.Default().WithTitle("B").Build(),
            TodoItemBuilder.Default().WithTitle("C").Build());
        await _db.SaveChangesAsync();

        var (items, total) = await _db.TodoItems.OrderBy(x => x.Title).ToPagedAsync(2, 2);

        Assert.Single(items);
        Assert.Equal(3, total);
        Assert.Equal("C", items[0].Title);
    }

    public async ValueTask DisposeAsync()
    {
        await _tx.RollbackAsync();
        await _tx.DisposeAsync();
        await _db.DisposeAsync();
    }
}
```

- [ ] **Step 2: Run infrastructure tests**

```powershell
dotnet test backend/tests/BackendTemplate.Infrastructure.Tests/BackendTemplate.Infrastructure.Tests.csproj --logger "console;verbosity=normal"
```
Expected: all 3 tests pass (requires Docker for Testcontainers).

- [ ] **Step 3: Commit**

```bash
git add backend/tests/BackendTemplate.Infrastructure.Tests/
git commit -m "test: add QueryableExtensions integration tests"
```

---

## Task 15: API handler integration tests

**Files:**
- Create all 5 handler test files under `backend/tests/BackendTemplate.Api.Tests/Features/TodoItems/`

- [ ] **Step 1: Write `Commands/CreateTodoItemHandlerTests.cs`**

```csharp
using BackendTemplate.Api.Features.TodoItems.Commands.CreateTodoItem;
using BackendTemplate.Domain.Entities;
using BackendTemplate.Infrastructure.Persistence;
using BackendTemplate.Testing.Common;
using Microsoft.EntityFrameworkCore.Storage;
using Xunit;

namespace BackendTemplate.Api.Tests.Features.TodoItems.Commands;

[Collection("Database")]
public class CreateTodoItemHandlerTests : IAsyncDisposable
{
    private readonly AppDbContext _db;
    private readonly IDbContextTransaction _tx;

    public CreateTodoItemHandlerTests(DatabaseFixture fixture)
    {
        _db = fixture.CreateDbContext();
        _tx = _db.Database.BeginTransaction();
    }

    [Fact]
    public async Task HandleAsync_GivenValidCommand_ThenReturnsSuccess()
    {
        var result = await new CreateTodoItemHandler(_db)
            .HandleAsync(new CreateTodoItemRequest("Buy milk"));
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task HandleAsync_GivenValidCommand_ThenCreatesItemWithPendingStatus()
    {
        var result = await new CreateTodoItemHandler(_db)
            .HandleAsync(new CreateTodoItemRequest("Buy milk"));
        Assert.Equal(TodoStatus.Pending, result.Value.Status);
        Assert.Equal("Buy milk", result.Value.Title);
    }

    [Fact]
    public async Task HandleAsync_GivenValidCommand_ThenPersistsToDatabase()
    {
        var result = await new CreateTodoItemHandler(_db)
            .HandleAsync(new CreateTodoItemRequest("Persisted item"));

        var persisted = await _db.TodoItems.FindAsync([result.Value.Id]);
        Assert.NotNull(persisted);
    }

    public async ValueTask DisposeAsync()
    {
        await _tx.RollbackAsync();
        await _tx.DisposeAsync();
        await _db.DisposeAsync();
    }
}
```

- [ ] **Step 2: Write `Queries/GetTodoItemHandlerTests.cs`**

```csharp
using BackendTemplate.Api.Features.TodoItems.Queries.GetTodoItem;
using BackendTemplate.Domain.Common;
using BackendTemplate.Infrastructure.Persistence;
using BackendTemplate.Testing.Common;
using BackendTemplate.Testing.Common.Builders;
using Microsoft.EntityFrameworkCore.Storage;
using Xunit;

namespace BackendTemplate.Api.Tests.Features.TodoItems.Queries;

[Collection("Database")]
public class GetTodoItemHandlerTests : IAsyncDisposable
{
    private readonly AppDbContext _db;
    private readonly IDbContextTransaction _tx;

    public GetTodoItemHandlerTests(DatabaseFixture fixture)
    {
        _db = fixture.CreateDbContext();
        _tx = _db.Database.BeginTransaction();
    }

    [Fact]
    public async Task HandleAsync_GivenExistingId_ThenReturnsItem()
    {
        var item = TodoItemBuilder.Default().WithTitle("Existing").Build();
        _db.TodoItems.Add(item);
        await _db.SaveChangesAsync();

        var result = await new GetTodoItemHandler(_db).HandleAsync(item.Id.Value);

        Assert.True(result.IsSuccess);
        Assert.Equal("Existing", result.Value.Title);
    }

    [Fact]
    public async Task HandleAsync_GivenMissingId_ThenReturnsNotFound()
    {
        var result = await new GetTodoItemHandler(_db).HandleAsync(Guid.NewGuid());

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorKind.NotFound, result.Kind);
    }

    public async ValueTask DisposeAsync()
    {
        await _tx.RollbackAsync();
        await _tx.DisposeAsync();
        await _db.DisposeAsync();
    }
}
```

- [ ] **Step 3: Write `Queries/ListTodoItemsHandlerTests.cs`**

```csharp
using BackendTemplate.Api.Features.TodoItems.Queries.ListTodoItems;
using BackendTemplate.Infrastructure.Persistence;
using BackendTemplate.Testing.Common;
using BackendTemplate.Testing.Common.Builders;
using Microsoft.EntityFrameworkCore.Storage;
using Xunit;

namespace BackendTemplate.Api.Tests.Features.TodoItems.Queries;

[Collection("Database")]
public class ListTodoItemsHandlerTests : IAsyncDisposable
{
    private readonly AppDbContext _db;
    private readonly IDbContextTransaction _tx;

    public ListTodoItemsHandlerTests(DatabaseFixture fixture)
    {
        _db = fixture.CreateDbContext();
        _tx = _db.Database.BeginTransaction();
    }

    [Fact]
    public async Task HandleAsync_GivenNoItems_ThenReturnsEmptyPage()
    {
        var result = await new ListTodoItemsHandler(_db)
            .HandleAsync(new ListTodoItemsRequest(1, 20));

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value.Items);
        Assert.Equal(0, result.Value.Total);
    }

    [Fact]
    public async Task HandleAsync_GivenTwoItems_ThenReturnsBothWithCorrectTotal()
    {
        _db.TodoItems.AddRange(
            TodoItemBuilder.Default().WithTitle("First").Build(),
            TodoItemBuilder.Default().WithTitle("Second").Build());
        await _db.SaveChangesAsync();

        var result = await new ListTodoItemsHandler(_db)
            .HandleAsync(new ListTodoItemsRequest(1, 20));

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Total);
        Assert.Equal(2, result.Value.Items.Count);
    }

    [Fact]
    public async Task HandleAsync_GivenPageSize1_ThenReturnsOnlyOneItem()
    {
        _db.TodoItems.AddRange(
            TodoItemBuilder.Default().WithTitle("A").Build(),
            TodoItemBuilder.Default().WithTitle("B").Build());
        await _db.SaveChangesAsync();

        var result = await new ListTodoItemsHandler(_db)
            .HandleAsync(new ListTodoItemsRequest(1, 1));

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value.Items);
        Assert.Equal(2, result.Value.Total);
    }

    public async ValueTask DisposeAsync()
    {
        await _tx.RollbackAsync();
        await _tx.DisposeAsync();
        await _db.DisposeAsync();
    }
}
```

- [ ] **Step 4: Write `Commands/UpdateTodoItemHandlerTests.cs`**

```csharp
using BackendTemplate.Api.Features.TodoItems.Commands.UpdateTodoItem;
using BackendTemplate.Domain.Common;
using BackendTemplate.Domain.Entities;
using BackendTemplate.Infrastructure.Persistence;
using BackendTemplate.Testing.Common;
using BackendTemplate.Testing.Common.Builders;
using Microsoft.EntityFrameworkCore.Storage;
using Xunit;

namespace BackendTemplate.Api.Tests.Features.TodoItems.Commands;

[Collection("Database")]
public class UpdateTodoItemHandlerTests : IAsyncDisposable
{
    private readonly AppDbContext _db;
    private readonly IDbContextTransaction _tx;

    public UpdateTodoItemHandlerTests(DatabaseFixture fixture)
    {
        _db = fixture.CreateDbContext();
        _tx = _db.Database.BeginTransaction();
    }

    [Fact]
    public async Task HandleAsync_GivenNewTitle_ThenUpdatesTitle()
    {
        var item = TodoItemBuilder.Default().WithTitle("Old").Build();
        _db.TodoItems.Add(item);
        await _db.SaveChangesAsync();

        var result = await new UpdateTodoItemHandler(_db)
            .HandleAsync(item.Id.Value, new UpdateTodoItemRequest("New", null));

        Assert.True(result.IsSuccess);
        Assert.Equal("New", result.Value.Title);
    }

    [Fact]
    public async Task HandleAsync_GivenNewStatus_ThenUpdatesStatus()
    {
        var item = TodoItemBuilder.Default().Build();
        _db.TodoItems.Add(item);
        await _db.SaveChangesAsync();

        var result = await new UpdateTodoItemHandler(_db)
            .HandleAsync(item.Id.Value, new UpdateTodoItemRequest(null, "Done"));

        Assert.True(result.IsSuccess);
        Assert.Equal(TodoStatus.Done, result.Value.Status);
    }

    [Fact]
    public async Task HandleAsync_GivenMissingId_ThenReturnsNotFound()
    {
        var result = await new UpdateTodoItemHandler(_db)
            .HandleAsync(Guid.NewGuid(), new UpdateTodoItemRequest("X", null));

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorKind.NotFound, result.Kind);
    }

    public async ValueTask DisposeAsync()
    {
        await _tx.RollbackAsync();
        await _tx.DisposeAsync();
        await _db.DisposeAsync();
    }
}
```

- [ ] **Step 5: Write `Commands/DeleteTodoItemHandlerTests.cs`**

```csharp
using BackendTemplate.Api.Features.TodoItems.Commands.DeleteTodoItem;
using BackendTemplate.Domain.Common;
using BackendTemplate.Infrastructure.Persistence;
using BackendTemplate.Testing.Common;
using BackendTemplate.Testing.Common.Builders;
using Microsoft.EntityFrameworkCore.Storage;
using Xunit;

namespace BackendTemplate.Api.Tests.Features.TodoItems.Commands;

[Collection("Database")]
public class DeleteTodoItemHandlerTests : IAsyncDisposable
{
    private readonly AppDbContext _db;
    private readonly IDbContextTransaction _tx;

    public DeleteTodoItemHandlerTests(DatabaseFixture fixture)
    {
        _db = fixture.CreateDbContext();
        _tx = _db.Database.BeginTransaction();
    }

    [Fact]
    public async Task HandleAsync_GivenExistingId_ThenReturnsSuccess()
    {
        var item = TodoItemBuilder.Default().Build();
        _db.TodoItems.Add(item);
        await _db.SaveChangesAsync();

        var result = await new DeleteTodoItemHandler(_db).HandleAsync(item.Id.Value);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task HandleAsync_GivenExistingId_ThenRemovesFromDatabase()
    {
        var item = TodoItemBuilder.Default().Build();
        _db.TodoItems.Add(item);
        await _db.SaveChangesAsync();

        await new DeleteTodoItemHandler(_db).HandleAsync(item.Id.Value);

        var found = await _db.TodoItems.FindAsync([item.Id]);
        Assert.Null(found);
    }

    [Fact]
    public async Task HandleAsync_GivenMissingId_ThenReturnsNotFound()
    {
        var result = await new DeleteTodoItemHandler(_db).HandleAsync(Guid.NewGuid());

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorKind.NotFound, result.Kind);
    }

    public async ValueTask DisposeAsync()
    {
        await _tx.RollbackAsync();
        await _tx.DisposeAsync();
        await _db.DisposeAsync();
    }
}
```

- [ ] **Step 6: Run API tests**

```powershell
dotnet test backend/tests/BackendTemplate.Api.Tests/BackendTemplate.Api.Tests.csproj --logger "console;verbosity=normal"
```
Expected: all tests pass (requires Docker).

- [ ] **Step 7: Commit**

```bash
git add backend/tests/BackendTemplate.Api.Tests/
git commit -m "test: add handler integration tests for all TodoItem slices"
```

---

## Task 16: EF migration and full test run

- [ ] **Step 1: Add initial migration**

Requires a running Postgres or a design-time factory. Use:
```powershell
dotnet ef migrations add InitialCreate `
  --project backend/src/BackendTemplate.Infrastructure/BackendTemplate.Infrastructure.csproj `
  --startup-project backend/src/BackendTemplate.Api/BackendTemplate.Api.csproj `
  --output-dir Persistence/Migrations
```

Note: `ConnectionStrings:Default` must be set for design-time. Set via user secrets before running:
```powershell
dotnet user-secrets set "ConnectionStrings:Default" "Host=localhost;Database=backendtemplate;Username=postgres;Password=postgres" `
  --project backend/src/BackendTemplate.Api/BackendTemplate.Api.csproj
```

Or set as an env var temporarily: `$env:ConnectionStrings__Default = "..."`.

- [ ] **Step 2: Run full solution build**

```powershell
dotnet build backend/BackendTemplate.sln
```
Expected: `Build succeeded. 0 Error(s)`

- [ ] **Step 3: Run full test suite**

```powershell
dotnet test backend/BackendTemplate.sln --logger "console;verbosity=normal"
```
Expected: all tests in Domain.Tests, Infrastructure.Tests, and Api.Tests pass.

- [ ] **Step 4: Final commit**

```bash
git add backend/src/BackendTemplate.Infrastructure/Persistence/Migrations/
git commit -m "feat: add InitialCreate EF migration"
```

---

## Self-Review

**Spec coverage check:**
- ✅ Solution structure (`backend/src/` + `backend/tests/`)
- ✅ Dependency direction: Api → Domain+Infrastructure, Infrastructure → Domain, Domain → nothing
- ✅ `Result<T>` with `Success/Failure/Value/Error/Kind/IsSuccess/IsFailure`
- ✅ `Unit` for delete operations
- ✅ `ErrorKind.Validation/NotFound/Conflict` → 422/404/409
- ✅ Strongly-typed ID `record struct` with bulk EF Core convention
- ✅ Minimal APIs, no controllers
- ✅ `IEndpoint` + assembly scan (`MapEndpoints`)
- ✅ `IScopedService/ISingletonService/ITransientService` marker interfaces + Scrutor scan
- ✅ Mapperly `static partial class`, not DI-registered
- ✅ FluentValidation via `ValidationFilter<TRequest>` only — no `ValidateAndThrow`
- ✅ Serilog + `UseSerilogRequestLogging()`
- ✅ OpenAPI at `/openapi/v1.json`, Scalar at `/scalar/v1`
- ✅ Health checks `/health/live` + `/health/ready` with EF Core DB check
- ✅ RFC 7807 Problem Details on all errors
- ✅ 201 + Location header on create
- ✅ 204 No Content on delete
- ✅ Paginated collection response `Page<T>`
- ✅ PATCH with JSON Merge Patch semantics
- ✅ `GlobalExceptionHandler` → 500, does not catch `ValidationException`
- ✅ `ToPagedAsync` in Infrastructure, 1-based page translation
- ✅ EF fluent config, no data annotations on entities
- ✅ Handlers inject `AppDbContext` directly, call `SaveChangesAsync` directly
- ✅ `AddInfrastructure()` extension method
- ✅ Test isolation: Testcontainers + one shared container + transaction rollback per test
- ✅ Test naming: `{Method}_Given{Scenario}_Then{Assertion}`
- ✅ Builders in `Testing.Common` with `Default()`, `InProgress()`, `Done()` recipes
- ✅ No MediatR, no repository pattern, no `IUnitOfWork`
- ✅ Nullable reference types enabled

**Gaps found:** None.

**Type consistency check:** All handler parameter types, request records, and response records are consistent across tasks. `TodoItemResponse` defined once at feature root and used everywhere. `TodoItemMapper.ToResponse` method signature stable throughout.
