namespace BackendTemplate.Api.Features.TodoItems;

public record TodoItemResponse(Guid Id, string Title, string Status, DateTime CreatedAt);
