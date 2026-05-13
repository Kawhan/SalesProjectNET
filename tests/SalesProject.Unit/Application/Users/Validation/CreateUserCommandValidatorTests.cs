using SalesProject.Application.Users.CreateUser;
using SalesProject.Domain.Enums;
using FluentAssertions;

namespace SalesProject.Unit.Application.Users.Validation;

/// <summary>
/// Contains unit tests for the <see cref="CreateUserCommandValidator"/> class.
/// </summary>
public class CreateUserCommandValidatorTests
{
    private readonly CreateUserCommandValidator _validator;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateUserCommandValidatorTests"/> class.
    /// </summary>
    public CreateUserCommandValidatorTests()
    {
        _validator = new CreateUserCommandValidator();
    }

    /// <summary>
    /// Tests that a valid create user command passes validation.
    /// </summary>
    [Fact(DisplayName = "Given valid user command When validating Then should not have errors")]
    public void Validate_ValidCommand_ShouldNotHaveValidationErrors()
    {
        // Given
        var command = new CreateUserCommand
        {
            Username = "JohnDoe",
            Password = "Test@123",
            Email = "john.doe@email.com",
            Phone = "+5581999999999",
            Status = UserStatus.Active,
            Role = UserRole.Customer
        };

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    /// <summary>
    /// Tests that an empty email fails validation.
    /// </summary>
    [Fact(DisplayName = "Given empty email When validating Then should have validation error")]
    public void Validate_EmptyEmail_ShouldHaveValidationError()
    {
        // Given
        var command = GenerateValidCommand();
        command.Email = string.Empty;

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();

        result.Errors.Should().Contain(error =>
            error.PropertyName == nameof(CreateUserCommand.Email) &&
            error.ErrorMessage == "The email address cannot be empty.");
    }

    /// <summary>
    /// Tests that an invalid email fails validation.
    /// </summary>
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
            error.PropertyName == nameof(CreateUserCommand.Email) &&
            error.ErrorMessage == "The provided email address is not valid.");
    }

    /// <summary>
    /// Tests that an email longer than maximum fails validation.
    /// </summary>
    [Fact(DisplayName = "Given email longer than maximum When validating Then should have validation error")]
    public void Validate_EmailLongerThanMaximum_ShouldHaveValidationError()
    {
        // Given
        var command = GenerateValidCommand();
        command.Email = $"{new string('a', 101)}@email.com";

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();

        result.Errors.Should().Contain(error =>
            error.PropertyName == nameof(CreateUserCommand.Email) &&
            error.ErrorMessage == "The email address cannot be longer than 100 characters.");
    }

    /// <summary>
    /// Tests that an empty username fails validation.
    /// </summary>
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
            error.PropertyName == nameof(CreateUserCommand.Username));
    }

    /// <summary>
    /// Tests that a username shorter than minimum fails validation.
    /// </summary>
    [Fact(DisplayName = "Given username shorter than minimum When validating Then should have validation error")]
    public void Validate_UsernameShorterThanMinimum_ShouldHaveValidationError()
    {
        // Given
        var command = GenerateValidCommand();
        command.Username = "Jo";

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();

        result.Errors.Should().Contain(error =>
            error.PropertyName == nameof(CreateUserCommand.Username));
    }

    /// <summary>
    /// Tests that a username longer than maximum fails validation.
    /// </summary>
    [Fact(DisplayName = "Given username longer than maximum When validating Then should have validation error")]
    public void Validate_UsernameLongerThanMaximum_ShouldHaveValidationError()
    {
        // Given
        var command = GenerateValidCommand();
        command.Username = new string('A', 51);

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();

        result.Errors.Should().Contain(error =>
            error.PropertyName == nameof(CreateUserCommand.Username));
    }

    /// <summary>
    /// Tests that an empty password fails validation.
    /// </summary>
    [Fact(DisplayName = "Given empty password When validating Then should have validation error")]
    public void Validate_EmptyPassword_ShouldHaveValidationError()
    {
        // Given
        var command = GenerateValidCommand();
        command.Password = string.Empty;

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();

        result.Errors.Should().Contain(error =>
            error.PropertyName == nameof(CreateUserCommand.Password));
    }

    /// <summary>
    /// Tests that a password without uppercase letter fails validation.
    /// </summary>
    [Fact(DisplayName = "Given password without uppercase When validating Then should have validation error")]
    public void Validate_PasswordWithoutUppercase_ShouldHaveValidationError()
    {
        // Given
        var command = GenerateValidCommand();
        command.Password = "test@123";

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();

        result.Errors.Should().Contain(error =>
            error.PropertyName == nameof(CreateUserCommand.Password) &&
            error.ErrorMessage == "Password must contain at least one uppercase letter.");
    }

    /// <summary>
    /// Tests that a password without lowercase letter fails validation.
    /// </summary>
    [Fact(DisplayName = "Given password without lowercase When validating Then should have validation error")]
    public void Validate_PasswordWithoutLowercase_ShouldHaveValidationError()
    {
        // Given
        var command = GenerateValidCommand();
        command.Password = "TEST@123";

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();

        result.Errors.Should().Contain(error =>
            error.PropertyName == nameof(CreateUserCommand.Password) &&
            error.ErrorMessage == "Password must contain at least one lowercase letter.");
    }

    /// <summary>
    /// Tests that a password without number fails validation.
    /// </summary>
    [Fact(DisplayName = "Given password without number When validating Then should have validation error")]
    public void Validate_PasswordWithoutNumber_ShouldHaveValidationError()
    {
        // Given
        var command = GenerateValidCommand();
        command.Password = "Test@Test";

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();

        result.Errors.Should().Contain(error =>
            error.PropertyName == nameof(CreateUserCommand.Password) &&
            error.ErrorMessage == "Password must contain at least one number.");
    }

    /// <summary>
    /// Tests that a password without special character fails validation.
    /// </summary>
    [Fact(DisplayName = "Given password without special character When validating Then should have validation error")]
    public void Validate_PasswordWithoutSpecialCharacter_ShouldHaveValidationError()
    {
        // Given
        var command = GenerateValidCommand();
        command.Password = "Test1234";

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();

        result.Errors.Should().Contain(error =>
            error.PropertyName == nameof(CreateUserCommand.Password) &&
            error.ErrorMessage == "Password must contain at least one special character.");
    }

    /// <summary>
    /// Tests that an invalid phone fails validation.
    /// </summary>
    [Fact(DisplayName = "Given invalid phone When validating Then should have validation error")]
    public void Validate_InvalidPhone_ShouldHaveValidationError()
    {
        // Given
        var command = GenerateValidCommand();
        command.Phone = "invalid-phone";

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();

        result.Errors.Should().Contain(error =>
            error.PropertyName == nameof(CreateUserCommand.Phone));
    }

    /// <summary>
    /// Tests that an unknown status fails validation.
    /// </summary>
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
            error.PropertyName == nameof(CreateUserCommand.Status));
    }

    /// <summary>
    /// Tests that a none role fails validation.
    /// </summary>
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
            error.PropertyName == nameof(CreateUserCommand.Role));
    }

    /// <summary>
    /// Generates a valid CreateUserCommand for validation tests.
    /// </summary>
    private static CreateUserCommand GenerateValidCommand()
    {
        return new CreateUserCommand
        {
            Username = "JohnDoe",
            Password = "Test@123",
            Email = "john.doe@email.com",
            Phone = "+5581999999999",
            Status = UserStatus.Active,
            Role = UserRole.Customer
        };
    }
}