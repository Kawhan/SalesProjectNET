using FluentValidation;
using SalesProject.Domain.Enums;

namespace SalesProject.WebApi.Features.Branches.CreateBranch;

/// <summary>
/// Validator for CreateBranchRequest.
/// </summary>
public class CreateBranchRequestValidator : AbstractValidator<CreateBranchRequest>
{
    /// <summary>
    /// Initializes a new instance of the CreateBranchRequestValidator with defined validation rules.
    /// </summary>
    /// <remarks>
    /// Validation rules include:
    /// - Name: Required, must be between 3 and 100 characters
    /// - Address: Must be at most 250 characters when provided
    /// - Status: Cannot be Unknown
    /// </remarks>
    public CreateBranchRequestValidator()
    {
        RuleFor(branch => branch.Name)
            .NotEmpty()
            .WithMessage("Branch name cannot be empty.")
            .MinimumLength(3)
            .WithMessage("Branch name must be at least 3 characters long.")
            .MaximumLength(100)
            .WithMessage("Branch name must be at most 100 characters long.");

        RuleFor(branch => branch.Address)
            .MaximumLength(250)
            .WithMessage("Branch address must be at most 250 characters long.");

        RuleFor(branch => branch.Status)
            .NotEqual(BranchStatus.Unknown)
            .WithMessage("Branch status cannot be unknown.");
    }
}