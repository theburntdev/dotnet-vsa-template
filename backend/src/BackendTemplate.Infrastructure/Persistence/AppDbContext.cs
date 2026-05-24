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
        // Find every `record struct` in the Domain assembly that looks like a strongly-typed ID:
        // a value type with exactly one property named Value of type Guid (e.g. TodoItemId).
        var idTypes = typeof(TodoItemId).Assembly
            .GetTypes()
            .Where(t => t.IsValueType
                && !t.IsEnum
                && !t.IsPrimitive
                && !t.IsGenericType
                && t.GetProperty("Value") is { PropertyType.FullName: "System.Guid" }
                && t.GetProperties().Length == 1);

        // Register a value converter for each discovered ID type so EF stores the inner
        // Guid in the DB column rather than trying to serialize the whole struct.
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
