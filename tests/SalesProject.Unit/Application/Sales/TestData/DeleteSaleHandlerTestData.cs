using Bogus;
using SalesProject.Application.Sales.DeleteSale;
using SalesProject.Domain.Entities;
using SalesProject.Domain.Enums;

namespace SalesProject.Unit.Application.Sales.TestData;

/// <summary>
/// Provides methods for generating test data using the Bogus library.
/// This class centralizes all test data generation to ensure consistency
/// across test cases.
/// </summary>
public static class DeleteSaleHandlerTestData
{
    /// <summary>
    /// Configures the Faker to generate valid DeleteSaleCommand instances.
    /// </summary>
    private static readonly Faker<DeleteSaleCommand> deleteSaleHandlerFaker = new Faker<DeleteSaleCommand>()
        .CustomInstantiator(f => new DeleteSaleCommand(f.Random.Guid()));

    /// <summary>
    /// Configures the Faker to generate valid Sale entities.
    /// </summary>
    private static readonly Faker<Sale> saleFaker = new Faker<Sale>()
        .RuleFor(s => s.Id, f => f.Random.Guid())
        .RuleFor(s => s.SaleNumber, f => $"SALE-{f.Date.Recent():yyyyMMddHHmmss}")
        .RuleFor(s => s.SaleDate, f => f.Date.Recent())
        .RuleFor(s => s.UserId, f => f.Random.Guid())
        .RuleFor(s => s.BranchId, f => f.Random.Guid())
        .RuleFor(s => s.TotalAmount, f => f.Random.Decimal(1, 1000))
        .RuleFor(s => s.Status, SaleStatus.Cancelled)
        .RuleFor(s => s.CreatedAt, f => f.Date.Past())
        .RuleFor(s => s.UpdatedAt, f => f.Date.Recent());

    /// <summary>
    /// Generates a valid DeleteSaleCommand with randomized data.
    /// </summary>
    /// <returns>A valid DeleteSaleCommand with randomly generated data.</returns>
    public static DeleteSaleCommand GenerateValidCommand()
    {
        return deleteSaleHandlerFaker.Generate();
    }

    /// <summary>
    /// Generates a valid Sale entity.
    /// </summary>
    /// <returns>A valid Sale entity with randomly generated data.</returns>
    public static Sale GenerateSale()
    {
        return saleFaker.Generate();
    }
}