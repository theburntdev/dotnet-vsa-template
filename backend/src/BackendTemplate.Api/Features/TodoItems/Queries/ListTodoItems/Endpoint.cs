using BackendTemplate.Api.Common;

namespace BackendTemplate.Api.Features.TodoItems.Queries.ListTodoItems;

public class ListTodoItemsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/todo-items", async (
            [AsParameters] ListTodoItemsRequest request,
            ListTodoItemsHandler handler,
            CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(request, ct);
            return result.ToHttpResult(p => p);
        })
        .WithValidation<ListTodoItemsRequest>()
        .WithTags(Feature.Tag)
        .WithName(nameof(ListTodoItemsEndpoint)[..^"Endpoint".Length]);
    }
}
