using BackendTemplate.Domain.Common;
using BackendTemplate.Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace BackendTemplate.Api.Features.TodoItems;

[Mapper]
public static partial class TodoItemMapper
{
    [MapProperty($"{nameof(TodoItem.Id)}.{nameof(TodoItemId.Value)}", nameof(TodoItemResponse.Id))]
    public static partial TodoItemResponse ToResponse(TodoItem source);
}
