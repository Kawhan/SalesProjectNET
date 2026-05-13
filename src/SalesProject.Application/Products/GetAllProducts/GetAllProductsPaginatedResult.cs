namespace SalesProject.Application.Products.GetAllProducts;

/// <summary>
/// Represents a paginated result for the GetAllProducts operation.
/// </summary>
/// <remarks>
/// This result contains the products returned for the current page,
/// along with pagination metadata such as current page, total pages,
/// page size and total number of products found.
/// </remarks>
public class GetAllProductsPaginatedResult
{
    /// <summary>
    /// Gets or sets the products returned in the current page.
    /// </summary>
    public List<GetAllProductsResult> Data { get; set; } = new();

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
    /// Gets or sets the total number of products found before pagination.
    /// </summary>
    public int TotalCount { get; set; }
}