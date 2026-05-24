using FluentValidation;

namespace BackendTemplate.Api.Features.TodoItems.Commands.CreateTodoItem;

public class CreateTodoItemValidator : AbstractValidator<CreateTodoItemRequest>
{
    public CreateTodoItemValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
    }
}
