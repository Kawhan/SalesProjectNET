using SalesProject.Application.Branches.GetBranch;
using FluentAssertions;

namespace SalesProject.Unit.Application.Branches.Validation;

/// <summary>
/// Contains unit tests for the <see cref="GetBranchCommandValidator"/> class.
/// </summary>
public class GetBranchCommandValidatorTests
{
    private readonly GetBranchCommandValidator _validator;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetBranchCommandValidatorTests"/> class.
    /// </summary>
    public GetBranchCommandValidatorTests()
    {
        _validator = new GetBranchCommandValidator();
    }

    /// <summary>
    /// Tests that a valid get branch command passes validation.
    /// </summary>
    [Fact(DisplayName = "Given valid branch id When validating Then should not have errors")]
    public void Validate_ValidCommand_ShouldNotHaveValidationErrors()
    {
        // Given
        var command = new GetBranchCommand(Guid.NewGuid());

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
        var command = new GetBranchCommand(Guid.Empty);

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();

        result.Errors.Should().Contain(error =>
            error.PropertyName == nameof(GetBranchCommand.Id) &&
            error.ErrorMessage == "Branch id cannot be empty.");
    }
}