using Bogus;
using SalesProject.Application.Sales.ReactivateSale;
using SalesProject.Domain.Entities;
using SalesProject.Domain.Enums;

namespace SalesProject.Unit.Application.Sales.TestData;

/// <summary>
/// Provides methods for generating test data using the Bogus library.
/// This class centralizes all test data generation to ensure consistency
/// across test cases.
/// </summary>
public static class ReactivateSaleHandlerTestData
{
    /// <summary>
    /// Configures the Faker to generate valid ReactivateSaleCommand instances.
    /// </summary>
    private static readonly Faker<ReactivateSaleCommand> reactivateSaleHandlerFaker = new Faker<ReactivateSaleCommand>()
        .CustomInstantiator(f => new ReactivateSaleCommand(f.Random.Guid()));

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
        .RuleFor(s => s.Status, SaleStatus.Active)
        .RuleFor(s => s.CreatedAt, f => f.Date.Past())
        .RuleFor(s => s.UpdatedAt, f => f.Date.Recent());

    /// <summary>
    /// Generates a valid ReactivateSaleCommand with randomized data.
    /// </summary>
    /// <returns>A valid ReactivateSaleCommand with randomly generated data.</returns>
    public static ReactivateSaleCommand GenerateValidCommand()
    {
        return reactivateSaleHandlerFaker.Generate();
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