using Microsoft.Extensions.Logging;
using Rebus.Handlers;
using SalesProject.Application.Products.Events;


namespace SalesProject.IoC.Messaging.Handlers.Products;

/// <summary>
/// Handles ProductModifiedEvent messages.
/// </summary>
public class ProductModifiedEventHandler : IHandleMessages<ProductModifiedEvent>
{
    private readonly ILogger<ProductModifiedEventHandler> _logger;

    public ProductModifiedEventHandler(ILogger<ProductModifiedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(ProductModifiedEvent message)
    {
        _logger.LogInformation(
            "Product modified. ProductId: {Id}, ProductName: {Name}, CurrentPrice: {CurrentPrice}, Status: {Status}, CreatedAt: {CreatedAt}",
            message.Id,
            message.Name,
            message.CurrentPrice,
            message.Status,
            message.CreatedAt);

        return Task.CompletedTask;
    }
}

