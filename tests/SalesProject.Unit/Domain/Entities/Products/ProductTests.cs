using SalesProject.Domain.Entities;
using SalesProject.Domain.Enums;
using SalesProject.Unit.Domain.Entities.Products.TestData;

namespace SalesProject.Unit.Domain.Entities.Products;

/// <summary>
/// Contains unit tests for the Product entity class.
/// Tests cover status changes and timestamp update scenarios.
/// </summary>
public class ProductTests
{
    /// <summary>
    /// Tests that a new product has CreatedAt set when instantiated.
    /// </summary>
    [Fact(DisplayName = "Product should have CreatedAt set when created")]
    public void Given_NewProduct_When_Created_Then_CreatedAtShouldBeSet()
    {
        // Arrange & Act
        var product = new Product();

        // Assert
        Assert.NotEqual(default, product.CreatedAt);
    }

    /// <summary>
    /// Tests that when an inactive product is activated, its status changes to Active.
    /// </summary>
    [Fact(DisplayName = "Product status should change to Active when activated")]
    public void Given_InactiveProduct_When_Activated_Then_StatusShouldBeActive()
    {
        // Arrange
        var product = ProductTestData.GenerateValidProduct();
        product.Status = ProductStatus.Inactive;

        // Act
        product.Activate();

        // Assert
        Assert.Equal(ProductStatus.Active, product.Status);
        Assert.NotNull(product.UpdatedAt);
    }

    /// <summary>
    /// Tests that when an active product is deactivated, its status changes to Inactive.
    /// </summary>
    [Fact(DisplayName = "Product status should change to Inactive when deactivated")]
    public void Given_ActiveProduct_When_Deactivated_Then_StatusShouldBeInactive()
    {
        // Arrange
        var product = ProductTestData.GenerateValidProduct();
        product.Status = ProductStatus.Active;

        // Act
        product.Deactivate();

        // Assert
        Assert.Equal(ProductStatus.Inactive, product.Status);
        Assert.NotNull(product.UpdatedAt);
    }

    /// <summary>
    /// Tests that UpdatedAt is set when UpdateTimestamp is called.
    /// </summary>
    [Fact(DisplayName = "Product UpdatedAt should be set when update timestamp is called")]
    public void Given_Product_When_UpdateTimestampCalled_Then_UpdatedAtShouldBeSet()
    {
        // Arrange
        var product = ProductTestData.GenerateValidProduct();
        product.UpdatedAt = null;

        // Act
        product.UpdateTimestamp();

        // Assert
        Assert.NotNull(product.UpdatedAt);
    }

    /// <summary>
    /// Tests that product sale items collection starts initialized.
    /// </summary>
    [Fact(DisplayName = "Product sale items should be initialized when product is created")]
    public void Given_NewProduct_When_Created_Then_SaleItemsShouldBeInitialized()
    {
        // Arrange & Act
        var product = new Product();

        // Assert
        Assert.NotNull(product.SaleItems);
        Assert.Empty(product.SaleItems);
    }
}