using FluentValidation;

namespace BackendTemplate.Api.Features.TodoItems.Queries.ListTodoItems;

public class ListTodoItemsValidator : AbstractValidator<ListTodoItemsRequest>
{
    public ListTodoItemsValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
