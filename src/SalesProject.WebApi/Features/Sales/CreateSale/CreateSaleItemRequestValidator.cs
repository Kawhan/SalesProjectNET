using FluentValidation;

namespace SalesProject.WebApi.Features.Sales.CreateSale;

/// <summary>
/// Validator for CreateSaleItemRequest.
/// </summary>
public class CreateSaleItemRequestValidator : AbstractValidator<CreateSaleItemRequest>
{
    /// <summary>
    /// Initializes a new instance of the CreateSaleItemRequestValidator with defined validation rules.
    /// </summary>
    /// <remarks>
    /// Validation rules include:
    /// - ProductId: Required and cannot be empty
    /// - Quantity: Must be greater than zero
    /// </remarks>
    public CreateSaleItemRequestValidator()
    {
        RuleFor(item => item.ProductId)
            .NotEmpty()
            .WithMessage("Product id cannot be empty.");

        RuleFor(item => item.Quantity)
            .GreaterThan(0)
            .WithMessage("Product quantity must be greater than zero.");
    }
}

