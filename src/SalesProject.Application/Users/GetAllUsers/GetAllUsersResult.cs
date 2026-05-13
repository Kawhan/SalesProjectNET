using SalesProject.Domain.Enums;

namespace SalesProject.Application.Users.GetAllUsers;

/// <summary>
/// Response model for the GetAllUsers operation.
/// </summary>
/// <remarks>
/// This result represents a user returned in a paginated user list,
/// including identification, contact information, role and status.
/// </remarks>
public class GetAllUsersResult
{
    /// <summary>
    /// Gets or sets the unique identifier of the user.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the user's Username.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's email address.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's phone number.
    /// </summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's role in the system.
    /// </summary>
    public UserRole Role { get; set; }

    /// <summary>
    /// Gets or sets the current status of the user.
    /// </summary>
    public UserStatus Status { get; set; }
}