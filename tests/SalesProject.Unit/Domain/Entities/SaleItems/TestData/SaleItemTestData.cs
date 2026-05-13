using Bogus;
using SalesProject.Domain.Entities;
using SalesProject.Domain.Enums;

namespace SalesProject.Unit.Domain.Entities.SaleItems.TestData;

/// <summary>
/// Provides methods for generating test data using the Bogus library.
/// </summary>
public static class SaleItemTestData
{
    /// <summary>
    /// Configures the Faker to generate valid SaleItem entities.
    /// </summary>
    private static readonly Faker<SaleItem> SaleItemFaker = new Faker<SaleItem>()
        .RuleFor(i => i.SaleId, f => f.Random.Guid())
        .RuleFor(i => i.ProductId, f => f.Random.Guid())
        .RuleFor(i => i.Quantity, f => f.Random.Int(1, 3))
        .RuleFor(i => i.UnitPrice, f => f.Random.Decimal(1, 100))
        .RuleFor(i => i.Status, SaleItemStatus.Active);

    /// <summary>
    /// Generates a valid SaleItem entity.
    /// </summary>
    public static SaleItem GenerateValidSaleItem()
    {
        return SaleItemFaker.Generate();
    }
}