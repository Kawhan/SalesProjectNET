using MediatR;
using SalesProject.Common.Validation;
using SalesProject.Domain.Enums;

namespace SalesProject.Application.Products.UpdateProduct;

/// <summary>
/// Command for updating an existing product.
/// </summary>
/// <remarks>
/// This command is used to capture the required data for updating a product,
/// including the product unique identifier, name, current price and status.
/// It implements <see cref="IRequest{TResponse}"/> to initiate the request
/// that returns a <see cref="UpdateProductResult"/>.
/// 
/// The data provided in this command is validated using the
/// <see cref="UpdateProductCommandValidator"/> which extends
/// <see cref="FluentValidation.AbstractValidator{T}"/> to ensure that the fields are correctly
/// populated and follow the required rules.
/// </remarks>
public class UpdateProductCommand : IRequest<UpdateProductResult>
{
    /// <summary>
    /// Gets or sets the unique identifier of the product to be updated.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the updated product name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the updated current price of the product.
    /// </summary>
    public decimal CurrentPrice { get; set; }

    /// <summary>
    /// Gets or sets the updated status of the product.
    /// </summary>
    public ProductStatus Status { get; set; }

    /// <summary>
    /// Validates the current command using the UpdateProductCommandValidator.
    /// </summary>
    /// <returns>The validation result details.</returns>
    public ValidationResultDetail Validate()
    {
        var validator = new UpdateProductCommandValidator();
        var result = validator.Validate(this);

        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(o => (ValidationErrorDetail)o)
        };
    }
}