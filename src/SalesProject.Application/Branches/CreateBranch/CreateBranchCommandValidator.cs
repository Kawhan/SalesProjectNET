using FluentValidation;
using SalesProject.Domain.Enums;

namespace SalesProject.Application.Branches.CreateBranch;

/// <summary>
/// Validator for CreateBranchCommand.
/// </summary>
public class CreateBranchCommandValidator : AbstractValidator<CreateBranchCommand>
{
    /// <summary>
    /// Initializes a new instance of the CreateBranchCommandValidator with defined validation rules.
    /// </summary>
    /// <remarks>
    /// Validation rules include:
    /// - Name: Required, must be between 3 and 100 characters
    /// - Address: Must be at most 250 characters when provided
    /// - Status: Cannot be Unknown
    /// </remarks>
    public CreateBranchCommandValidator()
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