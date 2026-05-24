using BackendTemplate.Api.Common;

namespace BackendTemplate.Api.Features.TodoItems.Commands.DeleteTodoItem;

public class DeleteTodoItemEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/todo-items/{id:guid}", async (
            Guid id,
            DeleteTodoItemHandler handler,
            CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(id, ct);
            return result.ToHttpResult();
        })
        .WithTags(Feature.Tag)
        .WithName(nameof(DeleteTodoItemEndpoint)[..^"Endpoint".Length]);
    }
}
