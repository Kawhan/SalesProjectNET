using SalesProject.Application.Sales.CreateSale;
using FluentAssertions;

namespace SalesProject.Unit.Application.Sales.Validation;

/// <summary>
/// Contains unit tests for the <see cref="CreateSaleCommandValidator"/> class.
/// </summary>
public class CreateSaleCommandValidatorTests
{
    private readonly CreateSaleCommandValidator _validator;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateSaleCommandValidatorTests"/> class.
    /// </summary>
    public CreateSaleCommandValidatorTests()
    {
        _validator = new CreateSaleCommandValidator();
    }

    /// <summary>
    /// Tests that a valid create sale command passes validation.
    /// </summary>
    [Fact(DisplayName = "Given valid sale command When validating Then should not have errors")]
    public void Validate_ValidCommand_ShouldNotHaveValidationErrors()
    {
        // Given
        var command = new CreateSaleCommand
        {
            UserId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            Items = new List<CreateSaleItemCommand>
            {
                new()
                {
                    ProductId = Guid.NewGuid(),
                    Quantity = 1
                }
            }
        };

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    /// <summary>
    /// Tests that an empty user id fails validation.
    /// </summary>
    [Fact(DisplayName = "Given empty user id When validating Then should have validation error")]
    public void Validate_EmptyUserId_ShouldHaveValidationError()
    {
        // Given
        var command = new CreateSaleCommand
        {
            UserId = Guid.Empty,
            BranchId = Guid.NewGuid(),
            Items = new List<CreateSaleItemCommand>
            {
                new()
                {
                    ProductId = Guid.NewGuid(),
                    Quantity = 1
                }
            }
        };

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();

        result.Errors.Should().Contain(error =>
            error.PropertyName == nameof(CreateSaleCommand.UserId) &&
            error.ErrorMessage == "User id cannot be empty.");
    }

    /// <summary>
    /// Tests that an empty branch id fails validation.
    /// </summary>
    [Fact(DisplayName = "Given empty branch id When validating Then should have validation error")]
    public void Validate_EmptyBranchId_ShouldHaveValidationError()
    {
        // Given
        var command = new CreateSaleCommand
        {
            UserId = Guid.NewGuid(),
            BranchId = Guid.Empty,
            Items = new List<CreateSaleItemCommand>
            {
                new()
                {
                    ProductId = Guid.NewGuid(),
                    Quantity = 1
                }
            }
        };

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();

        result.Errors.Should().Contain(error =>
            error.PropertyName == nameof(CreateSaleCommand.BranchId) &&
            error.ErrorMessage == "Branch id cannot be empty.");
    }

    /// <summary>
    /// Tests that an empty item list fails validation.
    /// </summary>
    [Fact(DisplayName = "Given empty items When validating Then should have validation error")]
    public void Validate_EmptyItems_ShouldHaveValidationError()
    {
        // Given
        var command = new CreateSaleCommand
        {
            UserId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            Items = new List<CreateSaleItemCommand>()
        };

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();

        result.Errors.Should().Contain(error =>
            error.PropertyName == nameof(CreateSaleCommand.Items) &&
            error.ErrorMessage == "Sale must contain at least one item.");
    }

    /// <summary>
    /// Tests that a sale item with empty product id fails validation.
    /// </summary>
    [Fact(DisplayName = "Given item with empty product id When validating Then should have validation error")]
    public void Validate_ItemWithEmptyProductId_ShouldHaveValidationError()
    {
        // Given
        var command = new CreateSaleCommand
        {
            UserId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            Items = new List<CreateSaleItemCommand>
            {
                new()
                {
                    ProductId = Guid.Empty,
                    Quantity = 1
                }
            }
        };

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();

        result.Errors.Should().Contain(error =>
            error.PropertyName == "Items[0].ProductId" &&
            error.ErrorMessage == "Product id cannot be empty.");
    }

    /// <summary>
    /// Tests that a sale item with quantity zero fails validation.
    /// </summary>
    [Fact(DisplayName = "Given item with zero quantity When validating Then should have validation error")]
    public void Validate_ItemWithZeroQuantity_ShouldHaveValidationError()
    {
        // Given
        var command = new CreateSaleCommand
        {
            UserId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            Items = new List<CreateSaleItemCommand>
            {
                new()
                {
                    ProductId = Guid.NewGuid(),
                    Quantity = 0
                }
            }
        };

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();

        result.Errors.Should().Contain(error =>
            error.PropertyName == "Items[0].Quantity" &&
            error.ErrorMessage == "Product quantity must be greater than zero.");
    }

    /// <summary>
    /// Tests that more than 20 identical items fails validation.
    /// </summary>
    [Fact(DisplayName = "Given more than twenty identical items When validating Then should have validation error")]
    public void Validate_MoreThanTwentyIdenticalItems_ShouldHaveValidationError()
    {
        // Given
        var productId = Guid.NewGuid();

        var command = new CreateSaleCommand
        {
            UserId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            Items = new List<CreateSaleItemCommand>
            {
                new()
                {
                    ProductId = productId,
                    Quantity = 10
                },
                new()
                {
                    ProductId = productId,
                    Quantity = 11
                }
            }
        };

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();

        result.Errors.Should().Contain(error =>
            error.PropertyName == nameof(CreateSaleCommand.Items) &&
            error.ErrorMessage == "It is not possible to sell more than 20 identical items.");
    }

    /// <summary>
    /// Tests that exactly 20 identical items passes validation.
    /// </summary>
    [Fact(DisplayName = "Given exactly twenty identical items When validating Then should not have item limit error")]
    public void Validate_ExactlyTwentyIdenticalItems_ShouldNotHaveItemLimitValidationError()
    {
        // Given
        var productId = Guid.NewGuid();

        var command = new CreateSaleCommand
        {
            UserId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            Items = new List<CreateSaleItemCommand>
            {
                new()
                {
                    ProductId = productId,
                    Quantity = 10
                },
                new()
                {
                    ProductId = productId,
                    Quantity = 10
                }
            }
        };

        // When
        var result = _validator.Validate(command);

        // Then
        result.Errors.Should().NotContain(error =>
            error.PropertyName == nameof(CreateSaleCommand.Items) &&
            error.ErrorMessage == "It is not possible to sell more than 20 identical items.");
    }
}