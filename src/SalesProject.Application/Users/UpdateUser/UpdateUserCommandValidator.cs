using FluentValidation;
using SalesProject.Domain.Enums;

namespace SalesProject.Application.Users.UpdateUser;

/// <summary>
/// Validator for UpdateUserCommand.
/// </summary>
public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    /// <summary>
    /// Initializes a new instance of the UpdateUserCommandValidator with defined validation rules.
    /// </summary>
    public UpdateUserCommandValidator()
    {
        RuleFor(user => user.Id)
            .NotEmpty()
            .WithMessage("User id cannot be empty.");

        RuleFor(user => user.Username)
            .NotEmpty()
            .WithMessage("Username cannot be empty.")
            .MinimumLength(3)
            .WithMessage("Username must be at least 3 characters long.")
            .MaximumLength(50)
            .WithMessage("Username must be at most 50 characters long.");

        RuleFor(user => user.Email)
            .NotEmpty()
            .WithMessage("Email cannot be empty.")
            .MaximumLength(100)
            .WithMessage("Email must be at most 100 characters long.")
            .EmailAddress()
            .WithMessage("Email must be a valid email address.");

        RuleFor(user => user.Phone)
            .NotEmpty()
            .WithMessage("Phone cannot be empty.")
            .MaximumLength(20)
            .WithMessage("Phone must be at most 20 characters long.");

        RuleFor(user => user.Status)
            .NotEqual(UserStatus.Unknown)
            .WithMessage("User status cannot be unknown.");

        RuleFor(user => user.Role)
            .NotEqual(UserRole.None)
            .WithMessage("User role cannot be none.");
    }
}