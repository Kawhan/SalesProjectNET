namespace SalesProject.WebApi.Features.Sales.DeleteSale;

/// <summary>
/// Request model for deleting an sale
/// </summary>

public class DeleteSaleRequest
{
    /// <summary>
    /// The unique identifier of the sale to delete
    /// </summary>
    public Guid Id { get; set; }
}
