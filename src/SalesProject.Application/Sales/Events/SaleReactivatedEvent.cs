namespace SalesProject.Application.Sales.Events;

/// <summary>
/// Event raised when a sale is successfully reactivated.
/// </summary>
public class SaleReactivatedEvent
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
    /// Gets or sets the user identifier associated with the sale.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the branch identifier associated with the sale.
    /// </summary>
    public Guid BranchId { get; set; }

    /// <summary>
    /// Gets or sets the total sale amount.
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Gets or sets the date when the sale was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date when the sale was updated or reactivated.
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
