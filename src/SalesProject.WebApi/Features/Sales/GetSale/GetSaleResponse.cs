using SalesProject.Domain.Enums;

namespace SalesProject.WebApi.Features.Sales.GetSale;

/// <summary>
/// Response model for GetSale operation.
/// </summary>
/// <remarks>
/// This response contains the complete sale information, including sale number,
/// sale date, user, branch, total amount, sale status and the items included in the sale.
/// </remarks>
public class GetSaleResponse
{
    /// <summary>
    /// Gets or sets the unique identifier of the sale.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the sale number.
    /// </summary>
    public string SaleNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date when the sale was made.
    /// </summary>
    public DateTime SaleDate { get; set; }

    /// <summary>
    /// Gets or sets the user identifier associated with the sale.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the user associated with the sale.
    /// </summary>
    public GetSaleUserResponse User { get; set; } = null!;

    /// <summary>
    /// Gets or sets the branch identifier where the sale was made.
    /// </summary>
    public Guid BranchId { get; set; }

    /// <summary>
    /// Gets or sets the branch where the sale was made.
    /// </summary>
    public GetSaleBranchResponse Branch { get; set; } = null!;

    /// <summary>
    /// Gets or sets the total amount of the sale.
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Gets or sets the current status of the sale.
    /// </summary>
    public SaleStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the sale was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time of the last update to the sale information.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the list of items included in the sale.
    /// </summary>
    public List<GetSaleItemResponse> Items { get; set; } = new();
}