using BackendTemplate.Api.Common;
using BackendTemplate.Domain.Common;
using BackendTemplate.Infrastructure.Persistence;

namespace BackendTemplate.Api.Features.TodoItems.Commands.DeleteTodoItem;

public sealed class DeleteTodoItemHandler(AppDbContext db) : IScopedService
{
    public async Task<Result<Unit>> HandleAsync(Guid id, CancellationToken ct = default)
    {
        var item = await db.TodoItems.FindAsync([new TodoItemId(id)], ct);
        if (item is null)
            return Result<Unit>.Failure(
                $"Todo item {id} was not found.", ErrorKind.NotFound);

        db.TodoItems.Remove(item);
        await db.SaveChangesAsync(ct);
        return Result<Unit>.Success(Unit.Value);
    }
}
