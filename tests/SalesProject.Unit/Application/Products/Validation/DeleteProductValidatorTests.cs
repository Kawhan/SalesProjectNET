using SalesProject.Application.Products.DeleteProduct;
using FluentAssertions;

namespace SalesProject.Unit.Application.Products.Validation;

/// <summary>
/// Contains unit tests for the <see cref="DeleteProductValidator"/> class.
/// </summary>
public class DeleteProductValidatorTests
{
    private readonly DeleteProductValidator _validator;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteProductValidatorTests"/> class.
    /// </summary>
    public DeleteProductValidatorTests()
    {
        _validator = new DeleteProductValidator();
    }

    /// <summary>
    /// Tests that a valid delete product command passes validation.
    /// </summary>
    [Fact(DisplayName = "Given valid product id When validating Then should not have errors")]
    public void Validate_ValidCommand_ShouldNotHaveValidationErrors()
    {
        // Given
        var command = new DeleteProductCommand(Guid.NewGuid());

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
    public void Validate_EmptyId_ShouldHaveValidationError()
    {
        // Given
        var command = new DeleteProductCommand(Guid.Empty);

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();

        result.Errors.Should().Contain(error =>
            error.PropertyName == nameof(DeleteProductCommand.Id) &&
            error.ErrorMessage == "Product ID is required");
    }
}
