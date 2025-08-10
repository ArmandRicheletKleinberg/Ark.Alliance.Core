

using System.Threading;
using System.Threading.Tasks;
using Ark.Cqrs.Messaging;

namespace Ark.Net.ZeroMq;

/// <summary>
/// Handles <see cref="PublishZeroMqMessageCommand{TMessage}"/> by delegating to <see cref="ZeroMqPublisher"/>.
/// </summary>
/// <typeparam name="TMessage">Type of the message payload.</typeparam>
public class PublishZeroMqMessageHandler<TMessage> : IRequestHandler<PublishZeroMqMessageCommand<TMessage>> where TMessage : class
{
    private readonly ZeroMqPublisher _publisher;

    /// <summary>
    /// Creates a new handler instance.
    /// </summary>
    /// <param name="publisher">Publisher used to send messages.</param>
    public PublishZeroMqMessageHandler(ZeroMqPublisher publisher)
    {
        _publisher = publisher;
    }

    /// <inheritdoc />
    public Task Handle(PublishZeroMqMessageCommand<TMessage> request, CancellationToken cancellationToken)
    {
        return _publisher.PublishAsync(request.Exchange, request.RoutingKey, request.Message, cancellationToken);
    }
}
