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
