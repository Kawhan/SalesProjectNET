using SalesProject.Domain.Enums;

namespace SalesProject.WebApi.Features.Products.GetProduct;

/// <summary>
/// API response model for GetProduct operation
/// </summary>
public class GetProductResponse
{
    /// <summary>
    /// The unique identifier of the created product
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The product's name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The product's current Price
    /// </summary>
    public decimal CurrentPrice { get; set; }

    /// <summary>
    /// The current status of the product
    /// </summary>
    public ProductStatus Status { get; set; }
}


