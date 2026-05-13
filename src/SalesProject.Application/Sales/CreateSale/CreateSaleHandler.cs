using AutoMapper;
using MediatR;
using SalesProject.Application.Common.Messaging;
using SalesProject.Application.Sales.Events;
using SalesProject.Domain.Entities;
using SalesProject.Domain.Enums;
using SalesProject.Domain.Repositories;

using FluentValidation;

namespace SalesProject.Application.Sales.CreateSale;

/// <summary>
/// Handles the CreateSaleCommand request
/// </summary>
public class CreateSaleHandler : IRequestHandler<CreateSaleCommand, CreateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IUserRepository _userRepository;
    private readonly IBranchRepository _branchRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly IMessageBusService _messageBusService;

    /// <summary>
    /// Initializes a new instance of CreateSaleHandler
    /// </summary>
    /// <param name="saleRepository">The sale repository instance</param>
    /// <param name="userRepository">The user repository instance</param>
    /// <param name="branchRepository">The branch repository instance</param>
    /// <param name="productRepository">The product repository instance</param>
    /// <param name="mapper">The AutoMapper instance</param>
    /// <param name="messageBusService">The Message Bus Service instance</param>
    public CreateSaleHandler(
        ISaleRepository saleRepository,
        IUserRepository userRepository,
        IBranchRepository branchRepository,
        IProductRepository productRepository,
        IMapper mapper,
        IMessageBusService messageBusService)
    {
        _saleRepository = saleRepository;
        _userRepository = userRepository;
        _branchRepository = branchRepository;
        _productRepository = productRepository;
        _mapper = mapper;
        _messageBusService = messageBusService;
    }

    /// <summary>
    /// Handles the CreateSaleCommand request
    /// </summary>
    /// <param name="command">The CreateSale command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created sale details</returns>
    public async Task<CreateSaleResult> Handle(
        CreateSaleCommand command,
        CancellationToken cancellationToken)
    {

        var validator = new CreateSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        await ValidateUserExistsAsync(command.UserId, cancellationToken);
        await ValidateBranchExistsAsync(command.BranchId, cancellationToken);

        var sale = _mapper.Map<Sale>(command);

        MergeRepeatedItems(sale);

        var products = await GetProductsAsync(sale, cancellationToken);

        ApplyItemCalculations(sale, products);
        CalculateSaleTotal(sale);

        var createdSale = await _saleRepository.CreateAsync(sale, cancellationToken);

        await _messageBusService.PublishAsync(new SaleCreatedEvent
        {
            SaleId = createdSale.Id,
            SaleNumber = createdSale.SaleNumber,
            UserId = createdSale.UserId,
            BranchId = createdSale.BranchId,
            TotalAmount = createdSale.TotalAmount,
            CreatedAt = createdSale.CreatedAt
        }, cancellationToken);


        return _mapper.Map<CreateSaleResult>(createdSale);
    }

    private async Task ValidateUserExistsAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);

        if (user is null)
            throw new KeyNotFoundException($"User with id {userId} was not found.");
    }

    private async Task ValidateBranchExistsAsync(
        Guid branchId,
        CancellationToken cancellationToken)
    {
        var branch = await _branchRepository.GetByIdAsync(branchId, cancellationToken);

        if (branch is null)
            throw new KeyNotFoundException($"Branch with id {branchId} was not found.");
    }

    private static void MergeRepeatedItems(Sale sale)
    {
        sale.Items = sale.Items
            .GroupBy(item => item.ProductId)
            .Select(group => new SaleItem
            {
                ProductId = group.Key,
                Quantity = group.Sum(item => item.Quantity)
            })
            .ToList();
    }

    private async Task<List<Product>> GetProductsAsync(
        Sale sale,
        CancellationToken cancellationToken)
    {
        var productIds = sale.Items
            .Select(item => item.ProductId)
            .Distinct()
            .ToList();

        var products = await _productRepository.GetByIdsAsync(productIds, cancellationToken);

        if (products.Count != productIds.Count)
            throw new KeyNotFoundException("One or more products were not found.");

        if (products.Any(product => product.Status != ProductStatus.Active))
            throw new InvalidOperationException("One or more products are inactive.");

        return products;
    }

    private static void ApplyItemCalculations(
        Sale sale,
        List<Product> products)
    {
        foreach (var item in sale.Items)
        {
            var product = products.First(product => product.Id == item.ProductId);

            item.UnitPrice = product.CurrentPrice;
            item.DiscountPercentage = GetDiscountPercentage(item.Quantity);

            var subtotalAmount = item.Quantity * item.UnitPrice;

            item.Discount = subtotalAmount * (item.DiscountPercentage / 100);
            item.TotalAmount = subtotalAmount - item.Discount;
            item.Status = SaleItemStatus.Active;
        }
    }

    private static void CalculateSaleTotal(Sale sale)
    {
        sale.TotalAmount = sale.Items.Sum(item => item.TotalAmount);
    }

    private static decimal GetDiscountPercentage(int quantity)
    {
        if (quantity >= 10 && quantity <= 20)
            return 20;

        if (quantity >= 4)
            return 10;

        return 0;
    }
}