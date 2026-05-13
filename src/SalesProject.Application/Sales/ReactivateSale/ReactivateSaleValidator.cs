using FluentValidation;

namespace SalesProject.Application.Sales.ReactivateSale;

/// <summary>
/// Validator for ReactivateSaleCommand.
/// </summary>
public class ReactivateSaleValidator : AbstractValidator<ReactivateSaleCommand>
{
    public ReactivateSaleValidator()
    {
        RuleFor(sale => sale.Id)
            .NotEmpty()
            .WithMessage("Sale id cannot be empty.");
    }
}