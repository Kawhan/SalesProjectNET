using FluentValidation;
using SalesProject.Domain.Enums;
using SalesProject.Domain.Validation;

namespace SalesProject.Application.Products.UpdateProduct;

/// <summary>
/// Validator for UpdateProductCommand.
/// </summary>
public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    /// <summary>
    /// Initializes a new instance of the UpdateProductCommandValidator with defined validation rules.
    /// </summary>
    /// <remarks>
    /// Validation rules include:
    /// - Id: Required and cannot be empty
    /// - Name: Required, must be at least 3 characters long and at most 50 characters long
    /// - CurrentPrice: Must be greater than zero
    /// - Status: Cannot be Unknown
    /// </remarks>
    public UpdateProductCommandValidator()
    {
        RuleFor(product => product.Id)
            .NotEmpty()
            .WithMessage("Product id cannot be empty.");

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

