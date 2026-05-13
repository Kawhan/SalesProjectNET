namespace SalesProject.WebApi.Features.Sales.GetSale;

/// <summary>
/// Request model for getting a Sale by ID
/// </summary>
public class GetSaleRequest
{
    /// <summary>
    /// The unique identifier of the sale to retrieve
    /// </summary>
    public Guid Id { get; set; }
}
