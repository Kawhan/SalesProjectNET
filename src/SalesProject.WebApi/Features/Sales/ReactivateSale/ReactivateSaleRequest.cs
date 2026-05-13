namespace SalesProject.WebApi.Features.Sales.ReactivateSale;

/// <summary>
/// Request model for reactivate an sale
/// </summary>
public class ReactivateSaleRequest
{
    /// <summary>
    /// The unique identifier of the sale to reactiave
    /// </summary>
    public Guid Id { get; set; }
}

