using SalesProject.Application.Branches.DeleteBranch;
using FluentAssertions;

namespace SalesProject.Unit.Application.Branches.Validation;

/// <summary>
/// Contains unit tests for the <see cref="DeleteBranchValidator"/> class.
/// </summary>
public class DeleteBranchValidatorTests
{
    private readonly DeleteBranchValidator _validator;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteBranchValidatorTests"/> class.
    /// </summary>
    public DeleteBranchValidatorTests()
    {
        _validator = new DeleteBranchValidator();
    }

    /// <summary>
    /// Tests that a valid delete branch command passes validation.
    /// </summary>
    [Fact(DisplayName = "Given valid branch id When validating Then should not have errors")]
    public void Validate_ValidCommand_ShouldNotHaveValidationErrors()
    {
        // Given
        var command = new DeleteBranchCommand(Guid.NewGuid());

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    /// <summary>
    /// Tests that an empty branch id fails validation.
    /// </summary>
    [Fact(DisplayName = "Given empty branch id When validating Then should have validation error")]
    public void Validate_EmptyId_ShouldHaveValidationError()
    {
        // Given
        var command = new DeleteBranchCommand(Guid.Empty);

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();

        result.Errors.Should().Contain(error =>
            error.PropertyName == nameof(DeleteBranchCommand.Id) &&
            error.ErrorMessage == "Branch id cannot be empty.");
    }
}