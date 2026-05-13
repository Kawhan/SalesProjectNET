using AutoMapper;
using MediatR;
using SalesProject.Application.Common.Messaging;
using SalesProject.Application.Sales.Events;
using SalesProject.Domain.Entities;
using SalesProject.Domain.Enums;
using SalesProject.Domain.Repositories;

using FluentValidation;

namespace SalesProject.Application.Sales.UpdateSale;

/// <summary>
/// Handler for processing UpdateSaleCommand requests.
/// </summary>
public class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, UpdateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IUserRepository _userRepository;
    private readonly IBranchRepository _branchRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly IMessageBusService _messageBusService;

    /// <summary>
    /// Initializes a new instance of UpdateSaleHandler.
    /// </summary>
    public UpdateSaleHandler(
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
    /// Handles the UpdateSaleCommand request.
    /// </summary>
    public async Task<UpdateSaleResult> Handle(
        UpdateSaleCommand command,
        CancellationToken cancellationToken)
    {
        var validator = new UpdateSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = await GetSaleAsync(command.Id, cancellationToken);

        await ValidateUserExistsAsync(command.UserId, cancellationToken);
        await ValidateBranchExistsAsync(command.BranchId, cancellationToken);

        var groupedItems = GroupItems(command.Items);
        var products = await GetProductsAsync(groupedItems, cancellationToken);

        sale.Update(command.UserId, command.BranchId);

        var cancelledItems = UpdateSaleItems(sale, groupedItems, products);

        if (cancelledItems.Any())
        {
            await _messageBusService.PublishAsync(new SaleItemsCancelledEvent
            {
                SaleId = sale.Id,
                SaleNumber = sale.SaleNumber,
                CancelledAt = DateTime.UtcNow,
                Items = cancelledItems.Select(item => new CancelledSaleItem
                {
                    SaleItemId = item.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    DiscountPercentage = item.DiscountPercentage,
                    Discount = item.Discount,
                    TotalAmount = item.TotalAmount
                }).ToList()
            }, cancellationToken);
        }

        sale.RecalculateTotal();

        var updatedSale = await _saleRepository.UpdateAsync(sale, cancellationToken);

        await _messageBusService.PublishAsync(new SaleModifiedEvent
        {
            SaleId = updatedSale.Id,
            SaleNumber = updatedSale.SaleNumber,
            UserId = updatedSale.UserId,
            BranchId = updatedSale.BranchId,
            TotalAmount = updatedSale.TotalAmount,
            CreatedAt = updatedSale.CreatedAt,
            ModifiedAt = updatedSale.UpdatedAt.GetValueOrDefault(DateTime.UtcNow),
        }, cancellationToken);

        return _mapper.Map<UpdateSaleResult>(updatedSale);
    }

    private async Task<Sale> GetSaleAsync(Guid saleId, CancellationToken cancellationToken)
    {
        var sale = await _saleRepository.GetByIdAsync(saleId, cancellationToken);

        if (sale is null)
            throw new KeyNotFoundException($"Sale with id {saleId} was not found.");

        if (sale.Status == SaleStatus.Cancelled)
            throw new InvalidOperationException("Cancelled sales cannot be updated.");

        return sale;
    }

    private async Task ValidateUserExistsAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);

        if (user is null)
            throw new KeyNotFoundException($"User with id {userId} was not found.");
    }

    private async Task ValidateBranchExistsAsync(Guid branchId, CancellationToken cancellationToken)
    {
        var branch = await _branchRepository.GetByIdAsync(branchId, cancellationToken);

        if (branch is null)
            throw new KeyNotFoundException($"Branch with id {branchId} was not found.");
    }

    private static List<GroupedSaleItem> GroupItems(IEnumerable<UpdateSaleItemCommand> items)
    {
        return items
            .GroupBy(item => item.ProductId)
            .Select(group => new GroupedSaleItem
            {
                ProductId = group.Key,
                Quantity = group.Sum(item => item.Quantity)
            })
            .ToList();
    }

    private async Task<List<Product>> GetProductsAsync(
        List<GroupedSaleItem> groupedItems,
        CancellationToken cancellationToken)
    {
        var productIds = groupedItems
            .Select(item => item.ProductId)
            .ToList();

        var products = await _productRepository.GetByIdsAsync(productIds, cancellationToken);

        if (products.Count != productIds.Count)
            throw new KeyNotFoundException("One or more products were not found.");

        if (products.Any(product => product.Status != ProductStatus.Active))
            throw new InvalidOperationException("One or more products are inactive.");


        return products;
    }

    private static List<SaleItem> UpdateSaleItems(
        Sale sale,
        List<GroupedSaleItem> groupedItems,
        List<Product> products)
    {
        var cancelledItems = CancelRemovedItems(sale, groupedItems);

        foreach (var groupedItem in groupedItems)
        {
            var product = products.First(product => product.Id == groupedItem.ProductId);

            var existingItem = sale.Items
                .FirstOrDefault(item => item.ProductId == groupedItem.ProductId);

            if (existingItem is not null)
            {
                existingItem.Update(groupedItem.Quantity, product.CurrentPrice);
                continue;
            }

            var newItem = new SaleItem
            {
                ProductId = product.Id
            };

            newItem.Update(groupedItem.Quantity, product.CurrentPrice);

            sale.Items.Add(newItem);
        }

        return cancelledItems;
    }

    private static List<SaleItem> CancelRemovedItems(Sale sale, List<GroupedSaleItem> groupedItems)
    {
        var requestProductIds = groupedItems
            .Select(item => item.ProductId)
            .ToHashSet();

        var removedItems = sale.Items
            .Where(item => item.Status == SaleItemStatus.Active)
            .Where(item => !requestProductIds.Contains(item.ProductId))
            .ToList();

        foreach (var item in removedItems)
            item.Cancel();

        return removedItems;
    }

    private class GroupedSaleItem
    {
        public Guid ProductId { get; set; }

        public int Quantity { get; set; }
    }
}