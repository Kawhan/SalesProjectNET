using FluentValidation;

namespace SalesProject.Application.Branches.GetBranch;

/// <summary>
/// Validator for GetBranchCommand.
/// </summary>
public class GetBranchCommandValidator : AbstractValidator<GetBranchCommand>
{
    /// <summary>
    /// Initializes a new instance of the GetBranchCommandValidator with defined validation rules.
    /// </summary>
    /// <remarks>
    /// Validation rules include:
    /// - Id: Required and cannot be empty
    /// </remarks>
    public GetBranchCommandValidator()
    {
        RuleFor(branch => branch.Id)
            .NotEmpty()
            .WithMessage("Branch id cannot be empty.");
    }
}