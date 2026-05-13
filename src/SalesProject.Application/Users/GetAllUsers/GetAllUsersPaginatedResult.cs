namespace SalesProject.Application.Users.GetAllUsers;

/// <summary>
/// Represents a paginated result for the GetAllUsers operation.
/// </summary>
/// <remarks>
/// This result contains the users returned for the current page,
/// along with pagination metadata such as current page, total pages,
/// page size and total number of users found.
/// </remarks>
public class GetAllUsersPaginatedResult
{
    /// <summary>
    /// Gets or sets the users returned in the current page.
    /// </summary>
    public List<GetAllUsersResult> Data { get; set; } = new();

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
    /// Gets or sets the total number of users found before pagination.
    /// </summary>
    public int TotalCount { get; set; }
}