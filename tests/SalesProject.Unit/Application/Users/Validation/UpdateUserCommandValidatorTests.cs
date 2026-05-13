using SalesProject.Application.Users.UpdateUser;
using SalesProject.Domain.Enums;
using FluentAssertions;

namespace SalesProject.Unit.Application.Users.Validation;

/// <summary>
/// Contains unit tests for the <see cref="UpdateUserCommandValidator"/> class.
/// </summary>
public class UpdateUserCommandValidatorTests
{
    private readonly UpdateUserCommandValidator _validator;

    public UpdateUserCommandValidatorTests()
    {
        _validator = new UpdateUserCommandValidator();
    }

    [Fact(DisplayName = "Given valid user command When validating Then should not have errors")]
    public void Validate_ValidCommand_ShouldNotHaveValidationErrors()
    {
        // Given
        var command = GenerateValidCommand();

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
        var command = GenerateValidCommand();
        command.Id = Guid.Empty;

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error =>
            error.PropertyName == nameof(UpdateUserCommand.Id) &&
            error.ErrorMessage == "User id cannot be empty.");
    }

    [Fact(DisplayName = "Given empty username When validating Then should have validation error")]
    public void Validate_EmptyUsername_ShouldHaveValidationError()
    {
        // Given
        var command = GenerateValidCommand();
        command.Username = string.Empty;

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error =>
            error.PropertyName == nameof(UpdateUserCommand.Username) &&
            error.ErrorMessage == "Username cannot be empty.");
    }

    [Fact(DisplayName = "Given invalid email When validating Then should have validation error")]
    public void Validate_InvalidEmail_ShouldHaveValidationError()
    {
        // Given
        var command = GenerateValidCommand();
        command.Email = "invalid-email";

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error =>
            error.PropertyName == nameof(UpdateUserCommand.Email) &&
            error.ErrorMessage == "Email must be a valid email address.");
    }

    [Fact(DisplayName = "Given empty phone When validating Then should have validation error")]
    public void Validate_EmptyPhone_ShouldHaveValidationError()
    {
        // Given
        var command = GenerateValidCommand();
        command.Phone = string.Empty;

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error =>
            error.PropertyName == nameof(UpdateUserCommand.Phone) &&
            error.ErrorMessage == "Phone cannot be empty.");
    }

    [Fact(DisplayName = "Given unknown status When validating Then should have validation error")]
    public void Validate_UnknownStatus_ShouldHaveValidationError()
    {
        // Given
        var command = GenerateValidCommand();
        command.Status = UserStatus.Unknown;

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error =>
            error.PropertyName == nameof(UpdateUserCommand.Status) &&
            error.ErrorMessage == "User status cannot be unknown.");
    }

    [Fact(DisplayName = "Given none role When validating Then should have validation error")]
    public void Validate_NoneRole_ShouldHaveValidationError()
    {
        // Given
        var command = GenerateValidCommand();
        command.Role = UserRole.None;

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error =>
            error.PropertyName == nameof(UpdateUserCommand.Role) &&
            error.ErrorMessage == "User role cannot be none.");
    }

    private static UpdateUserCommand GenerateValidCommand()
    {
        return new UpdateUserCommand
        {
            Id = Guid.NewGuid(),
            Username = "JohnDoe",
            Email = "john.doe@email.com",
            Phone = "+5581999999999",
            Status = UserStatus.Active,
            Role = UserRole.Customer
        };
    }
}