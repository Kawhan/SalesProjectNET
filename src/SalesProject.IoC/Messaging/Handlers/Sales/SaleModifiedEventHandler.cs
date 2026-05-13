using Microsoft.Extensions.Logging;
using Rebus.Handlers;
using SalesProject.Application.Sales.Events;


namespace SalesProject.IoC.Messaging.Handlers.Sales;

/// <summary>
/// Handles SaleModifiedEvent messages.
/// </summary>
public class SaleModifiedEventHandler : IHandleMessages<SaleModifiedEvent>
{
    private readonly ILogger<SaleModifiedEventHandler> _logger;

    public SaleModifiedEventHandler(ILogger<SaleModifiedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(SaleModifiedEvent message)
    {
        _logger.LogInformation(
            "Sale modified. SaleId: {SaleId}, SaleNumber: {SaleNumber}, UserId: {UserId}, BranchId: {BranchId}, TotalAmount: {TotalAmount}, CreatedAt: {CreatedAt}, ModifiedAt: {ModifiedAt} ",
            message.SaleId,
            message.SaleNumber,
            message.UserId,
            message.BranchId,
            message.TotalAmount,
            message.CreatedAt,
            message.ModifiedAt);

        return Task.CompletedTask;
    }
}

