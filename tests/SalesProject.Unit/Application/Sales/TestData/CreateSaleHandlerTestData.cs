using Bogus;
using SalesProject.Application.Sales.CreateSale;
using SalesProject.Domain.Entities;
using SalesProject.Domain.Enums;

namespace SalesProject.Unit.Application.Sales.TestData;

/// <summary>
/// Provides methods for generating test data using the Bogus library.
/// This class centralizes all test data generation to ensure consistency
/// across test cases.
/// </summary>
public static class CreateSaleHandlerTestData
{
    /// <summary>
    /// Configures the Faker to generate valid CreateSaleCommand instances.
    /// </summary>
    private static readonly Faker<CreateSaleCommand> createSaleHandlerFaker = new Faker<CreateSaleCommand>()
        .RuleFor(s => s.UserId, f => f.Random.Guid())
        .RuleFor(s => s.BranchId, f => f.Random.Guid())
        .RuleFor(s => s.Items, f => new List<CreateSaleItemCommand>
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
        .RuleFor(b => b.Status, f => f.PickRandom(BranchStatus.Active, BranchStatus.Inactive))
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
    /// Generates a valid CreateSaleCommand with randomized data.
    /// </summary>
    public static CreateSaleCommand GenerateValidCommand()
    {
        return createSaleHandlerFaker.Generate();
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
    public static Product GenerateProduct(Guid? id = null, decimal price = 100, ProductStatus status = ProductStatus.Active)
    {
        var product = productFaker.Generate();

        if (id.HasValue)
            product.Id = id.Value;

        product.CurrentPrice = price;
        product.Status = status;

        return product;
    }

    /// <summary>
    /// Generates a Sale entity based on a CreateSaleCommand.
    /// </summary>
    public static Sale GenerateSaleFromCommand(CreateSaleCommand command)
    {
        return new Sale
        {
            Id = Guid.NewGuid(),
            UserId = command.UserId,
            BranchId = command.BranchId,
            Items = command.Items.Select(item => new SaleItem
            {
                Id = Guid.NewGuid(),
                ProductId = item.ProductId,
                Quantity = item.Quantity
            }).ToList()
        };
    }

    /// <summary>
    /// Generates a CreateSaleResult object based on a Sale entity.
    /// </summary>
    public static CreateSaleResult GenerateSaleResult(Sale sale)
    {
        return new CreateSaleResult
        {
            Id = sale.Id,
            SaleNumber = sale.SaleNumber,
            SaleDate = sale.SaleDate,
            UserId = sale.UserId,
            BranchId = sale.BranchId,
            TotalAmount = sale.TotalAmount,
            Status = sale.Status,
            Items = sale.Items.Select(item => new CreateSaleItemResult
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