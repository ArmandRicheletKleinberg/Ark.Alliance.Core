using System;
using System.Threading;
using System.Threading.Tasks;
using Ark.Cqrs.Messaging.Abstractions;
using Microsoft.Extensions.Options;

namespace Ark.Net.ZeroMq;

/// <summary>
/// Adapter exposing <see cref="ZeroMqConsumer"/> through the <see cref="IBrokerConsumer"/> contract.
/// </summary>
public class ZeroMqBrokerConsumer(ZeroMqConsumer consumer, IOptions<ZeroMqSettings> options) : IBrokerConsumer
{
    private readonly ZeroMqConsumer _consumer = consumer;
    private readonly ZeroMqSettings _settings = options.Value;

    /// <inheritdoc />
    public async Task SubscribeAsync<T>(Func<T, BrokerMetadata, Task> handler, CancellationToken ct = default) where T : class
    {
        var topic = _settings.Endpoint;
        await _consumer.ConsumeAsync(topic, msg => handler(msg, new BrokerMetadata(topic)), ct);
    }
}
