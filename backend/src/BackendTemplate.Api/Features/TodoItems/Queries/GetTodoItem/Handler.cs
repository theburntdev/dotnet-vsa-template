using BackendTemplate.Api.Common;
using BackendTemplate.Api.Features.TodoItems;
using BackendTemplate.Domain.Common;
using BackendTemplate.Infrastructure.Persistence;

namespace BackendTemplate.Api.Features.TodoItems.Queries.GetTodoItem;

public sealed class GetTodoItemHandler(AppDbContext db) : IScopedService
{
    public async Task<Result<TodoItemResponse>> HandleAsync(
        Guid id,
        CancellationToken ct = default)
    {
        var item = await db.TodoItems.FindAsync([new TodoItemId(id)], ct);
        if (item is null)
            return Result<TodoItemResponse>.Failure(
                $"Todo item {id} was not found.", ErrorKind.NotFound);

        return Result<TodoItemResponse>.Success(TodoItemMapper.ToResponse(item));
    }
}
