using MediatR;
using SalesProject.Common.Validation;
using SalesProject.Domain.Enums;

namespace SalesProject.Application.Users.UpdateUser;

/// <summary>
/// Command for updating an existing user.
/// </summary>
/// <remarks>
/// This command is used to capture the required data for updating a user,
/// including the user identifier, username, phone, email, status and role.
/// It implements <see cref="IRequest{TResponse}"/> to initiate the request
/// that returns a <see cref="UpdateUserResult"/>.
/// </remarks>
public class UpdateUserCommand : IRequest<UpdateUserResult>
{
    /// <summary>
    /// Gets or sets the unique identifier of the user to update.
    /// </summary>
    public Guid Id { get; set; }

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

    /// <summary>
    /// Validates the current command using the UpdateUserCommandValidator.
    /// </summary>
    /// <returns>The validation result details.</returns>
    public ValidationResultDetail Validate()
    {
        var validator = new UpdateUserCommandValidator();
        var result = validator.Validate(this);

        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(o => (ValidationErrorDetail)o)
        };
    }
}