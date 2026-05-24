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
