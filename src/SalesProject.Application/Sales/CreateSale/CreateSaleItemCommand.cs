namespace SalesProject.Application.Sales.CreateSale;

/// <summary>
/// Command item for creating a new sale item.
/// </summary>
public class CreateSaleItemCommand
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