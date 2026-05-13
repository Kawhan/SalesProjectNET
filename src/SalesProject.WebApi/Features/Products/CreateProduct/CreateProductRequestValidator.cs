using FluentValidation;
using SalesProject.Domain.Enums;
using SalesProject.Domain.Validation;

namespace SalesProject.WebApi.Features.Products.CreateProduct;

/// <summary>
/// Validator for CreateProductRequest that defines validation rules for product creation.
/// </summary>
public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
{
    /// <summary>
    /// Initializes a new instance of the CreateProductRequestValidator with defined validation rules.
    /// </summary>
    /// <remarks>
    /// Validation rules include:
    /// - Name: Required, must be at least 3 characters long and at most 50 characters long
    /// - CurrentPrice: Must be greater than zero
    /// - Status: Cannot be Unknown
    /// </remarks>
    public CreateProductRequestValidator()
    {
        RuleFor(product => product.Name).SetValidator((new NameValidator()));
        RuleFor(product => product.CurrentPrice)
            .GreaterThan(0)
            .WithMessage("Product price must be greater than zero.");
        RuleFor(product => product.Status).NotEqual(ProductStatus.Unknown);
    }
}

