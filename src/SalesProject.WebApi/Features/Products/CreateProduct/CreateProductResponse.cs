using SalesProject.Domain.Enums;

namespace SalesProject.WebApi.Features.Products.CreateProduct;

/// <summary>
/// API response model for CreateProduct operation
/// </summary>
public class CreateProductResponse
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

