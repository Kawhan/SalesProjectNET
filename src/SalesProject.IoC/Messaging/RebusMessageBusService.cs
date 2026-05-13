using Rebus.Bus;
using SalesProject.Application.Common.Messaging;

namespace SalesProject.IoC.Messaging;

/// <summary>
/// Rebus implementation for publishing application messages.
/// </summary>
public class RebusMessageBusService : IMessageBusService
{
    private readonly IBus _bus;

    /// <summary>
    /// Initializes a new instance of RebusMessageBusService.
    /// </summary>
    /// <param name="bus">The Rebus bus instance.</param>
    public RebusMessageBusService(IBus bus)
    {
        _bus = bus;
    }

    /// <summary>
    /// Publishes a message using Rebus.
    /// </summary>
    /// <typeparam name="TMessage">The message type.</typeparam>
    /// <param name="message">The message to publish.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task PublishAsync<TMessage>(
        TMessage message,
        CancellationToken cancellationToken = default)
        where TMessage : class
    {
        await _bus.Publish(message);
    }
}


