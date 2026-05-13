using FluentValidation;

namespace SalesProject.Domain.Validation;

public class NameValidator : AbstractValidator<string>
{
    public NameValidator()
    {
        RuleFor(name => name)
            .NotEmpty()
            .WithMessage("The product name cannot be empty.")
            .MinimumLength(3)
            .WithMessage("The product name must be at least 3 characters long.")
            .MaximumLength(50)
            .WithMessage("The product name must be at most 50 characters long.");
    }
}


