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
