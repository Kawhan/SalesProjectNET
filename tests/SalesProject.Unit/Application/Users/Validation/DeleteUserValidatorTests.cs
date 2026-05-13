using SalesProject.Application.Users.DeleteUser;
using FluentAssertions;

namespace SalesProject.Unit.Application.Users.Validation;

/// <summary>
/// Contains unit tests for the <see cref="DeleteUserValidator"/> class.
/// </summary>
public class DeleteUserValidatorTests
{
    private readonly DeleteUserValidator _validator;

    public DeleteUserValidatorTests()
    {
        _validator = new DeleteUserValidator();
    }

    [Fact(DisplayName = "Given valid user id When validating Then should not have errors")]
    public void Validate_ValidCommand_ShouldNotHaveValidationErrors()
    {
        // Given
        var command = new DeleteUserCommand(Guid.NewGuid());

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact(DisplayName = "Given empty user id When validating Then should have validation error")]
    public void Validate_EmptyId_ShouldHaveValidationError()
    {
        // Given
        var command = new DeleteUserCommand(Guid.Empty);

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();

        result.Errors.Should().Contain(error =>
            error.PropertyName == nameof(DeleteUserCommand.Id) &&
            error.ErrorMessage == "User ID is required");
    }
}
