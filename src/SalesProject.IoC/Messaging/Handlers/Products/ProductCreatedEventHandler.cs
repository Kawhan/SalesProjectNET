using Microsoft.Extensions.Logging;
using Rebus.Handlers;
using SalesProject.Application.Products.Events;


namespace SalesProject.IoC.Messaging.Handlers.Products;

/// <summary>
/// Handles ProductCreatedEvent messages.
/// </summary>
public class ProductCreatedEventHandler : IHandleMessages<ProductCreatedEvent>
{
    private readonly ILogger<ProductCreatedEventHandler> _logger;

    public ProductCreatedEventHandler(ILogger<ProductCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(ProductCreatedEvent message)
    {
        _logger.LogInformation(
            "Product created. ProductId: {Id}, ProductName: {Name}, CurrentPrice: {CurrentPrice}, Status: {Status}, CreatedAt: {CreatedAt}",
            message.Id,
            message.Name,
            message.CurrentPrice,
            message.Status,
            message.CreatedAt);

        return Task.CompletedTask;
    }
}


