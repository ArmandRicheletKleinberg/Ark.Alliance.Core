

using System.Threading;
using System.Threading.Tasks;
using Ark.Cqrs.Messaging;

namespace Ark.Net.Mqtt.Iot.Emqx5;

/// <summary>
/// Handles <see cref="PublishEmqx5BkrMttqMessageCommand{TMessage}"/> by delegating to <see cref="Emqx5BkrMttqPublisher"/>.
/// </summary>
/// <typeparam name="TMessage">Type of the message payload.</typeparam>
public class PublishEmqx5BkrMttqMessageHandler<TMessage> : IRequestHandler<PublishEmqx5BkrMttqMessageCommand<TMessage>> where TMessage : class
{
    #region Fields
    private readonly Emqx5BkrMttqPublisher _publisher;
    #endregion

    /// <summary>
    /// Creates a new handler instance.
    /// </summary>
    /// <param name="publisher">Publisher used to send messages.</param>
    /// <summary>
    /// Creates a new handler instance.
    /// </summary>
    /// <param name="publisher">Publisher used to send messages.</param>
    #region Constructors
    public PublishEmqx5BkrMttqMessageHandler(Emqx5BkrMttqPublisher publisher)
    {
        _publisher = publisher;
    }
    #endregion

    /// <inheritdoc />
    /// <inheritdoc />
    #region Methods (Public)
    public Task Handle(PublishEmqx5BkrMttqMessageCommand<TMessage> request, CancellationToken cancellationToken)
    {
        return _publisher.PublishAsync(request.Topic, request.Message, cancellationToken);
    }
    #endregion
}
