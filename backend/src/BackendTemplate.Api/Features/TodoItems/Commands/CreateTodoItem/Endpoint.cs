using BackendTemplate.Api.Common;
using BackendTemplate.Api.Features.TodoItems;

namespace BackendTemplate.Api.Features.TodoItems.Commands.CreateTodoItem;

public class CreateTodoItemEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/todo-items", async (
            CreateTodoItemRequest request,
            CreateTodoItemHandler handler,
            CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(request, ct);
            return result.ToHttpResult(
                TodoItemMapper.ToResponse,
                r => $"/todo-items/{r.Id}");
        })
        .WithValidation<CreateTodoItemRequest>()
        .WithTags(Feature.Tag)
        .WithName(nameof(CreateTodoItemEndpoint)[..^"Endpoint".Length]);
    }
}
