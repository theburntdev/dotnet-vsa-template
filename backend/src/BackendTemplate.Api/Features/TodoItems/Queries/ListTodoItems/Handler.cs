using BackendTemplate.Api.Common;
using BackendTemplate.Api.Features.TodoItems;
using BackendTemplate.Domain.Common;
using BackendTemplate.Infrastructure.Extensions;
using BackendTemplate.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BackendTemplate.Api.Features.TodoItems.Queries.ListTodoItems;

public sealed class ListTodoItemsHandler(AppDbContext db) : IScopedService
{
    public async Task<Result<Page<TodoItemResponse>>> HandleAsync(
        ListTodoItemsRequest request,
        CancellationToken ct = default)
    {
        var (items, total) = await db.TodoItems
            .OrderBy(x => x.CreatedAt)
            .Select(x => new TodoItemResponse(x.Id.Value, x.Title, x.Status.ToString(), x.CreatedAt))
            .ToPagedAsync(request.Page, request.PageSize, ct);

        return Result<Page<TodoItemResponse>>.Success(
            new Page<TodoItemResponse>(items, total, request.Page, request.PageSize));
    }
}
