namespace SalesProject.Application.Common.Messaging;

/// <summary>
/// Defines a contract for publishing application messages.
/// </summary>
public interface IMessageBusService
{
    /// <summary>
    /// Publishes a message to the message bus.
    /// </summary>
    /// <typeparam name="TMessage">The message type.</typeparam>
    /// <param name="message">The message to publish.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task PublishAsync<TMessage>(
        TMessage message,
        CancellationToken cancellationToken = default)
        where TMessage : class;
}