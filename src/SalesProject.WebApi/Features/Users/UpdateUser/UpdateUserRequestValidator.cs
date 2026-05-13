using FluentValidation;
using SalesProject.Domain.Enums;

namespace SalesProject.WebApi.Features.Users.UpdateUser;

/// <summary>
/// Validator for UpdateUserRequest.
/// </summary>
public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    /// <summary>
    /// Initializes a new instance of the UpdateUserRequestValidator with defined validation rules.
    /// </summary>
    public UpdateUserRequestValidator()
    {
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