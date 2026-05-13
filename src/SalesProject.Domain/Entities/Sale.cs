using SalesProject.Domain.Common;
using SalesProject.Domain.Enums;

namespace SalesProject.Domain.Entities;

/// <summary>
/// Represents a sale made in the system.
/// This entity stores the sale header information, including sale number,
/// sale date, customer/user, branch, total amount and cancellation status.
/// </summary>
public class Sale : BaseEntity
{
    /// <summary>
    /// Gets or sets the sale number.
    /// This value identifies the sale from a business perspective.
    /// </summary>
    public string SaleNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date when the sale was made.
    /// </summary>
    public DateTime SaleDate { get; set; }

    /// <summary>
    /// Gets or sets the user identifier associated with the sale.
    /// This user represents the customer who made the purchase.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the user associated with the sale.
    /// This navigation property represents the customer who made the purchase.
    /// </summary>
    public User User { get; set; } = null!;

    /// <summary>
    /// Gets or sets the branch identifier where the sale was made.
    /// </summary>
    public Guid BranchId { get; set; }

    /// <summary>
    /// Gets or sets the branch where the sale was made.
    /// </summary>
    public Branch Branch { get; set; } = null!;

    /// <summary>
    /// Gets or sets the total amount of the sale.
    /// This value represents the final amount after applying item discounts.
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Gets or sets the current status of the sale.
    /// Indicates whether the sale is active or cancelled.
    /// </summary>
    public SaleStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the list of items included in the sale.
    /// Each item represents a product, its quantity, unit price, discount and total amount.
    /// </summary>
    public List<SaleItem> Items { get; set; } = new();

    /// <summary>
    /// Gets the date and time when the sale was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets the date and time of the last update to the sale's information.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Initializes a new instance of the Sale class.
    /// </summary>
    public Sale()
    {
        CreatedAt = DateTime.UtcNow;
        SaleDate = DateTime.UtcNow;
        Status = SaleStatus.Active;
        GenerateSaleNumber();
    }

    /// <summary>
    /// Generates a new sale number based on the current date and time.
    /// Changes the sale's sale number.
    /// </summary>
    public void GenerateSaleNumber()
    {
        SaleNumber = $"SALE-{DateTime.UtcNow:yyyyMMddHHmmss}";
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Activates the sale.
    /// Changes the sales's status to Active.
    /// </summary>
    public void Activate()
    {
        Status = SaleStatus.Active;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Deactivates the sale.
    /// Changes the sale's status to Inactive.
    /// </summary>
    public void Deactivate()
    {
        Status = SaleStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the sale header information.
    /// </summary>
    /// <param name="userId">The updated user identifier.</param>
    /// <param name="branchId">The updated branch identifier.</param>
    public void Update(Guid userId, Guid branchId)
    {
        UserId = userId;
        BranchId = branchId;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Recalculates the sale total amount based on active items.
    /// </summary>
    public void RecalculateTotal()
    {
        TotalAmount = Items
            .Where(item => item.Status == SaleItemStatus.Active)
            .Sum(item => item.TotalAmount);

        UpdatedAt = DateTime.UtcNow;
    }
}
