using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ark.Cqrs.Messaging.Abstractions;
using Microsoft.Extensions.Options;

namespace Ark.Mq.RabbitMq;

/// <summary>
/// Adapter exposing <see cref="RabbitMqConsumer"/> through the <see cref="IBrokerConsumer"/> contract.
/// </summary>
public class RabbitMqBrokerConsumer(RabbitMqConsumer consumer, IOptions<RabbitMqSettings> options) : IBrokerConsumer
{
    private readonly RabbitMqConsumer _consumer = consumer;
    private readonly RabbitMqSettings _settings = options.Value;

    /// <inheritdoc />
    public async Task SubscribeAsync<T>(Func<T, BrokerMetadata, Task> handler, CancellationToken ct = default) where T : class
    {
        var queue = _settings.QueueName;
        await _consumer.ConsumeAsync(queue, ctx =>
        {
            var headers = ctx.Headers?.ToDictionary(k => k.Key, v => v.Value?.ToString() ?? string.Empty);
            var meta = new BrokerMetadata(queue, headers);
            return handler(ctx.Payload, meta);
        }, ct);
    }
}
