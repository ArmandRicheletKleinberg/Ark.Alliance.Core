using System;
using System.Threading;
using System.Threading.Tasks;
using Ark.Cqrs.Messaging.Abstractions;
using Microsoft.Extensions.Options;

namespace Ark.Net.Mqtt.Iot.Emqx5;

/// <summary>
/// Adapter exposing <see cref="Emqx5BkrMttqConsumer"/> through the <see cref="IBrokerConsumer"/> contract.
/// </summary>
public class Emqx5BkrMttqBrokerConsumer(Emqx5BkrMttqConsumer consumer, IOptions<Emqx5BkrMttqSettings> options) : IBrokerConsumer
{
    private readonly Emqx5BkrMttqConsumer _consumer = consumer;
    private readonly Emqx5BkrMttqSettings _settings = options.Value;

    /// <inheritdoc />
    public async Task SubscribeAsync<T>(Func<T, BrokerMetadata, Task> handler, CancellationToken ct = default) where T : class
    {
        var topic = _settings.Topic;
        await _consumer.ConsumeAsync(topic, msg => handler(msg, new BrokerMetadata(topic)), ct);
    }
}
