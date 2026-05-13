using Microsoft.Extensions.Logging;
using Rebus.Handlers;
using SalesProject.Application.Sales.Events;


namespace SalesProject.IoC.Messaging.Handlers.Sales;

/// <summary>
/// Handles SaleItemsCancelledEvent messages.
/// </summary>
public class SaleItemsCancelledEventHandler
    : IHandleMessages<SaleItemsCancelledEvent>
{
    private readonly ILogger<SaleItemsCancelledEventHandler> _logger;

    public SaleItemsCancelledEventHandler(
        ILogger<SaleItemsCancelledEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(SaleItemsCancelledEvent message)
    {
        _logger.LogInformation(
            "Sale items cancelled. SaleId: {SaleId}, SaleNumber: {SaleNumber}, CancelledItemsCount: {CancelledItemsCount}, CancelledAt: {CancelledAt}",
            message.SaleId,
            message.SaleNumber,
            message.Items.Count,
            message.CancelledAt);

        foreach (var item in message.Items)
        {
            _logger.LogInformation(
                "Cancelled sale item. SaleItemId: {SaleItemId}, ProductId: {ProductId}, Quantity: {Quantity}, UnitPrice: {UnitPrice}, TotalAmount: {TotalAmount}",
                item.SaleItemId,
                item.ProductId,
                item.Quantity,
                item.UnitPrice,
                item.TotalAmount);
        }

        return Task.CompletedTask;
    }
}

