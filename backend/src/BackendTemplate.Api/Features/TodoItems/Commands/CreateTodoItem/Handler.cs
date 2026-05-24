using BackendTemplate.Api.Common;
using BackendTemplate.Domain.Common;
using BackendTemplate.Domain.Entities;
using BackendTemplate.Infrastructure.Persistence;

namespace BackendTemplate.Api.Features.TodoItems.Commands.CreateTodoItem;

public sealed class CreateTodoItemHandler(AppDbContext db) : IScopedService
{
    public async Task<Result<TodoItem>> HandleAsync(
        CreateTodoItemRequest request,
        CancellationToken ct = default)
    {
        var item = TodoItem.Create(request.Title);
        db.TodoItems.Add(item);
        await db.SaveChangesAsync(ct);
        return Result<TodoItem>.Success(item);
    }
}
