using SalesProject.Domain.Enums;

namespace SalesProject.Application.Sales.GetSale;

/// <summary>
/// Represents the product returned in the GetSale operation.
/// </summary>
public class GetSaleProductResult
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
    /// Gets or sets the current product price.
    /// </summary>
    public decimal CurrentPrice { get; set; }

    /// <summary>
    /// Gets or sets the current status of the product.
    /// </summary>
    public ProductStatus Status { get; set; }
}

