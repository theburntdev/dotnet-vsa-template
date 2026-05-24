namespace BackendTemplate.Api.Features.TodoItems.Queries.ListTodoItems;

public record ListTodoItemsRequest(int Page = 1, int PageSize = 20);
