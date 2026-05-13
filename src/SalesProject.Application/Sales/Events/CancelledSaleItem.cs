namespace SalesProject.Application.Sales.Events;

/// <summary>
/// Represents a cancelled item from a sale.
/// </summary>
public class CancelledSaleItem
{
    /// <summary>
    /// Gets or sets the sale item identifier.
    /// </summary>
    public Guid SaleItemId { get; set; }

    /// <summary>
    /// Gets or sets the product identifier.
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// Gets or sets the quantity that was cancelled.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the unit price at the moment of the sale.
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Gets or sets the discount percentage applied to the item.
    /// </summary>
    public decimal DiscountPercentage { get; set; }

    /// <summary>
    /// Gets or sets the discount amount applied to the item.
    /// </summary>
    public decimal Discount { get; set; }

    /// <summary>
    /// Gets or sets the total amount of the item after discount.
    /// </summary>
    public decimal TotalAmount { get; set; }
}