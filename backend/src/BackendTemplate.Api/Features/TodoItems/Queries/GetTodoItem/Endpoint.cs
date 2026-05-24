using BackendTemplate.Api.Common;

namespace BackendTemplate.Api.Features.TodoItems.Queries.GetTodoItem;

public class GetTodoItemEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/todo-items/{id:guid}", async (
            Guid id,
            GetTodoItemHandler handler,
            CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(id, ct);
            return result.ToHttpResult(x => x);
        })
        .WithTags(Feature.Tag)
        .WithName(nameof(GetTodoItemEndpoint)[..^"Endpoint".Length]);
    }
}
