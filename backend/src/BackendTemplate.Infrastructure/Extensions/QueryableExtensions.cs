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
