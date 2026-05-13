using SalesProject.Domain.Enums;

namespace SalesProject.WebApi.Features.Products.GetAllProducts;

/// <summary>
/// Response model for GetAllProducts operation.
/// </summary>
public class GetAllProductsResponse
{
    /// <summary>
    /// The unique identifier of the product.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The product name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The current price of the product.
    /// </summary>
    public decimal CurrentPrice { get; set; }

    /// <summary>
    /// The current status of the product.
    /// </summary>
    public ProductStatus Status { get; set; }

    /// <summary>
    /// The date and time when the product was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// The date and time of the last update to the product.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
