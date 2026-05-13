using FluentValidation;

namespace SalesProject.WebApi.Features.Branches.GetAllBranches;

/// <summary>
/// Validator for GetAllBranchesRequest.
/// </summary>
public class GetAllBranchesRequestValidator : AbstractValidator<GetAllBranchesRequest>
{
    /// <summary>
    /// Initializes a new instance of the GetAllBranchesRequestValidator with defined validation rules.
    /// </summary>
    public GetAllBranchesRequestValidator()
    {
        RuleFor(request => request.PageNumber)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than zero.");

        RuleFor(request => request.PageSize)
            .GreaterThan(0)
            .WithMessage("Page size must be greater than zero.")
            .LessThanOrEqualTo(100)
            .WithMessage("Page size must be less than or equal to 100.");
    }
}