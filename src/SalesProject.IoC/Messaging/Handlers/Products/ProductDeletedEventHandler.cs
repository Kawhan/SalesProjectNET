using Microsoft.Extensions.Logging;
using Rebus.Handlers;
using SalesProject.Application.Products.Events;


namespace SalesProject.IoC.Messaging.Handlers.Products;

/// <summary>
/// Handles ProductDeletedEvent messages.
/// </summary>
public class ProductDeletedEventHandler : IHandleMessages<ProductDeletedEvent>
{
    private readonly ILogger<ProductDeletedEventHandler> _logger;

    public ProductDeletedEventHandler(ILogger<ProductDeletedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(ProductDeletedEvent message)
    {
        _logger.LogInformation(
            "Product deleted. ProductId: {Id}, ProductName: {Name}, CurrentPrice: {CurrentPrice}, Status: {Status}, CreatedAt: {CreatedAt}",
            message.Id,
            message.Name,
            message.CurrentPrice,
            message.Status,
            message.CreatedAt);

        return Task.CompletedTask;
    }
}

