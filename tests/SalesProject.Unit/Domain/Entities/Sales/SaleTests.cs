using SalesProject.Domain.Entities;
using SalesProject.Domain.Enums;
using SalesProject.Unit.Domain.Entities.Sales.TestData;

namespace SalesProject.Unit.Domain.Entities.Sales;


/// <summary>
/// Contains unit tests for the Sale entity class.
/// Tests cover status changes, sale number generation and total recalculation.
/// </summary>
public class SaleTests
{
    /// <summary>
    /// Tests that a new sale is initialized with default values.
    /// </summary>
    [Fact(DisplayName = "Sale should be initialized with default values when created")]
    public void Given_NewSale_When_Created_Then_ShouldHaveDefaultValues()
    {
        // Arrange & Act
        var sale = new Sale();

        // Assert
        Assert.NotEqual(default, sale.CreatedAt);
        Assert.NotEqual(default, sale.SaleDate);
        Assert.Equal(SaleStatus.Active, sale.Status);
        Assert.False(string.IsNullOrWhiteSpace(sale.SaleNumber));
        Assert.StartsWith("SALE-", sale.SaleNumber);
        Assert.NotNull(sale.Items);
        Assert.Empty(sale.Items);
    }

    /// <summary>
    /// Tests that GenerateSaleNumber creates a sale number using the expected prefix.
    /// </summary>
    [Fact(DisplayName = "Sale number should be generated with SALE prefix")]
    public void Given_Sale_When_GenerateSaleNumber_Then_SaleNumberShouldHaveSalePrefix()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();

        // Act
        sale.GenerateSaleNumber();

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(sale.SaleNumber));
        Assert.StartsWith("SALE-", sale.SaleNumber);
        Assert.NotNull(sale.UpdatedAt);
    }

    /// <summary>
    /// Tests that when a cancelled sale is activated, its status changes to Active.
    /// </summary>
    [Fact(DisplayName = "Sale status should change to Active when activated")]
    public void Given_CancelledSale_When_Activated_Then_StatusShouldBeActive()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        sale.Status = SaleStatus.Cancelled;

        // Act
        sale.Activate();

        // Assert
        Assert.Equal(SaleStatus.Active, sale.Status);
        Assert.NotNull(sale.UpdatedAt);
    }

    /// <summary>
    /// Tests that when an active sale is deactivated, its status changes to Cancelled.
    /// </summary>
    [Fact(DisplayName = "Sale status should change to Cancelled when deactivated")]
    public void Given_ActiveSale_When_Deactivated_Then_StatusShouldBeCancelled()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        sale.Status = SaleStatus.Active;

        // Act
        sale.Deactivate();

        // Assert
        Assert.Equal(SaleStatus.Cancelled, sale.Status);
        Assert.NotNull(sale.UpdatedAt);
    }

    /// <summary>
    /// Tests that sale header information is updated correctly.
    /// </summary>
    [Fact(DisplayName = "Sale user and branch should be updated when update is called")]
    public void Given_Sale_When_Updated_Then_UserAndBranchShouldBeChanged()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        var newUserId = Guid.NewGuid();
        var newBranchId = Guid.NewGuid();

        // Act
        sale.Update(newUserId, newBranchId);

        // Assert
        Assert.Equal(newUserId, sale.UserId);
        Assert.Equal(newBranchId, sale.BranchId);
        Assert.NotNull(sale.UpdatedAt);
    }

    /// <summary>
    /// Tests that sale total is recalculated using only active items.
    /// </summary>
    [Fact(DisplayName = "Sale total should be recalculated using only active items")]
    public void Given_SaleWithActiveAndCancelledItems_When_RecalculateTotal_Then_TotalShouldUseOnlyActiveItems()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();

        sale.Items = new List<SaleItem>
        {
            SaleTestData.GenerateActiveItem(100),
            SaleTestData.GenerateActiveItem(200),
            SaleTestData.GenerateCancelledItem(500)
        };

        // Act
        sale.RecalculateTotal();

        // Assert
        Assert.Equal(300, sale.TotalAmount);
        Assert.NotNull(sale.UpdatedAt);
    }

    /// <summary>
    /// Tests that sale total becomes zero when there are no active items.
    /// </summary>
    [Fact(DisplayName = "Sale total should be zero when there are no active items")]
    public void Given_SaleWithoutActiveItems_When_RecalculateTotal_Then_TotalShouldBeZero()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();

        sale.Items = new List<SaleItem>
        {
            SaleTestData.GenerateCancelledItem(100),
            SaleTestData.GenerateCancelledItem(200)
        };

        // Act
        sale.RecalculateTotal();

        // Assert
        Assert.Equal(0, sale.TotalAmount);
        Assert.NotNull(sale.UpdatedAt);
    }
}
