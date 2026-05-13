namespace SalesProject.WebApi.Features.Sales.CreateSale;

/// <summary>
/// Represents a product item in a sale creation request.
/// </summary>
public class CreateSaleItemRequest
{
    /// <summary>
    /// Gets or sets the product identifier.
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// Gets or sets the quantity of the product to be sold.
    /// </summary>
    public int Quantity { get; set; }
}
