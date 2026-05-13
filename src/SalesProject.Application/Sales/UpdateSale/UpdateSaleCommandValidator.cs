using FluentValidation;

namespace SalesProject.Application.Sales.UpdateSale;

/// <summary>
/// Validator for UpdateSaleCommand.
/// </summary>
public class UpdateSaleCommandValidator : AbstractValidator<UpdateSaleCommand>
{
    /// <summary>
    /// Initializes a new instance of the UpdateSaleCommandValidator with defined validation rules.
    /// </summary>
    /// <remarks>
    /// Validation rules include:
    /// - Id: Required and cannot be empty
    /// - UserId: Required and cannot be empty
    /// - BranchId: Required and cannot be empty
    /// - Items: Required and must contain at least one item
    /// - ProductId: Required and cannot be empty
    /// - Quantity: Must be greater than zero
    /// - Items: It is not possible to sell more than 20 identical items
    /// </remarks>
    public UpdateSaleCommandValidator()
    {
        RuleFor(sale => sale.Id)
            .NotEmpty()
            .WithMessage("Sale id cannot be empty.");

        RuleFor(sale => sale.UserId)
            .NotEmpty()
            .WithMessage("User id cannot be empty.");

        RuleFor(sale => sale.BranchId)
            .NotEmpty()
            .WithMessage("Branch id cannot be empty.");

        RuleFor(sale => sale.Items)
            .NotEmpty()
            .WithMessage("Sale must contain at least one item.");

        RuleForEach(sale => sale.Items)
            .ChildRules(item =>
            {
                item.RuleFor(saleItem => saleItem.ProductId)
                    .NotEmpty()
                    .WithMessage("Product id cannot be empty.");

                item.RuleFor(saleItem => saleItem.Quantity)
                    .GreaterThan(0)
                    .WithMessage("Product quantity must be greater than zero.");
            });

        RuleFor(sale => sale.Items)
            .Must(NotExceedTwentyIdenticalItems)
            .WithMessage("It is not possible to sell more than 20 identical items.");
    }

    private static bool NotExceedTwentyIdenticalItems(List<UpdateSaleItemCommand> items)
    {
        if (items is null || items.Count == 0)
            return true;

        return items
            .GroupBy(item => item.ProductId)
            .All(group => group.Sum(item => item.Quantity) <= 20);
    }
}