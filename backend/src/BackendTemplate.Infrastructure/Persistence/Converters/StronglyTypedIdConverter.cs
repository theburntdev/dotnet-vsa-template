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
