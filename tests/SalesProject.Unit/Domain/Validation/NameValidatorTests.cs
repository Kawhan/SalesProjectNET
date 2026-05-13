using SalesProject.Domain.Validation;
using FluentValidation.TestHelper;


namespace SalesProject.Unit.Domain.Validation;

/// <summary>
/// Contains unit tests for the NameValidator class.
/// Tests cover validation of product name requirements,
/// including empty, minimum length and maximum length rules.
/// </summary>
public class NameValidatorTests
{
    private readonly NameValidator _validator;

    public NameValidatorTests()
    {
        _validator = new NameValidator();
    }

    /// <summary>
    /// Tests that validation passes when the product name is valid.
    /// This test verifies that a product name with:
    /// - At least 3 characters
    /// - At most 50 characters
    /// passes all validation rules without any errors.
    /// </summary>
    [Fact(DisplayName = "Valid product name should pass all validation rules")]
    public void Given_ValidName_When_Validated_Then_ShouldNotHaveErrors()
    {
        // Arrange
        var name = "Product Name";

        // Act
        var result = _validator.TestValidate(name);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    /// <summary>
    /// Tests that validation fails when the product name is empty.
    /// </summary>
    [Fact(DisplayName = "Empty product name should fail validation")]
    public void Given_EmptyName_When_Validated_Then_ShouldHaveError()
    {
        // Arrange
        var name = string.Empty;

        // Act
        var result = _validator.TestValidate(name);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("The product name cannot be empty.");
    }

    /// <summary>
    /// Tests that validation fails when the product name is shorter than 3 characters.
    /// </summary>
    [Fact(DisplayName = "Product name shorter than minimum length should fail validation")]
    public void Given_NameShorterThanMinimum_When_Validated_Then_ShouldHaveError()
    {
        // Arrange
        var name = "AB";

        // Act
        var result = _validator.TestValidate(name);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("The product name must be at least 3 characters long.");
    }

    /// <summary>
    /// Tests that validation fails when the product name is longer than 50 characters.
    /// </summary>
    [Fact(DisplayName = "Product name longer than maximum length should fail validation")]
    public void Given_NameLongerThanMaximum_When_Validated_Then_ShouldHaveError()
    {
        // Arrange
        var name = new string('A', 51);

        // Act
        var result = _validator.TestValidate(name);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("The product name must be at most 50 characters long.");
    }

    /// <summary>
    /// Tests that validation passes when the product name has exactly 3 characters.
    /// </summary>
    [Fact(DisplayName = "Product name with minimum length should pass validation")]
    public void Given_NameWithMinimumLength_When_Validated_Then_ShouldNotHaveErrors()
    {
        // Arrange
        var name = "ABC";

        // Act
        var result = _validator.TestValidate(name);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    /// <summary>
    /// Tests that validation passes when the product name has exactly 50 characters.
    /// </summary>
    [Fact(DisplayName = "Product name with maximum length should pass validation")]
    public void Given_NameWithMaximumLength_When_Validated_Then_ShouldNotHaveErrors()
    {
        // Arrange
        var name = new string('A', 50);

        // Act
        var result = _validator.TestValidate(name);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}