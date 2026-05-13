using SalesProject.Domain.Enums;

namespace SalesProject.Application.Products.GetAllProducts;

/// <summary>
/// Response model for the GetAllProducts operation.
/// </summary>
/// <remarks>
/// This result represents a product returned in a paginated product list,
/// including its identification, price, status and audit information.
/// </remarks>
public class GetAllProductsResult
{
    /// <summary>
    /// Gets or sets the unique identifier of the product.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the product name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the current price of the product.
    /// </summary>
    public decimal CurrentPrice { get; set; }

    /// <summary>
    /// Gets or sets the current status of the product.
    /// </summary>
    public ProductStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the product was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the product was last updated.
    /// This value can be null when the product has not been updated yet.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}