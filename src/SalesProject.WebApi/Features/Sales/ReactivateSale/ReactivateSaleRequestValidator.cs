using FluentValidation;

namespace SalesProject.WebApi.Features.Sales.ReactivateSale;

/// <summary>
/// Validator for ReactivateSaleRequest
/// </summary>
public class ReactivateSaleRequestValidator : AbstractValidator<ReactivateSaleRequest>
{
    /// <summary>
    /// Initializes validation rules for ReactivateSaleRequest
    /// </summary>
    public ReactivateSaleRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Sale ID is required");
    }
}

