using SalesProject.Domain.Enums;

namespace SalesProject.WebApi.Features.Products.CreateProduct;

/// <summary>
/// Represents a request to create a new product in the system.
/// </summary>
public class CreateProductRequest
{
    /// <summary>
    /// Gets or sets the name. Must be unique and contain only valid characters.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the current price of the product.
    /// Must be greater than zero and represents the active price used for new sales.
    /// </summary>
    public decimal CurrentPrice { get; set; }

    /// <summary>
    /// Gets or sets the initial status of the product account.
    /// </summary>
    public ProductStatus Status { get; set; }
}
