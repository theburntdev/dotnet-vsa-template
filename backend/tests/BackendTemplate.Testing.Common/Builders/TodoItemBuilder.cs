using BackendTemplate.Domain.Entities;

namespace BackendTemplate.Testing.Common.Builders;

public class TodoItemBuilder
{
    private string _title = "Default Todo Item";
    private TodoStatus _status = TodoStatus.Pending;

    public static TodoItemBuilder Default() => new();
    public static TodoItemBuilder InProgress() => new TodoItemBuilder().WithStatus(TodoStatus.InProgress);
    public static TodoItemBuilder Done() => new TodoItemBuilder().WithStatus(TodoStatus.Done);

    public TodoItemBuilder WithTitle(string title)
    {
        _title = title;
        return this;
    }

    public TodoItemBuilder WithStatus(TodoStatus status)
    {
        _status = status;
        return this;
    }

    public TodoItem Build()
    {
        var item = TodoItem.Create(_title);
        if (_status != TodoStatus.Pending)
            item.UpdateStatus(_status);
        return item;
    }
}
