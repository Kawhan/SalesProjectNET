using Bogus;
using SalesProject.Application.Sales.GetAllSales;
using SalesProject.Domain.Entities;
using SalesProject.Domain.Enums;

namespace SalesProject.Unit.Application.Sales.TestData;

/// <summary>
/// Provides methods for generating test data using the Bogus library.
/// This class centralizes all test data generation to ensure consistency
/// across test cases.
/// </summary>
public static class GetAllSalesHandlerTestData
{
    /// <summary>
    /// Configures the Faker to generate valid GetAllSalesCommand instances.
    /// </summary>
    private static readonly Faker<GetAllSalesCommand> getAllSalesHandlerFaker = new Faker<GetAllSalesCommand>()
        .RuleFor(s => s.PageNumber, f => f.Random.Int(1, 5))
        .RuleFor(s => s.PageSize, f => f.Random.Int(5, 20))
        .RuleFor(s => s.SaleNumber, f => $"SALE-{f.Date.Recent():yyyyMMddHHmmss}")
        .RuleFor(s => s.UserId, f => f.Random.Guid())
        .RuleFor(s => s.BranchId, f => f.Random.Guid())
        .RuleFor(s => s.Status, f => f.PickRandom(SaleStatus.Active, SaleStatus.Cancelled))
        .RuleFor(s => s.StartDate, f => f.Date.Past())
        .RuleFor(s => s.EndDate, f => f.Date.Recent())
        .RuleFor(s => s.MinTotalAmount, f => f.Random.Decimal(1, 100))
        .RuleFor(s => s.MaxTotalAmount, f => f.Random.Decimal(101, 1000));

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
        .RuleFor(s => s.Status, f => f.PickRandom(SaleStatus.Active, SaleStatus.Cancelled))
        .RuleFor(s => s.CreatedAt, f => f.Date.Past())
        .RuleFor(s => s.UpdatedAt, f => f.Date.Recent())
        .RuleFor(s => s.Items, f => new List<SaleItem>
        {
            new()
            {
                Id = f.Random.Guid(),
                ProductId = f.Random.Guid(),
                Quantity = f.Random.Int(1, 5),
                UnitPrice = f.Random.Decimal(1, 100),
                DiscountPercentage = 0,
                Discount = 0,
                TotalAmount = f.Random.Decimal(1, 500),
                Status = SaleItemStatus.Active,
                Product = new Product
                {
                    Id = f.Random.Guid(),
                    Name = f.Commerce.ProductName(),
                    CurrentPrice = f.Random.Decimal(1, 100),
                    Status = ProductStatus.Active
                }
            }
        });

    /// <summary>
    /// Generates a valid GetAllSalesCommand with randomized data.
    /// </summary>
    /// <returns>A valid GetAllSalesCommand with randomly generated data.</returns>
    public static GetAllSalesCommand GenerateValidCommand()
    {
        return getAllSalesHandlerFaker.Generate();
    }

    /// <summary>
    /// Generates a list of valid Sale entities.
    /// </summary>
    /// <param name="count">The number of sales to generate.</param>
    /// <returns>A list of valid Sale entities.</returns>
    public static List<Sale> GenerateSales(int count)
    {
        return saleFaker.Generate(count);
    }

    /// <summary>
    /// Generates a list of GetAllSalesResult objects based on sale entities.
    /// </summary>
    /// <param name="sales">The sale entities.</param>
    /// <returns>A list of GetAllSalesResult objects.</returns>
    public static List<GetAllSalesResult> GenerateSaleResults(List<Sale> sales)
    {
        return sales.Select(sale => new GetAllSalesResult
        {
            Id = sale.Id,
            SaleNumber = sale.SaleNumber,
            SaleDate = sale.SaleDate,
            UserId = sale.UserId,
            BranchId = sale.BranchId,
            TotalAmount = sale.TotalAmount,
            Status = sale.Status,
            ItemsCount = sale.Items.Count,
            CreatedAt = sale.CreatedAt,
            UpdatedAt = sale.UpdatedAt,
            Items = sale.Items.Select(item => new GetAllSalesItemResult
            {
                Id = item.Id,
                ProductId = item.ProductId,
                ProductName = item.Product?.Name ?? string.Empty,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                DiscountPercentage = item.DiscountPercentage,
                Discount = item.Discount,
                TotalAmount = item.TotalAmount,
                Status = item.Status
            }).ToList()
        }).ToList();
    }
}