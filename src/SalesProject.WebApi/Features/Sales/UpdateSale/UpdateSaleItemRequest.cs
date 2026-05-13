namespace SalesProject.WebApi.Features.Sales.UpdateSale;

/// <summary>
/// Represents an item in a sale update request.
/// </summary>
public class UpdateSaleItemRequest
{
    /// <summary>
    /// Gets or sets the product identifier.
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// Gets or sets the updated quantity of the product.
    /// </summary>
    public int Quantity { get; set; }
}