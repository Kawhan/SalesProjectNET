using SalesProject.Domain.Enums;

namespace SalesProject.Application.Products.GetProduct;

/// <summary>
/// Response model for GetProduct operation
/// </summary>
public class GetProductResult
{
    /// <summary>
    /// The unique identifier of the product
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The product name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The current price of the product
    /// </summary>
    public decimal CurrentPrice { get; set; }

    /// <summary>
    /// The current status of the product
    /// </summary>
    public ProductStatus Status { get; set; }

}

