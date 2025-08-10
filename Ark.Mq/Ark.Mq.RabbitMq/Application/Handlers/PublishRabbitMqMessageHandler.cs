

using System.Threading;
using System.Threading.Tasks;
using Ark.Cqrs.Messaging;

namespace Ark.Net.RabbitMq;

/// <summary>
/// Handles <see cref="PublishRabbitMqMessageCommand{TMessage}"/> by delegating to <see cref="RabbitMqPublisher"/>.
/// </summary>
/// <typeparam name="TMessage">Type of the message payload.</typeparam>
public class PublishRabbitMqMessageHandler<TMessage> : IRequestHandler<PublishRabbitMqMessageCommand<TMessage>> where TMessage : class
{
    private readonly RabbitMqPublisher _publisher;

    /// <summary>
    /// Creates a new handler instance.
    /// </summary>
    /// <param name="publisher">Publisher used to send messages.</param>
    public PublishRabbitMqMessageHandler(RabbitMqPublisher publisher)
    {
        _publisher = publisher;
    }

    /// <inheritdoc />
    public Task Handle(PublishRabbitMqMessageCommand<TMessage> request, CancellationToken cancellationToken)
    {
        return _publisher.PublishAsync(request.Exchange, request.RoutingKey, request.Message, cancellationToken);
    }
}
