using FluentValidation;

namespace TodoApp.Application.Commands.CreateTodo;

public class CreateTodoCommandValidator : AbstractValidator<CreateTodoCommand>
{
    public CreateTodoCommandValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MinimumLength(10).WithMessage("Description must be at least 10 characters long")
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

        RuleFor(x => x.Deadline)
            .GreaterThan(DateTime.UtcNow).When(x => x.Deadline.HasValue)
            .WithMessage("Deadline must be in the future");
    }
}
