using FluentValidation;

namespace SalesProject.WebApi.Features.Branches.DeleteBranch;

/// <summary>
/// Validator for DeleteBranchRequest.
/// </summary>
public class DeleteBranchRequestValidator : AbstractValidator<DeleteBranchRequest>
{
    /// <summary>
    /// Initializes a new instance of DeleteBranchRequestValidator with defined validation rules.
    /// </summary>
    public DeleteBranchRequestValidator()
    {
        RuleFor(branch => branch.Id)
            .NotEmpty()
            .WithMessage("Branch id cannot be empty.");
    }
}