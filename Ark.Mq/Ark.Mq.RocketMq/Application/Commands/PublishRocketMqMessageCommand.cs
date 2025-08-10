using Ark.Cqrs.Messaging;

/// <summary>
/// Command describing a message to publish on RocketMQ.
/// </summary>
/// <typeparam name="TMessage">Type of the message payload.</typeparam>
/// <param name="Exchange">Target exchange name.</param>
/// <param name="RoutingKey">Routing key used for the message.</param>
/// <param name="Message">Message payload to serialize.</param>
public sealed record PublishRocketMqMessageCommand<TMessage>(string Exchange, string RoutingKey, TMessage Message) : IRequest where TMessage : class;
