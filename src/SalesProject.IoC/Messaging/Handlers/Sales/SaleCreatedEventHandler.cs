using Microsoft.Extensions.Logging;
using Rebus.Handlers;
using SalesProject.Application.Sales.Events;


namespace SalesProject.IoC.Messaging.Handlers.Sales;


/// <summary>
/// Handles SaleCreatedEvent messages.
/// </summary>
public class SaleCreatedEventHandler : IHandleMessages<SaleCreatedEvent>
{
    private readonly ILogger<SaleCreatedEventHandler> _logger;

    public SaleCreatedEventHandler(ILogger<SaleCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(SaleCreatedEvent message)
    {
        _logger.LogInformation(
            "Sale created. SaleId: {SaleId}, SaleNumber: {SaleNumber}, UserId: {UserId}, BranchId: {BranchId}, TotalAmount: {TotalAmount}",
            message.SaleId,
            message.SaleNumber,
            message.UserId,
            message.BranchId,
            message.TotalAmount);

        return Task.CompletedTask;
    }
}

