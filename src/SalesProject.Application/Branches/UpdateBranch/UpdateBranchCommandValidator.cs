using FluentValidation;
using SalesProject.Domain.Enums;

namespace SalesProject.Application.Branches.UpdateBranch;

/// <summary>
/// Validator for UpdateBranchCommand.
/// </summary>
public class UpdateBranchCommandValidator : AbstractValidator<UpdateBranchCommand>
{
    /// <summary>
    /// Initializes a new instance of the UpdateBranchCommandValidator with defined validation rules.
    /// </summary>
    /// <remarks>
    /// Validation rules include:
    /// - Id: Required and cannot be empty
    /// - Name: Required, must be between 3 and 100 characters
    /// - Address: Must be at most 250 characters when provided
    /// - Status: Cannot be Unknown
    /// </remarks>
    public UpdateBranchCommandValidator()
    {
        RuleFor(branch => branch.Id)
            .NotEmpty()
            .WithMessage("Branch id cannot be empty.");

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
