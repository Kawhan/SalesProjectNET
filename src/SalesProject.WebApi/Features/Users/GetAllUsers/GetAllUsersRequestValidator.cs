using FluentValidation;

namespace SalesProject.WebApi.Features.Users.GetAllUsers;

/// <summary>
/// Validator for GetAllUsersRequest.
/// </summary>
public class GetAllUsersRequestValidator : AbstractValidator<GetAllUsersRequest>
{
    /// <summary>
    /// Initializes a new instance of the GetAllUsersRequestValidator with defined validation rules.
    /// </summary>
    public GetAllUsersRequestValidator()
    {
        RuleFor(request => request.PageNumber)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than zero.");

        RuleFor(request => request.PageSize)
            .GreaterThan(0)
            .WithMessage("Page size must be greater than zero.")
            .LessThanOrEqualTo(100)
            .WithMessage("Page size must be less than or equal to 100.");

        RuleFor(request => request.Username)
            .MaximumLength(50)
            .WithMessage("Name filter must be at most 50 characters long.");

        RuleFor(request => request.Email)
            .MaximumLength(100)
            .WithMessage("Email filter must be at most 100 characters long.");

        RuleFor(request => request.Phone)
            .MaximumLength(20)
            .WithMessage("Phone filter must be at most 20 characters long.");
    }
}
