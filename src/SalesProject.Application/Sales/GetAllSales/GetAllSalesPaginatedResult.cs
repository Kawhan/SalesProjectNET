namespace SalesProject.Application.Sales.GetAllSales;

/// <summary>
/// Represents a paginated result for the GetAllSales operation.
/// </summary>
/// <remarks>
/// This result contains the sales returned for the current page,
/// along with pagination metadata such as current page, total pages,
/// page size and total number of sales found.
/// </remarks>
public class GetAllSalesPaginatedResult
{
    /// <summary>
    /// Gets or sets the sales returned in the current page.
    /// </summary>
    public List<GetAllSalesResult> Data { get; set; } = new();

    /// <summary>
    /// Gets or sets the current page number.
    /// </summary>
    public int CurrentPage { get; set; }

    /// <summary>
    /// Gets or sets the total number of pages available.
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// Gets or sets the number of items per page.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Gets or sets the total number of sales found before pagination.
    /// </summary>
    public int TotalCount { get; set; }
}