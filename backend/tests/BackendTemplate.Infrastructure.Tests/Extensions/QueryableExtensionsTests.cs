using BackendTemplate.Infrastructure.Extensions;
using BackendTemplate.Infrastructure.Persistence;
using BackendTemplate.Testing.Common;
using BackendTemplate.Testing.Common.Builders;
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
