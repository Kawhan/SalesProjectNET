using Bogus;
using SalesProject.Application.Products.CreateProduct;
using SalesProject.Domain.Entities;
using SalesProject.Domain.Enums;

namespace SalesProject.Unit.Application.Products.TestData;

/// <summary>
/// Provides methods for generating test data using the Bogus library.
/// This class centralizes all test data generation to ensure consistency
/// across test cases.
/// </summary>
public static class CreateProductHandlerTestData
{
    /// <summary>
    /// Configures the Faker to generate valid CreateProductCommand instances.
    /// </summary>
    private static readonly Faker<CreateProductCommand> createProductHandlerFaker = new Faker<CreateProductCommand>()
        .RuleFor(p => p.Name, f => f.Commerce.ProductName())
        .RuleFor(p => p.CurrentPrice, f => f.Random.Decimal(1, 1000))
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
    /// Generates a valid CreateProductCommand with randomized data.
    /// </summary>
    /// <returns>A valid CreateProductCommand with randomly generated data.</returns>
    public static CreateProductCommand GenerateValidCommand()
    {
        return createProductHandlerFaker.Generate();
    }

    /// <summary>
    /// Generates a valid Product entity.
    /// </summary>
    /// <returns>A valid Product entity with randomly generated data.</returns>
    public static Product GenerateProduct()
    {
        return productFaker.Generate();
    }

    /// <summary>
    /// Generates a CreateProductResult object based on a Product entity.
    /// </summary>
    /// <param name="product">The product entity.</param>
    /// <returns>A CreateProductResult object.</returns>
    public static CreateProductResult GenerateProductResult(Product product)
    {
        return new CreateProductResult
        {
            Id = product.Id,
            Name = product.Name,
            CurrentPrice = product.CurrentPrice,
            Status = product.Status
        };
    }
}