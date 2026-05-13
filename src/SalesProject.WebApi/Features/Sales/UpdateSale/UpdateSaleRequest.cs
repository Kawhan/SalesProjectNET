namespace SalesProject.WebApi.Features.Sales.UpdateSale;

/// <summary>
/// Represents a request to update an existing sale in the system.
/// </summary>
public class UpdateSaleRequest
{
    /// <summary>
    /// Gets or sets the user identifier associated with the sale.
    /// This user represents the customer who made the purchase.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the branch identifier where the sale was made.
    /// </summary>
    public Guid BranchId { get; set; }

    /// <summary>
    /// Gets or sets the list of products included in the sale.
    /// Items not included in this list may be cancelled during the update.
    /// </summary>
    public List<UpdateSaleItemRequest> Items { get; set; } = new();
}