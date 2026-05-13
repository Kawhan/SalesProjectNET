using FluentValidation;
using SalesProject.Domain.Enums;
using SalesProject.Domain.Validation;

namespace SalesProject.WebApi.Features.Products.UpdateProduct;

/// <summary>
/// Validator for UpdateProductRequest.
/// </summary>
public class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequest>
{
    /// <summary>
    /// Initializes a new instance of the UpdateProductRequestValidator with defined validation rules.
    /// </summary>
    /// <remarks>
    /// Validation rules include:
    /// - Name: Required, must be at least 3 characters long and at most 50 characters long
    /// - CurrentPrice: Must be greater than zero
    /// - Status: Cannot be Unknown
    /// </remarks>
    public UpdateProductRequestValidator()
    {
        RuleFor(product => product.Name)
            .SetValidator(new NameValidator());

        RuleFor(product => product.CurrentPrice)
            .GreaterThan(0)
            .WithMessage("Product price must be greater than zero.");

        RuleFor(product => product.Status)
            .NotEqual(ProductStatus.Unknown)
            .WithMessage("Product status cannot be unknown.");
    }
}
