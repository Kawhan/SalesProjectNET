using Bogus;
using SalesProject.Application.Sales.GetSale;
using SalesProject.Domain.Entities;
using SalesProject.Domain.Enums;

namespace SalesProject.Unit.Application.Sales.TestData;

/// <summary>
/// Provides methods for generating test data using the Bogus library.
/// This class centralizes all test data generation to ensure consistency
/// across test cases.
/// </summary>
public static class GetSaleHandlerTestData
{
    /// <summary>
    /// Configures the Faker to generate valid GetSaleCommand instances.
    /// </summary>
    private static readonly Faker<GetSaleCommand> getSaleHandlerFaker = new Faker<GetSaleCommand>()
        .CustomInstantiator(f => new GetSaleCommand(f.Random.Guid()));

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
        .RuleFor(s => s.User, f => new User
        {
            Id = f.Random.Guid(),
            Username = f.Internet.UserName(),
            Email = f.Internet.Email()
        })
        .RuleFor(s => s.Branch, f => new Branch
        {
            Id = f.Random.Guid(),
            Name = f.Company.CompanyName(),
            Address = f.Address.FullAddress(),
            Status = BranchStatus.Active
        })
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
    /// Generates a valid GetSaleCommand with randomized data.
    /// </summary>
    public static GetSaleCommand GenerateValidCommand()
    {
        return getSaleHandlerFaker.Generate();
    }

    /// <summary>
    /// Generates a valid Sale entity.
    /// </summary>
    public static Sale GenerateSale()
    {
        return saleFaker.Generate();
    }

    /// <summary>
    /// Generates a GetSaleResult object based on a Sale entity.
    /// </summary>
    public static GetSaleResult GenerateSaleResult(Sale sale)
    {
        return new GetSaleResult
        {
            Id = sale.Id,
            SaleNumber = sale.SaleNumber,
            SaleDate = sale.SaleDate,
            UserId = sale.UserId,
            User = new GetSaleUserResult
            {
                Id = sale.User.Id,
                Username = sale.User.Username,
                Email = sale.User.Email
            },
            BranchId = sale.BranchId,
            Branch = new GetSaleBranchResult
            {
                Id = sale.Branch.Id,
                Name = sale.Branch.Name,
                Address = sale.Branch.Address,
                Status = sale.Branch.Status
            },
            TotalAmount = sale.TotalAmount,
            Status = sale.Status,
            CreatedAt = sale.CreatedAt,
            UpdatedAt = sale.UpdatedAt,
            Items = sale.Items.Select(item => new GetSaleItemResult
            {
                Id = item.Id,
                ProductId = item.ProductId,
                Product = new GetSaleProductResult
                {
                    Id = item.Product.Id,
                    Name = item.Product.Name,
                    CurrentPrice = item.Product.CurrentPrice,
                    Status = item.Product.Status
                },
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                DiscountPercentage = item.DiscountPercentage,
                Discount = item.Discount,
                TotalAmount = item.TotalAmount,
                Status = item.Status
            }).ToList()
        };
    }
}
