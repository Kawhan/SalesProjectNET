using FluentValidation;

namespace SalesProject.WebApi.Features.Sales.CreateSale;

/// <summary>
/// Validator for CreateSaleRequest.
/// </summary>
public class CreateSaleRequestValidator : AbstractValidator<CreateSaleRequest>
{
    /// <summary>
    /// Initializes a new instance of the CreateSaleRequestValidator with defined validation rules.
    /// </summary>
    /// <remarks>
    /// Validation rules include:
    /// - UserId: Required and cannot be empty
    /// - BranchId: Required and cannot be empty
    /// - Items: Required and must contain at least one item
    /// - Items: Each item must have a valid product and quantity
    /// - Items: It is not possible to sell more than 20 identical products
    /// </remarks>
    public CreateSaleRequestValidator()
    {
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
            .SetValidator(new CreateSaleItemRequestValidator());

        RuleFor(sale => sale.Items)
            .Must(NotExceedTwentyIdenticalItems)
            .WithMessage("It is not possible to sell more than 20 identical items.");
    }

    private static bool NotExceedTwentyIdenticalItems(List<CreateSaleItemRequest> items)
    {
        if (items is null || items.Count == 0)
            return true;

        return items
            .GroupBy(item => item.ProductId)
            .All(group => group.Sum(item => item.Quantity) <= 20);
    }
}
