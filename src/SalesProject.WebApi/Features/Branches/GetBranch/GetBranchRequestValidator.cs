using FluentValidation;

namespace SalesProject.WebApi.Features.Branches.GetBranch;

/// <summary>
/// Validator for GetBranchRequest.
/// </summary>
public class GetBranchRequestValidator : AbstractValidator<GetBranchRequest>
{
    /// <summary>
    /// Initializes a new instance of the GetBranchRequestValidator with defined validation rules.
    /// </summary>
    public GetBranchRequestValidator()
    {
        RuleFor(branch => branch.Id)
            .NotEmpty()
            .WithMessage("Branch id cannot be empty.");
    }
}