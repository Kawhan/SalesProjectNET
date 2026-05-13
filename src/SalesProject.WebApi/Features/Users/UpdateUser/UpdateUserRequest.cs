using SalesProject.Domain.Enums;

namespace SalesProject.WebApi.Features.Users.UpdateUser;

/// <summary>
/// Represents a request to update an existing user in the system.
/// </summary>
public class UpdateUserRequest
{
    /// <summary>
    /// Gets or sets the updated username.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the updated phone number.
    /// </summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the updated email address.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the updated status of the user.
    /// </summary>
    public UserStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the updated role of the user.
    /// </summary>
    public UserRole Role { get; set; }
}