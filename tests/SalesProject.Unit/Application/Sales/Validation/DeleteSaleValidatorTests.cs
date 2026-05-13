using SalesProject.Application.Sales.DeleteSale;
using FluentAssertions;

namespace SalesProject.Unit.Application.Sales.Validation;

/// <summary>
/// Contains unit tests for the <see cref="DeleteSaleValidator"/> class.
/// </summary>
public class DeleteSaleValidatorTests
{
    private readonly DeleteSaleValidator _validator;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteSaleValidatorTests"/> class.
    /// </summary>
    public DeleteSaleValidatorTests()
    {
        _validator = new DeleteSaleValidator();
    }

    /// <summary>
    /// Tests that a valid delete sale command passes validation.
    /// </summary>
    [Fact(DisplayName = "Given valid sale id When validating Then should not have errors")]
    public void Validate_ValidCommand_ShouldNotHaveValidationErrors()
    {
        // Given
        var command = new DeleteSaleCommand(Guid.NewGuid());

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
        var command = new DeleteSaleCommand(Guid.Empty);

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();

        result.Errors.Should().Contain(error =>
            error.PropertyName == nameof(DeleteSaleCommand.Id) &&
            error.ErrorMessage == "Sale ID is required");
    }
}