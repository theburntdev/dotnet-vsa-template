using BackendTemplate.Api.Common;
using BackendTemplate.Domain.Common;
using BackendTemplate.Domain.Entities;
using BackendTemplate.Infrastructure.Persistence;

namespace BackendTemplate.Api.Features.TodoItems.Commands.UpdateTodoItem;

public sealed class UpdateTodoItemHandler(AppDbContext db) : IScopedService
{
    public async Task<Result<TodoItem>> HandleAsync(
        Guid id,
        UpdateTodoItemRequest request,
        CancellationToken ct = default)
    {
        var item = await db.TodoItems.FindAsync([new TodoItemId(id)], ct);
        if (item is null)
            return Result<TodoItem>.Failure(
                $"Todo item {id} was not found.", ErrorKind.NotFound);

        if (request.Title is not null)
            item.UpdateTitle(request.Title);

        if (request.Status is not null)
        {
            if (!Enum.TryParse<TodoStatus>(request.Status, ignoreCase: true, out var status))
                return Result<TodoItem>.Failure($"Invalid status: {request.Status}");
            item.UpdateStatus(status);
        }

        await db.SaveChangesAsync(ct);
        return Result<TodoItem>.Success(item);
    }
}
