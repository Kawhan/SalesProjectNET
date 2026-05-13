using SalesProject.Domain.Enums;

namespace SalesProject.WebApi.Features.Products.UpdateProduct;

/// <summary>
/// Represents a request to update an existing product in the system.
/// </summary>
public class UpdateProductRequest
{
    /// <summary>
    /// Gets or sets the updated product name.
    /// Must be unique and contain only valid characters.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the updated current price of the product.
    /// Must be greater than zero and represents the active price used for new sales.
    /// </summary>
    public decimal CurrentPrice { get; set; }

    /// <summary>
    /// Gets or sets the updated status of the product.
    /// </summary>
    public ProductStatus Status { get; set; }
}

