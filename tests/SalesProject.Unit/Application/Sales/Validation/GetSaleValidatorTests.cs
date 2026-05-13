using SalesProject.Application.Sales.GetSale;
using FluentAssertions;

namespace SalesProject.Unit.Application.Sales.Validation;

/// <summary>
/// Contains unit tests for the <see cref="GetSaleValidator"/> class.
/// </summary>
public class GetSaleValidatorTests
{
    private readonly GetSaleValidator _validator;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetSaleValidatorTests"/> class.
    /// </summary>
    public GetSaleValidatorTests()
    {
        _validator = new GetSaleValidator();
    }

    /// <summary>
    /// Tests that a valid get sale command passes validation.
    /// </summary>
    [Fact(DisplayName = "Given valid sale id When validating Then should not have errors")]
    public void Validate_ValidCommand_ShouldNotHaveValidationErrors()
    {
        // Given
        var command = new GetSaleCommand(Guid.NewGuid());

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
        var command = new GetSaleCommand(Guid.Empty);

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();

        result.Errors.Should().Contain(error =>
            error.PropertyName == nameof(GetSaleCommand.Id) &&
            error.ErrorMessage == "Sale ID is required");
    }
}