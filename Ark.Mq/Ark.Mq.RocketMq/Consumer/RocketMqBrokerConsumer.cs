using System;
using System.Threading;
using System.Threading.Tasks;
using Ark.Cqrs.Messaging.Abstractions;
using Microsoft.Extensions.Options;

namespace Ark.Net.RocketMq;

/// <summary>
/// Adapter exposing <see cref="RocketMqConsumer"/> through the <see cref="IBrokerConsumer"/> contract.
/// </summary>
public class RocketMqBrokerConsumer(RocketMqConsumer consumer, IOptions<RocketMqSettings> options) : IBrokerConsumer
{
    private readonly RocketMqConsumer _consumer = consumer;
    private readonly RocketMqSettings _settings = options.Value;

    /// <inheritdoc />
    public async Task SubscribeAsync<T>(Func<T, BrokerMetadata, Task> handler, CancellationToken ct = default) where T : class
    {
        var topic = _settings.TopicName;
        await _consumer.ConsumeAsync(topic, msg => handler(msg, new BrokerMetadata(topic)), ct);
    }
}
