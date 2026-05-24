using BackendTemplate.Domain.Entities;
using FluentValidation;

namespace BackendTemplate.Api.Features.TodoItems.Commands.UpdateTodoItem;

public class UpdateTodoItemValidator : AbstractValidator<UpdateTodoItemRequest>
{
    public UpdateTodoItemValidator()
    {
        RuleFor(x => x)
            .Must(x => x.Title is not null || x.Status is not null)
            .WithName("request")
            .WithMessage("At least one field must be provided.");

        When(x => x.Title is not null, () =>
            RuleFor(x => x.Title!).NotEmpty().MaximumLength(200));

        When(x => x.Status is not null, () =>
            RuleFor(x => x.Status!)
                .Must(s => Enum.TryParse<TodoStatus>(s, ignoreCase: true, out _))
                .WithMessage($"Status must be one of: {string.Join(", ", Enum.GetNames<TodoStatus>())}"));
    }
}
