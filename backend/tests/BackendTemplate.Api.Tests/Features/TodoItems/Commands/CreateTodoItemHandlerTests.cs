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
