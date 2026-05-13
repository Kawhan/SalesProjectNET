using SalesProject.Application.Sales.CreateSale;
using FluentAssertions;

namespace SalesProject.Unit.Application.Sales.Validation;

/// <summary>
/// Contains unit tests for the <see cref="CreateSaleItemCommandValidator"/> class.
/// </summary>
public class CreateSaleItemCommandValidatorTests
{
    private readonly CreateSaleItemCommandValidator _validator;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateSaleItemCommandValidatorTests"/> class.
    /// </summary>
    public CreateSaleItemCommandValidatorTests()
    {
        _validator = new CreateSaleItemCommandValidator();
    }

    /// <summary>
    /// Tests that a valid create sale item command passes validation.
    /// </summary>
    [Fact(DisplayName = "Given valid sale item command When validating Then should not have errors")]
    public void Validate_ValidCommand_ShouldNotHaveValidationErrors()
    {
        // Given
        var command = new CreateSaleItemCommand
        {
            ProductId = Guid.NewGuid(),
            Quantity = 1
        };

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    /// <summary>
    /// Tests that an empty product id fails validation.
    /// </summary>
    [Fact(DisplayName = "Given empty product id When validating Then should have validation error")]
    public void Validate_EmptyProductId_ShouldHaveValidationError()
    {
        // Given
        var command = new CreateSaleItemCommand
        {
            ProductId = Guid.Empty,
            Quantity = 1
        };

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();

        result.Errors.Should().Contain(error =>
            error.PropertyName == nameof(CreateSaleItemCommand.ProductId) &&
            error.ErrorMessage == "Product id cannot be empty.");
    }

    /// <summary>
    /// Tests that zero quantity fails validation.
    /// </summary>
    [Fact(DisplayName = "Given zero quantity When validating Then should have validation error")]
    public void Validate_ZeroQuantity_ShouldHaveValidationError()
    {
        // Given
        var command = new CreateSaleItemCommand
        {
            ProductId = Guid.NewGuid(),
            Quantity = 0
        };

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();

        result.Errors.Should().Contain(error =>
            error.PropertyName == nameof(CreateSaleItemCommand.Quantity) &&
            error.ErrorMessage == "Product quantity must be greater than zero.");
    }

    /// <summary>
    /// Tests that negative quantity fails validation.
    /// </summary>
    [Fact(DisplayName = "Given negative quantity When validating Then should have validation error")]
    public void Validate_NegativeQuantity_ShouldHaveValidationError()
    {
        // Given
        var command = new CreateSaleItemCommand
        {
            ProductId = Guid.NewGuid(),
            Quantity = -1
        };

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();

        result.Errors.Should().Contain(error =>
            error.PropertyName == nameof(CreateSaleItemCommand.Quantity) &&
            error.ErrorMessage == "Product quantity must be greater than zero.");
    }
}