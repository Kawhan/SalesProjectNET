using SalesProject.Domain.Enums;

namespace SalesProject.Application.Products.UpdateProduct;

/// <summary>
/// Represents the response returned after successfully updating a product.
/// </summary>
/// <remarks>
/// This response contains the updated product data, including its unique identifier,
/// name, current price and status.
/// </remarks>
public class UpdateProductResult
{
    /// <summary>
    /// Gets or sets the unique identifier of the updated product.
    /// </summary>
    /// <value>A GUID that uniquely identifies the product in the system.</value>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the updated product name.
    /// </summary>
    /// <value>The product name registered in the system.</value>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the updated current price of the product.
    /// </summary>
    /// <value>The active price of the product after the update.</value>
    public decimal CurrentPrice { get; set; }

    /// <summary>
    /// The current status of the product.
    /// </summary>
    public ProductStatus Status { get; set; }
}
