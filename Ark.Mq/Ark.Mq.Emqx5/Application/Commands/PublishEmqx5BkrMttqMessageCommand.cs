

namespace Ark.Net.Mqtt.Iot.Emqx5;

/// <summary>
/// Command describing a message to publish via MQTT.
/// </summary>
/// <typeparam name="TMessage">Type of the message payload.</typeparam>
/// <param name="Topic">Target topic.</param>
/// <param name="Message">Message payload to serialize.</param>
public sealed record PublishEmqx5BkrMttqMessageCommand<TMessage>(string Topic, TMessage Message) : IRequest where TMessage : class;
