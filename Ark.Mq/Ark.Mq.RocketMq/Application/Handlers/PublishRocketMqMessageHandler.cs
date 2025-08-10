

using System.Threading;
using System.Threading.Tasks;
using Ark.Cqrs.Messaging;

namespace Ark.Net.RocketMq;

/// <summary>
/// Handles <see cref="PublishRocketMqMessageCommand{TMessage}"/> by delegating to <see cref="RocketMqPublisher"/>.
/// </summary>
/// <typeparam name="TMessage">Type of the message payload.</typeparam>
public class PublishRocketMqMessageHandler<TMessage> : IRequestHandler<PublishRocketMqMessageCommand<TMessage>> where TMessage : class
{
    private readonly RocketMqPublisher _publisher;

    /// <summary>
    /// Creates a new handler instance.
    /// </summary>
    /// <param name="publisher">Publisher used to send messages.</param>
    public PublishRocketMqMessageHandler(RocketMqPublisher publisher)
    {
        _publisher = publisher;
    }

    /// <inheritdoc />
    public Task Handle(PublishRocketMqMessageCommand<TMessage> request, CancellationToken cancellationToken)
    {
        return _publisher.PublishAsync(request.Exchange, request.RoutingKey, request.Message, cancellationToken);
    }
}
