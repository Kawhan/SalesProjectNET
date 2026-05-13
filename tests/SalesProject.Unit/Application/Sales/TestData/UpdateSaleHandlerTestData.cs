using Bogus;
using SalesProject.Application.Sales.UpdateSale;
using SalesProject.Domain.Entities;
using SalesProject.Domain.Enums;

namespace SalesProject.Unit.Application.Sales.TestData;

/// <summary>
/// Provides methods for generating test data using the Bogus library.
/// This class centralizes all test data generation to ensure consistency
/// across test cases.
/// </summary>
public static class UpdateSaleHandlerTestData
{
    /// <summary>
    /// Configures the Faker to generate valid UpdateSaleCommand instances.
    /// </summary>
    private static readonly Faker<UpdateSaleCommand> updateSaleHandlerFaker = new Faker<UpdateSaleCommand>()
        .RuleFor(s => s.Id, f => f.Random.Guid())
        .RuleFor(s => s.UserId, f => f.Random.Guid())
        .RuleFor(s => s.BranchId, f => f.Random.Guid())
        .RuleFor(s => s.Items, f => new List<UpdateSaleItemCommand>
        {
            new()
            {
                ProductId = f.Random.Guid(),
                Quantity = f.Random.Int(1, 3)
            }
        });

    /// <summary>
    /// Configures the Faker to generate valid User entities.
    /// </summary>
    private static readonly Faker<User> userFaker = new Faker<User>()
        .RuleFor(u => u.Id, f => f.Random.Guid())
        .RuleFor(u => u.Username, f => f.Internet.UserName())
        .RuleFor(u => u.Email, f => f.Internet.Email())
        .RuleFor(u => u.Phone, f => $"+55{f.Random.Number(11, 99)}{f.Random.Number(100000000, 999999999)}")
        .RuleFor(u => u.Password, f => $"Test@{f.Random.Number(100, 999)}")
        .RuleFor(u => u.Status, f => f.PickRandom(UserStatus.Active, UserStatus.Suspended))
        .RuleFor(u => u.Role, f => f.PickRandom(UserRole.Customer, UserRole.Admin))
        .RuleFor(u => u.CreatedAt, f => f.Date.Past());

    /// <summary>
    /// Configures the Faker to generate valid Branch entities.
    /// </summary>
    private static readonly Faker<Branch> branchFaker = new Faker<Branch>()
        .RuleFor(b => b.Id, f => f.Random.Guid())
        .RuleFor(b => b.Name, f => f.Company.CompanyName())
        .RuleFor(b => b.Address, f => f.Address.FullAddress())
        .RuleFor(b => b.Status, BranchStatus.Active)
        .RuleFor(b => b.CreatedAt, f => f.Date.Past());

    /// <summary>
    /// Configures the Faker to generate valid Product entities.
    /// </summary>
    private static readonly Faker<Product> productFaker = new Faker<Product>()
        .RuleFor(p => p.Id, f => f.Random.Guid())
        .RuleFor(p => p.Name, f => f.Commerce.ProductName())
        .RuleFor(p => p.CurrentPrice, f => f.Random.Decimal(1, 1000))
        .RuleFor(p => p.Status, ProductStatus.Active)
        .RuleFor(p => p.CreatedAt, f => f.Date.Past());

    /// <summary>
    /// Generates a valid UpdateSaleCommand with randomized data.
    /// </summary>
    public static UpdateSaleCommand GenerateValidCommand()
    {
        return updateSaleHandlerFaker.Generate();
    }

    /// <summary>
    /// Generates a valid User entity.
    /// </summary>
    public static User GenerateUser(Guid? id = null)
    {
        var user = userFaker.Generate();

        if (id.HasValue)
            user.Id = id.Value;

        return user;
    }

    /// <summary>
    /// Generates a valid Branch entity.
    /// </summary>
    public static Branch GenerateBranch(Guid? id = null)
    {
        var branch = branchFaker.Generate();

        if (id.HasValue)
            branch.Id = id.Value;

        return branch;
    }

    /// <summary>
    /// Generates a valid Product entity.
    /// </summary>
    public static Product GenerateProduct(
        Guid? id = null,
        decimal price = 100,
        ProductStatus status = ProductStatus.Active)
    {
        var product = productFaker.Generate();

        if (id.HasValue)
            product.Id = id.Value;

        product.CurrentPrice = price;
        product.Status = status;

        return product;
    }

    /// <summary>
    /// Generates a valid active Sale entity.
    /// </summary>
    public static Sale GenerateSale(Guid? id = null, Guid? userId = null, Guid? branchId = null)
    {
        var productId = Guid.NewGuid();

        var sale = new Sale
        {
            Id = id ?? Guid.NewGuid(),
            UserId = userId ?? Guid.NewGuid(),
            BranchId = branchId ?? Guid.NewGuid(),
            Status = SaleStatus.Active,
            Items = new List<SaleItem>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    ProductId = productId,
                    Quantity = 2,
                    UnitPrice = 100,
                    DiscountPercentage = 0,
                    Discount = 0,
                    TotalAmount = 200,
                    Status = SaleItemStatus.Active
                }
            }
        };

        sale.RecalculateTotal();

        return sale;
    }

    /// <summary>
    /// Generates an UpdateSaleResult object based on a Sale entity.
    /// </summary>
    public static UpdateSaleResult GenerateSaleResult(Sale sale)
    {
        return new UpdateSaleResult
        {
            Id = sale.Id,
            SaleNumber = sale.SaleNumber,
            SaleDate = sale.SaleDate,
            UserId = sale.UserId,
            BranchId = sale.BranchId,
            TotalAmount = sale.TotalAmount,
            Status = sale.Status,
            UpdatedAt = sale.UpdatedAt,
            Items = sale.Items.Select(item => new UpdateSaleItemResult
            {
                Id = item.Id,
                ProductId = item.ProductId,
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
