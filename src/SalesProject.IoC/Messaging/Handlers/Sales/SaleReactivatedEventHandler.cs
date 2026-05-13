using Microsoft.Extensions.Logging;
using Rebus.Handlers;
using SalesProject.Application.Sales.Events;

namespace SalesProject.IoC.Messaging.Handlers.Sales;

/// <summary>
/// Handles SaleReactivatedEvent messages.
/// </summary>
public class SaleReactivatedEventHandler : IHandleMessages<SaleReactivatedEvent>
{
    private readonly ILogger<SaleReactivatedEventHandler> _logger;

    public SaleReactivatedEventHandler(ILogger<SaleReactivatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(SaleReactivatedEvent message)
    {
        _logger.LogInformation(
            "Sale reactivated. SaleId: {SaleId}, SaleNumber: {SaleNumber}, UserId: {UserId}, BranchId: {BranchId}, TotalAmount: {TotalAmount}, CreatedAt: {CreatedAt}, Reactivated: {CancelledAt}",
            message.SaleId,
            message.SaleNumber,
            message.UserId,
            message.BranchId,
            message.TotalAmount,
            message.CreatedAt,
            message.UpdatedAt);

        return Task.CompletedTask;
    }
}

