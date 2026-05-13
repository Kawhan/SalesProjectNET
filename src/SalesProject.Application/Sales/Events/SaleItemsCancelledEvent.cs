namespace SalesProject.Application.Sales.Events;

/// <summary>
/// Event published when one or more sale items are cancelled.
/// </summary>
public class SaleItemsCancelledEvent
{
    /// <summary>
    /// Gets or sets the sale identifier.
    /// </summary>
    public Guid SaleId { get; set; }

    /// <summary>
    /// Gets or sets the sale number.
    /// </summary>
    public string SaleNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date when the items were cancelled.
    /// </summary>
    public DateTime CancelledAt { get; set; }

    /// <summary>
    /// Gets or sets the cancelled sale items.
    /// </summary>
    public List<CancelledSaleItem> Items { get; set; } = [];
}