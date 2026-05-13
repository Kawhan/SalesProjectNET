using FluentValidation;

namespace SalesProject.Application.Sales.CreateSale;

/// <summary>
/// Validator for CreateSaleItemCommand.
/// </summary>
public class CreateSaleItemCommandValidator : AbstractValidator<CreateSaleItemCommand>
{
    /// <summary>
    /// Initializes a new instance of the CreateSaleItemCommandValidator with defined validation rules.
    /// </summary>
    /// <remarks>
    /// Validation rules include:
    /// - ProductId: Required and cannot be empty
    /// - Quantity: Must be greater than zero
    /// </remarks>
    public CreateSaleItemCommandValidator()
    {
        RuleFor(item => item.ProductId)
            .NotEmpty()
            .WithMessage("Product id cannot be empty.");

        RuleFor(item => item.Quantity)
            .GreaterThan(0)
            .WithMessage("Product quantity must be greater than zero.");
    }
}