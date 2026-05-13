using SalesProject.Application.Branches.CreateBranch;
using SalesProject.Domain.Enums;
using FluentAssertions;


namespace SalesProject.Unit.Application.Branches.Validation;

/// <summary>
/// Contains unit tests for the <see cref="CreateBranchCommandValidator"/> class.
/// </summary>
public class CreateBranchCommandValidatorTests
{
    private readonly CreateBranchCommandValidator _validator;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateBranchCommandValidatorTests"/> class.
    /// </summary>
    public CreateBranchCommandValidatorTests()
    {
        _validator = new CreateBranchCommandValidator();
    }

    /// <summary>
    /// Tests that a valid branch command passes validation.
    /// </summary>
    [Fact(DisplayName = "Given valid branch command When validating Then should not have errors")]
    public void Validate_ValidCommand_ShouldNotHaveValidationErrors()
    {
        // Given
        var command = new CreateBranchCommand
        {
            Name = "Main Branch",
            Address = "Main Street, 123",
            Status = BranchStatus.Active
        };

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    /// <summary>
    /// Tests that an empty branch name fails validation.
    /// </summary>
    [Fact(DisplayName = "Given empty branch name When validating Then should have validation error")]
    public void Validate_EmptyName_ShouldHaveValidationError()
    {
        // Given
        var command = new CreateBranchCommand
        {
            Name = string.Empty,
            Address = "Main Street, 123",
            Status = BranchStatus.Active
        };

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();

        result.Errors.Should().Contain(error =>
            error.PropertyName == nameof(CreateBranchCommand.Name) &&
            error.ErrorMessage == "Branch name cannot be empty.");
    }

    /// <summary>
    /// Tests that a branch name shorter than 3 characters fails validation.
    /// </summary>
    [Fact(DisplayName = "Given branch name shorter than minimum When validating Then should have validation error")]
    public void Validate_NameShorterThanMinimum_ShouldHaveValidationError()
    {
        // Given
        var command = new CreateBranchCommand
        {
            Name = "AB",
            Address = "Main Street, 123",
            Status = BranchStatus.Active
        };

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();

        result.Errors.Should().Contain(error =>
            error.PropertyName == nameof(CreateBranchCommand.Name) &&
            error.ErrorMessage == "Branch name must be at least 3 characters long.");
    }

    /// <summary>
    /// Tests that a branch name longer than 100 characters fails validation.
    /// </summary>
    [Fact(DisplayName = "Given branch name longer than maximum When validating Then should have validation error")]
    public void Validate_NameLongerThanMaximum_ShouldHaveValidationError()
    {
        // Given
        var command = new CreateBranchCommand
        {
            Name = new string('A', 101),
            Address = "Main Street, 123",
            Status = BranchStatus.Active
        };

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();

        result.Errors.Should().Contain(error =>
            error.PropertyName == nameof(CreateBranchCommand.Name) &&
            error.ErrorMessage == "Branch name must be at most 100 characters long.");
    }

    /// <summary>
    /// Tests that a branch name with exactly 3 characters passes validation.
    /// </summary>
    [Fact(DisplayName = "Given branch name with minimum length When validating Then should not have name error")]
    public void Validate_NameWithMinimumLength_ShouldNotHaveNameValidationError()
    {
        // Given
        var command = new CreateBranchCommand
        {
            Name = "ABC",
            Address = "Main Street, 123",
            Status = BranchStatus.Active
        };

        // When
        var result = _validator.Validate(command);

        // Then
        result.Errors.Should().NotContain(error =>
            error.PropertyName == nameof(CreateBranchCommand.Name));
    }

    /// <summary>
    /// Tests that a branch name with exactly 100 characters passes validation.
    /// </summary>
    [Fact(DisplayName = "Given branch name with maximum length When validating Then should not have name error")]
    public void Validate_NameWithMaximumLength_ShouldNotHaveNameValidationError()
    {
        // Given
        var command = new CreateBranchCommand
        {
            Name = new string('A', 100),
            Address = "Main Street, 123",
            Status = BranchStatus.Active
        };

        // When
        var result = _validator.Validate(command);

        // Then
        result.Errors.Should().NotContain(error =>
            error.PropertyName == nameof(CreateBranchCommand.Name));
    }

    /// <summary>
    /// Tests that a branch address longer than 250 characters fails validation.
    /// </summary>
    [Fact(DisplayName = "Given branch address longer than maximum When validating Then should have validation error")]
    public void Validate_AddressLongerThanMaximum_ShouldHaveValidationError()
    {
        // Given
        var command = new CreateBranchCommand
        {
            Name = "Main Branch",
            Address = new string('A', 251),
            Status = BranchStatus.Active
        };

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();

        result.Errors.Should().Contain(error =>
            error.PropertyName == nameof(CreateBranchCommand.Address) &&
            error.ErrorMessage == "Branch address must be at most 250 characters long.");
    }

    /// <summary>
    /// Tests that a branch address with exactly 250 characters passes validation.
    /// </summary>
    [Fact(DisplayName = "Given branch address with maximum length When validating Then should not have address error")]
    public void Validate_AddressWithMaximumLength_ShouldNotHaveAddressValidationError()
    {
        // Given
        var command = new CreateBranchCommand
        {
            Name = "Main Branch",
            Address = new string('A', 250),
            Status = BranchStatus.Active
        };

        // When
        var result = _validator.Validate(command);

        // Then
        result.Errors.Should().NotContain(error =>
            error.PropertyName == nameof(CreateBranchCommand.Address));
    }

    /// <summary>
    /// Tests that an unknown branch status fails validation.
    /// </summary>
    [Fact(DisplayName = "Given unknown branch status When validating Then should have validation error")]
    public void Validate_UnknownStatus_ShouldHaveValidationError()
    {
        // Given
        var command = new CreateBranchCommand
        {
            Name = "Main Branch",
            Address = "Main Street, 123",
            Status = BranchStatus.Unknown
        };

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();

        result.Errors.Should().Contain(error =>
            error.PropertyName == nameof(CreateBranchCommand.Status) &&
            error.ErrorMessage == "Branch status cannot be unknown.");
    }

    /// <summary>
    /// Tests that a valid branch status passes validation.
    /// </summary>
    [Fact(DisplayName = "Given valid branch status When validating Then should not have status error")]
    public void Validate_ValidStatus_ShouldNotHaveStatusValidationError()
    {
        // Given
        var command = new CreateBranchCommand
        {
            Name = "Main Branch",
            Address = "Main Street, 123",
            Status = BranchStatus.Active
        };

        // When
        var result = _validator.Validate(command);

        // Then
        result.Errors.Should().NotContain(error =>
            error.PropertyName == nameof(CreateBranchCommand.Status));
    }
}