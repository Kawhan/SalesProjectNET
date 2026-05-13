using Bogus;
using SalesProject.Domain.Entities;
using SalesProject.Domain.Enums;

namespace SalesProject.Unit.Domain.Entities.Sales.TestData;

/// <summary>
/// Provides methods for generating test data using the Bogus library.
/// </summary>
public static class SaleTestData
{
    /// <summary>
    /// Configures the Faker to generate valid Sale entities.
    /// </summary>
    private static readonly Faker<Sale> SaleFaker = new Faker<Sale>()
        .RuleFor(s => s.UserId, f => f.Random.Guid())
        .RuleFor(s => s.BranchId, f => f.Random.Guid())
        .RuleFor(s => s.Status, SaleStatus.Active);

    /// <summary>
    /// Generates a valid Sale entity with randomized data.
    /// </summary>
    /// <returns>A valid Sale entity with randomly generated data.</returns>
    public static Sale GenerateValidSale()
    {
        return SaleFaker.Generate();
    }

    /// <summary>
    /// Generates an active SaleItem with the informed total amount.
    /// </summary>
    public static SaleItem GenerateActiveItem(decimal totalAmount)
    {
        return new SaleItem
        {
            Id = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            Quantity = 1,
            UnitPrice = totalAmount,
            DiscountPercentage = 0,
            Discount = 0,
            TotalAmount = totalAmount,
            Status = SaleItemStatus.Active
        };
    }

    /// <summary>
    /// Generates a cancelled SaleItem with the informed total amount.
    /// </summary>
    public static SaleItem GenerateCancelledItem(decimal totalAmount)
    {
        return new SaleItem
        {
            Id = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            Quantity = 1,
            UnitPrice = totalAmount,
            DiscountPercentage = 0,
            Discount = 0,
            TotalAmount = totalAmount,
            Status = SaleItemStatus.Cancelled
        };
    }
}