using SalesProject.Domain.Enums;

namespace SalesProject.WebApi.Features.Users.UpdateUser;

/// <summary>
/// Represents the response returned after successfully updating a user.
/// </summary>
public class UpdateUserResponse
{
    /// <summary>
    /// Gets or sets the unique identifier of the updated user.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the updated username.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the updated email address.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the updated phone number.
    /// </summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the current role of the user.
    /// </summary>
    public UserRole Role { get; set; }

    /// <summary>
    /// Gets or sets the current status of the user.
    /// </summary>
    public UserStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the user was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the user was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}

