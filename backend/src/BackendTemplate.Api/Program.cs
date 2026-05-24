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
