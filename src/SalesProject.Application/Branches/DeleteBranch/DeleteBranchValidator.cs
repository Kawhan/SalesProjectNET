using FluentValidation;

namespace SalesProject.Application.Branches.DeleteBranch;

/// <summary>
/// Validator for DeleteBranchCommand.
/// </summary>
public class DeleteBranchValidator : AbstractValidator<DeleteBranchCommand>
{
    /// <summary>
    /// Initializes a new instance of DeleteBranchValidator with defined validation rules.
    /// </summary>
    public DeleteBranchValidator()
    {
        RuleFor(branch => branch.Id)
            .NotEmpty()
            .WithMessage("Branch id cannot be empty.");
    }
}