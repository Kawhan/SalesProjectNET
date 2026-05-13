using SalesProject.Domain.Entities;
using SalesProject.Domain.Enums;
using SalesProject.Unit.Domain.Entities.SaleItems.TestData;

namespace SalesProject.Unit.Domain.Entities.SaleItems;

/// <summary>
/// Contains unit tests for the SaleItem entity class.
/// Tests cover update, cancel and recalculation scenarios.
/// </summary>
public class SaleItemTests
{
    /// <summary>
    /// Tests that a new sale item is initialized with default values.
    /// </summary>
    [Fact(DisplayName = "Sale item should be initialized with default values when created")]
    public void Given_NewSaleItem_When_Created_Then_ShouldHaveDefaultValues()
    {
        // Arrange & Act
        var item = new SaleItem();

        // Assert
        Assert.Equal(SaleItemStatus.Active, item.Status);
        Assert.NotEqual(default, item.CreatedAt);
    }

    /// <summary>
    /// Tests that when a sale item is updated with less than 4 items, no discount is applied.
    /// </summary>
    [Fact(DisplayName = "Sale item should have no discount when quantity is lower than four")]
    public void Given_QuantityLowerThanFour_When_Updated_Then_ShouldApplyNoDiscount()
    {
        // Arrange
        var item = SaleItemTestData.GenerateValidSaleItem();

        // Act
        item.Update(quantity: 3, unitPrice: 100);

        // Assert
        Assert.Equal(3, item.Quantity);
        Assert.Equal(100, item.UnitPrice);
        Assert.Equal(0, item.DiscountPercentage);
        Assert.Equal(0, item.Discount);
        Assert.Equal(300, item.TotalAmount);
        Assert.Equal(SaleItemStatus.Active, item.Status);
        Assert.NotNull(item.UpdatedAt);
    }

    /// <summary>
    /// Tests that when a sale item is updated with quantity between 4 and 9, 10% discount is applied.
    /// </summary>
    [Fact(DisplayName = "Sale item should apply ten percent discount when quantity is between four and nine")]
    public void Given_QuantityBetweenFourAndNine_When_Updated_Then_ShouldApplyTenPercentDiscount()
    {
        // Arrange
        var item = SaleItemTestData.GenerateValidSaleItem();

        // Act
        item.Update(quantity: 4, unitPrice: 100);

        // Assert
        Assert.Equal(4, item.Quantity);
        Assert.Equal(100, item.UnitPrice);
        Assert.Equal(10, item.DiscountPercentage);
        Assert.Equal(40, item.Discount);
        Assert.Equal(360, item.TotalAmount);
        Assert.Equal(SaleItemStatus.Active, item.Status);
        Assert.NotNull(item.UpdatedAt);
    }

    /// <summary>
    /// Tests that when a sale item is updated with quantity between 10 and 20, 20% discount is applied.
    /// </summary>
    [Fact(DisplayName = "Sale item should apply twenty percent discount when quantity is between ten and twenty")]
    public void Given_QuantityBetweenTenAndTwenty_When_Updated_Then_ShouldApplyTwentyPercentDiscount()
    {
        // Arrange
        var item = SaleItemTestData.GenerateValidSaleItem();

        // Act
        item.Update(quantity: 10, unitPrice: 100);

        // Assert
        Assert.Equal(10, item.Quantity);
        Assert.Equal(100, item.UnitPrice);
        Assert.Equal(20, item.DiscountPercentage);
        Assert.Equal(200, item.Discount);
        Assert.Equal(800, item.TotalAmount);
        Assert.Equal(SaleItemStatus.Active, item.Status);
        Assert.NotNull(item.UpdatedAt);
    }

    /// <summary>
    /// Tests that when a sale item is cancelled, its status changes to Cancelled.
    /// </summary>
    [Fact(DisplayName = "Sale item status should change to Cancelled when cancelled")]
    public void Given_ActiveSaleItem_When_Cancelled_Then_StatusShouldBeCancelled()
    {
        // Arrange
        var item = SaleItemTestData.GenerateValidSaleItem();
        item.Status = SaleItemStatus.Active;

        // Act
        item.Cancel();

        // Assert
        Assert.Equal(SaleItemStatus.Cancelled, item.Status);
        Assert.NotNull(item.UpdatedAt);
    }

    /// <summary>
    /// Tests that Recalculate updates discount and total amount based on quantity and unit price.
    /// </summary>
    [Fact(DisplayName = "Sale item should recalculate discount and total amount")]
    public void Given_SaleItem_When_Recalculated_Then_ShouldUpdateDiscountAndTotalAmount()
    {
        // Arrange
        var item = SaleItemTestData.GenerateValidSaleItem();
        item.Quantity = 5;
        item.UnitPrice = 200;

        // Act
        item.Recalculate();

        // Assert
        Assert.Equal(10, item.DiscountPercentage);
        Assert.Equal(100, item.Discount);
        Assert.Equal(900, item.TotalAmount);
    }
}