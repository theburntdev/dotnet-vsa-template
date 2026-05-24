using BackendTemplate.Api.Common;
using BackendTemplate.Api.Features.TodoItems;

namespace BackendTemplate.Api.Features.TodoItems.Commands.UpdateTodoItem;

public class UpdateTodoItemEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("/todo-items/{id:guid}", async (
            Guid id,
            UpdateTodoItemRequest request,
            UpdateTodoItemHandler handler,
            CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(id, request, ct);
            return result.ToHttpResult(TodoItemMapper.ToResponse);
        })
        .WithValidation<UpdateTodoItemRequest>()
        .WithTags("TodoItems")
        .WithName("UpdateTodoItem");
    }
}
