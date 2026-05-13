using Bogus;
using SalesProject.Domain.Entities;
using SalesProject.Domain.Enums;

namespace SalesProject.Unit.Domain.Entities.Products.TestData;

/// <summary>
/// Provides methods for generating test data using the Bogus library.
/// </summary>
public static class ProductTestData
{
    /// <summary>
    /// Configures the Faker to generate valid Product entities.
    /// </summary>
    private static readonly Faker<Product> ProductFaker = new Faker<Product>()
        .RuleFor(p => p.Name, f => f.Commerce.ProductName())
        .RuleFor(p => p.CurrentPrice, f => f.Random.Decimal(1, 1000))
        .RuleFor(p => p.Status, f => f.PickRandom(ProductStatus.Active, ProductStatus.Inactive));

    /// <summary>
    /// Generates a valid Product entity with randomized data.
    /// </summary>
    /// <returns>A valid Product entity with randomly generated data.</returns>
    public static Product GenerateValidProduct()
    {
        return ProductFaker.Generate();
    }
}