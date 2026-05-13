namespace SalesProject.Application.Sales.UpdateSale;

/// <summary>
/// Command item for updating a sale item.
/// </summary>
public class UpdateSaleItemCommand
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
