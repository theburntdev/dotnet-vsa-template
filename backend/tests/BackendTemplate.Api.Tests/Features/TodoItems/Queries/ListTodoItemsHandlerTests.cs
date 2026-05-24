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
