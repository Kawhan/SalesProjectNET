using SalesProject.Application.Sales.ReactivateSale;
using FluentAssertions;

namespace SalesProject.Unit.Application.Sales.Validation;

/// <summary>
/// Contains unit tests for the <see cref="ReactivateSaleValidator"/> class.
/// </summary>
public class ReactivateSaleValidatorTests
{
    private readonly ReactivateSaleValidator _validator;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactivateSaleValidatorTests"/> class.
    /// </summary>
    public ReactivateSaleValidatorTests()
    {
        _validator = new ReactivateSaleValidator();
    }

    /// <summary>
    /// Tests that a valid reactivate sale command passes validation.
    /// </summary>
    [Fact(DisplayName = "Given valid sale id When validating Then should not have errors")]
    public void Validate_ValidCommand_ShouldNotHaveValidationErrors()
    {
        // Given
        var command = new ReactivateSaleCommand(Guid.NewGuid());

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    /// <summary>
    /// Tests that an empty sale id fails validation.
    /// </summary>
    [Fact(DisplayName = "Given empty sale id When validating Then should have validation error")]
    public void Validate_EmptyId_ShouldHaveValidationError()
    {
        // Given
        var command = new ReactivateSaleCommand(Guid.Empty);

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();

        result.Errors.Should().Contain(error =>
            error.PropertyName == nameof(ReactivateSaleCommand.Id) &&
            error.ErrorMessage == "Sale id cannot be empty.");
    }
}