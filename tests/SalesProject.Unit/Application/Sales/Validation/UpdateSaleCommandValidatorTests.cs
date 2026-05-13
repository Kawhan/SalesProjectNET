using SalesProject.Application.Sales.UpdateSale;
using FluentAssertions;

namespace SalesProject.Unit.Application.Sales.Validation;

/// <summary>
/// Contains unit tests for the <see cref="UpdateSaleCommandValidator"/> class.
/// </summary>
public class UpdateSaleCommandValidatorTests
{
    private readonly UpdateSaleCommandValidator _validator;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateSaleCommandValidatorTests"/> class.
    /// </summary>
    public UpdateSaleCommandValidatorTests()
    {
        _validator = new UpdateSaleCommandValidator();
    }

    /// <summary>
    /// Tests that a valid update sale command passes validation.
    /// </summary>
    [Fact(DisplayName = "Given valid sale command When validating Then should not have errors")]
    public void Validate_ValidCommand_ShouldNotHaveValidationErrors()
    {
        // Given
        var command = new UpdateSaleCommand
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            Items = new List<UpdateSaleItemCommand>
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
    /// Tests that an empty sale id fails validation.
    /// </summary>
    [Fact(DisplayName = "Given empty sale id When validating Then should have validation error")]
    public void Validate_EmptySaleId_ShouldHaveValidationError()
    {
        // Given
        var command = new UpdateSaleCommand
        {
            Id = Guid.Empty,
            UserId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            Items = new List<UpdateSaleItemCommand>
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
            error.PropertyName == nameof(UpdateSaleCommand.Id) &&
            error.ErrorMessage == "Sale id cannot be empty.");
    }

    /// <summary>
    /// Tests that an empty user id fails validation.
    /// </summary>
    [Fact(DisplayName = "Given empty user id When validating Then should have validation error")]
    public void Validate_EmptyUserId_ShouldHaveValidationError()
    {
        // Given
        var command = new UpdateSaleCommand
        {
            Id = Guid.NewGuid(),
            UserId = Guid.Empty,
            BranchId = Guid.NewGuid(),
            Items = new List<UpdateSaleItemCommand>
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
            error.PropertyName == nameof(UpdateSaleCommand.UserId) &&
            error.ErrorMessage == "User id cannot be empty.");
    }

    /// <summary>
    /// Tests that an empty branch id fails validation.
    /// </summary>
    [Fact(DisplayName = "Given empty branch id When validating Then should have validation error")]
    public void Validate_EmptyBranchId_ShouldHaveValidationError()
    {
        // Given
        var command = new UpdateSaleCommand
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            BranchId = Guid.Empty,
            Items = new List<UpdateSaleItemCommand>
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
            error.PropertyName == nameof(UpdateSaleCommand.BranchId) &&
            error.ErrorMessage == "Branch id cannot be empty.");
    }

    /// <summary>
    /// Tests that empty items fails validation.
    /// </summary>
    [Fact(DisplayName = "Given empty items When validating Then should have validation error")]
    public void Validate_EmptyItems_ShouldHaveValidationError()
    {
        // Given
        var command = new UpdateSaleCommand
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            Items = new List<UpdateSaleItemCommand>()
        };

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();

        result.Errors.Should().Contain(error =>
            error.PropertyName == nameof(UpdateSaleCommand.Items) &&
            error.ErrorMessage == "Sale must contain at least one item.");
    }

    /// <summary>
    /// Tests that an item with empty product id fails validation.
    /// </summary>
    [Fact(DisplayName = "Given item with empty product id When validating Then should have validation error")]
    public void Validate_ItemWithEmptyProductId_ShouldHaveValidationError()
    {
        // Given
        var command = new UpdateSaleCommand
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            Items = new List<UpdateSaleItemCommand>
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
    /// Tests that an item with zero quantity fails validation.
    /// </summary>
    [Fact(DisplayName = "Given item with zero quantity When validating Then should have validation error")]
    public void Validate_ItemWithZeroQuantity_ShouldHaveValidationError()
    {
        // Given
        var command = new UpdateSaleCommand
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            Items = new List<UpdateSaleItemCommand>
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
    /// Tests that an item with negative quantity fails validation.
    /// </summary>
    [Fact(DisplayName = "Given item with negative quantity When validating Then should have validation error")]
    public void Validate_ItemWithNegativeQuantity_ShouldHaveValidationError()
    {
        // Given
        var command = new UpdateSaleCommand
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            Items = new List<UpdateSaleItemCommand>
            {
                new()
                {
                    ProductId = Guid.NewGuid(),
                    Quantity = -1
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

        var command = new UpdateSaleCommand
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            Items = new List<UpdateSaleItemCommand>
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
            error.PropertyName == nameof(UpdateSaleCommand.Items) &&
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

        var command = new UpdateSaleCommand
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            Items = new List<UpdateSaleItemCommand>
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
            error.PropertyName == nameof(UpdateSaleCommand.Items) &&
            error.ErrorMessage == "It is not possible to sell more than 20 identical items.");
    }
}