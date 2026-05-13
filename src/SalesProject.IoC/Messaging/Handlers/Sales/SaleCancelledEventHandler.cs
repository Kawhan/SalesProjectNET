using Microsoft.Extensions.Logging;
using Rebus.Handlers;
using SalesProject.Application.Sales.Events;


namespace SalesProject.IoC.Messaging.Handlers.Sales;

/// <summary>
/// Handles SaleCancelledvent messages.
/// </summary>
public class SaleCancelledEventHandler : IHandleMessages<SaleCancelledEvent>
{
    private readonly ILogger<SaleCancelledEventHandler> _logger;

    public SaleCancelledEventHandler(ILogger<SaleCancelledEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(SaleCancelledEvent message)
    {
        _logger.LogInformation(
            "Sale cancelled. SaleId: {SaleId}, SaleNumber: {SaleNumber}, UserId: {UserId}, BranchId: {BranchId}, TotalAmount: {TotalAmount}, CreatedAt: {CreatedAt}, CancelledAt: {CancelledAt}",
            message.SaleId,
            message.SaleNumber,
            message.UserId,
            message.BranchId,
            message.TotalAmount,
            message.CreatedAt,
            message.CancelledAt);

        return Task.CompletedTask;
    }
}