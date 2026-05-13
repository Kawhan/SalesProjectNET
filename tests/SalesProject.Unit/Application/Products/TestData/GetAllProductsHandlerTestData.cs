using Bogus;
using SalesProject.Application.Products.GetAllProducts;
using SalesProject.Domain.Entities;
using SalesProject.Domain.Enums;

namespace SalesProject.Unit.Application.Products.TestData;

/// <summary>
/// Provides methods for generating test data using the Bogus library.
/// This class centralizes all test data generation to ensure consistency
/// across test cases.
/// </summary>
public static class GetAllProductsHandlerTestData
{
    /// <summary>
    /// Configures the Faker to generate valid GetAllProductsCommand instances.
    /// </summary>
    private static readonly Faker<GetAllProductsCommand> getAllProductsHandlerFaker = new Faker<GetAllProductsCommand>()
        .RuleFor(p => p.PageNumber, f => f.Random.Int(1, 5))
        .RuleFor(p => p.PageSize, f => f.Random.Int(5, 20))
        .RuleFor(p => p.Name, f => f.Commerce.ProductName())
        .RuleFor(p => p.MinPrice, f => f.Random.Decimal(1, 100))
        .RuleFor(p => p.MaxPrice, f => f.Random.Decimal(101, 1000))
        .RuleFor(p => p.Status, f => f.PickRandom(ProductStatus.Active, ProductStatus.Inactive));

    /// <summary>
    /// Configures the Faker to generate valid Product entities.
    /// </summary>
    private static readonly Faker<Product> productFaker = new Faker<Product>()
        .RuleFor(p => p.Id, f => f.Random.Guid())
        .RuleFor(p => p.Name, f => f.Commerce.ProductName())
        .RuleFor(p => p.CurrentPrice, f => f.Random.Decimal(1, 1000))
        .RuleFor(p => p.Status, f => f.PickRandom(ProductStatus.Active, ProductStatus.Inactive))
        .RuleFor(p => p.CreatedAt, f => f.Date.Past())
        .RuleFor(p => p.UpdatedAt, f => f.Date.Recent());

    /// <summary>
    /// Generates a valid GetAllProductsCommand with randomized data.
    /// </summary>
    /// <returns>A valid GetAllProductsCommand with randomly generated data.</returns>
    public static GetAllProductsCommand GenerateValidCommand()
    {
        return getAllProductsHandlerFaker.Generate();
    }

    /// <summary>
    /// Generates a list of valid Product entities.
    /// </summary>
    /// <param name="count">The number of products to generate.</param>
    /// <returns>A list of valid Product entities.</returns>
    public static List<Product> GenerateProducts(int count)
    {
        return productFaker.Generate(count);
    }

    /// <summary>
    /// Generates a list of GetAllProductsResult objects based on product entities.
    /// </summary>
    /// <param name="products">The product entities.</param>
    /// <returns>A list of GetAllProductsResult objects.</returns>
    public static List<GetAllProductsResult> GenerateProductResults(List<Product> products)
    {
        return products.Select(product => new GetAllProductsResult
        {
            Id = product.Id,
            Name = product.Name,
            CurrentPrice = product.CurrentPrice,
            Status = product.Status,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        }).ToList();
    }
}