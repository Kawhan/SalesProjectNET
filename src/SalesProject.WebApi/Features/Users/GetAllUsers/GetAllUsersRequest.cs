using SalesProject.Domain.Enums;

namespace SalesProject.WebApi.Features.Users.GetAllUsers;

/// <summary>
/// Represents a request to retrieve users with pagination and filters.
/// </summary>
public class GetAllUsersRequest
{
    /// <summary>
    /// Gets or sets the page number.
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Gets or sets the number of items per page.
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Gets or sets the username filter.
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// Gets or sets the user email filter.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets the user phone filter.
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Gets or sets the user role filter.
    /// </summary>
    public UserRole? Role { get; set; }

    /// <summary>
    /// Gets or sets the user status filter.
    /// </summary>
    public UserStatus? Status { get; set; }
}