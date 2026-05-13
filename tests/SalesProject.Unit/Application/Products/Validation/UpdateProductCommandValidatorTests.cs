using SalesProject.Application.Products.UpdateProduct;
using SalesProject.Domain.Enums;
using FluentAssertions;

namespace SalesProject.Unit.Application.Products.Validation;

/// <summary>
/// Contains unit tests for the <see cref="UpdateProductCommandValidator"/> class.
/// </summary>
public class UpdateProductCommandValidatorTests
{
    private readonly UpdateProductCommandValidator _validator;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateProductCommandValidatorTests"/> class.
    /// </summary>
    public UpdateProductCommandValidatorTests()
    {
        _validator = new UpdateProductCommandValidator();
    }

    /// <summary>
    /// Tests that a valid update product command passes validation.
    /// </summary>
    [Fact(DisplayName = "Given valid product command When validating Then should not have errors")]
    public void Validate_ValidCommand_ShouldNotHaveValidationErrors()
    {
        // Given
        var command = new UpdateProductCommand
        {
            Id = Guid.NewGuid(),
            Name = "Product Test",
            CurrentPrice = 10,
            Status = ProductStatus.Active
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
    public void Validate_EmptyId_ShouldHaveValidationError()
    {
        // Given
        var command = new UpdateProductCommand
        {
            Id = Guid.Empty,
            Name = "Product Test",
            CurrentPrice = 10,
            Status = ProductStatus.Active
        };

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();

        result.Errors.Should().Contain(error =>
            error.PropertyName == nameof(UpdateProductCommand.Id) &&
            error.ErrorMessage == "Product id cannot be empty.");
    }

    /// <summary>
    /// Tests that an empty product name fails validation.
    /// </summary>
    [Fact(DisplayName = "Given empty product name When validating Then should have validation error")]
    public void Validate_EmptyName_ShouldHaveValidationError()
    {
        // Given
        var command = new UpdateProductCommand
        {
            Id = Guid.NewGuid(),
            Name = string.Empty,
            CurrentPrice = 10,
            Status = ProductStatus.Active
        };

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();

        result.Errors.Should().Contain(error =>
            error.PropertyName == nameof(UpdateProductCommand.Name) &&
            error.ErrorMessage == "The product name cannot be empty.");
    }

    /// <summary>
    /// Tests that a product name shorter than minimum fails validation.
    /// </summary>
    [Fact(DisplayName = "Given product name shorter than minimum When validating Then should have validation error")]
    public void Validate_NameShorterThanMinimum_ShouldHaveValidationError()
    {
        // Given
        var command = new UpdateProductCommand
        {
            Id = Guid.NewGuid(),
            Name = "AB",
            CurrentPrice = 10,
            Status = ProductStatus.Active
        };

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();

        result.Errors.Should().Contain(error =>
            error.PropertyName == nameof(UpdateProductCommand.Name) &&
            error.ErrorMessage == "The product name must be at least 3 characters long.");
    }

    /// <summary>
    /// Tests that a product name longer than maximum fails validation.
    /// </summary>
    [Fact(DisplayName = "Given product name longer than maximum When validating Then should have validation error")]
    public void Validate_NameLongerThanMaximum_ShouldHaveValidationError()
    {
        // Given
        var command = new UpdateProductCommand
        {
            Id = Guid.NewGuid(),
            Name = new string('A', 51),
            CurrentPrice = 10,
            Status = ProductStatus.Active
        };

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();

        result.Errors.Should().Contain(error =>
            error.PropertyName == nameof(UpdateProductCommand.Name) &&
            error.ErrorMessage == "The product name must be at most 50 characters long.");
    }

    /// <summary>
    /// Tests that a product name with minimum length passes validation.
    /// </summary>
    [Fact(DisplayName = "Given product name with minimum length When validating Then should not have name error")]
    public void Validate_NameWithMinimumLength_ShouldNotHaveNameValidationError()
    {
        // Given
        var command = new UpdateProductCommand
        {
            Id = Guid.NewGuid(),
            Name = "ABC",
            CurrentPrice = 10,
            Status = ProductStatus.Active
        };

        // When
        var result = _validator.Validate(command);

        // Then
        result.Errors.Should().NotContain(error =>
            error.PropertyName == nameof(UpdateProductCommand.Name));
    }

    /// <summary>
    /// Tests that a product name with maximum length passes validation.
    /// </summary>
    [Fact(DisplayName = "Given product name with maximum length When validating Then should not have name error")]
    public void Validate_NameWithMaximumLength_ShouldNotHaveNameValidationError()
    {
        // Given
        var command = new UpdateProductCommand
        {
            Id = Guid.NewGuid(),
            Name = new string('A', 50),
            CurrentPrice = 10,
            Status = ProductStatus.Active
        };

        // When
        var result = _validator.Validate(command);

        // Then
        result.Errors.Should().NotContain(error =>
            error.PropertyName == nameof(UpdateProductCommand.Name));
    }

    /// <summary>
    /// Tests that zero product price fails validation.
    /// </summary>
    [Fact(DisplayName = "Given zero product price When validating Then should have validation error")]
    public void Validate_ZeroCurrentPrice_ShouldHaveValidationError()
    {
        // Given
        var command = new UpdateProductCommand
        {
            Id = Guid.NewGuid(),
            Name = "Product Test",
            CurrentPrice = 0,
            Status = ProductStatus.Active
        };

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();

        result.Errors.Should().Contain(error =>
            error.PropertyName == nameof(UpdateProductCommand.CurrentPrice) &&
            error.ErrorMessage == "Product price must be greater than zero.");
    }

    /// <summary>
    /// Tests that negative product price fails validation.
    /// </summary>
    [Fact(DisplayName = "Given negative product price When validating Then should have validation error")]
    public void Validate_NegativeCurrentPrice_ShouldHaveValidationError()
    {
        // Given
        var command = new UpdateProductCommand
        {
            Id = Guid.NewGuid(),
            Name = "Product Test",
            CurrentPrice = -1,
            Status = ProductStatus.Active
        };

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();

        result.Errors.Should().Contain(error =>
            error.PropertyName == nameof(UpdateProductCommand.CurrentPrice) &&
            error.ErrorMessage == "Product price must be greater than zero.");
    }

    /// <summary>
    /// Tests that a product price greater than zero passes validation.
    /// </summary>
    [Fact(DisplayName = "Given positive product price When validating Then should not have price error")]
    public void Validate_PositiveCurrentPrice_ShouldNotHavePriceValidationError()
    {
        // Given
        var command = new UpdateProductCommand
        {
            Id = Guid.NewGuid(),
            Name = "Product Test",
            CurrentPrice = 1,
            Status = ProductStatus.Active
        };

        // When
        var result = _validator.Validate(command);

        // Then
        result.Errors.Should().NotContain(error =>
            error.PropertyName == nameof(UpdateProductCommand.CurrentPrice));
    }

    /// <summary>
    /// Tests that an unknown product status fails validation.
    /// </summary>
    [Fact(DisplayName = "Given unknown product status When validating Then should have validation error")]
    public void Validate_UnknownStatus_ShouldHaveValidationError()
    {
        // Given
        var command = new UpdateProductCommand
        {
            Id = Guid.NewGuid(),
            Name = "Product Test",
            CurrentPrice = 10,
            Status = ProductStatus.Unknown
        };

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();

        result.Errors.Should().Contain(error =>
            error.PropertyName == nameof(UpdateProductCommand.Status) &&
            error.ErrorMessage == "Product status cannot be unknown.");
    }

    /// <summary>
    /// Tests that a valid product status passes validation.
    /// </summary>
    [Fact(DisplayName = "Given valid product status When validating Then should not have status error")]
    public void Validate_ValidStatus_ShouldNotHaveStatusValidationError()
    {
        // Given
        var command = new UpdateProductCommand
        {
            Id = Guid.NewGuid(),
            Name = "Product Test",
            CurrentPrice = 10,
            Status = ProductStatus.Active
        };

        // When
        var result = _validator.Validate(command);

        // Then
        result.Errors.Should().NotContain(error =>
            error.PropertyName == nameof(UpdateProductCommand.Status));
    }
}