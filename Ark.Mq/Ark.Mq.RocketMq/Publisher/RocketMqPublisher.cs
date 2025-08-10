using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Ark;
using Ark.Cqrs.Messaging.Abstractions;
using System.Diagnostics;
using Ark.Net.RocketMq.Diagnostics;

namespace Ark.Net.RocketMq;

/// <summary>
/// Simple message publisher.
/// </summary>
public class RocketMqPublisher : IBrokerProducer
{
    private readonly RocketMqConnectionPool _pool;
    private readonly ILogger<RocketMqPublisher> _logger;
    private static readonly ActivitySource ActivitySource = new("Ark.Mq.RocketMq.Publisher");

    public RocketMqPublisher(RocketMqConnectionPool pool, ILogger<RocketMqPublisher> logger)
    {
        _pool = pool;
        _logger = logger;
    }

    public Task PublishAsync<T>(T msg, BrokerMetadata meta, CancellationToken token = default)
        => PublishAsync(meta.Topic, meta.Topic, msg!, token);

    /// <summary>
    /// Publishes a message to the specified topic.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message.</typeparam>
    /// <param name="exchange">The topic name.</param>
    /// <param name="routingKey">The tag or routing key.</param>
    /// <param name="message">The message instance.</param>
    /// <param name="token">Cancellation token.</param>
    /// <returns>
    /// Success : The execution has succeeded.
    /// Unexpected : An unexpected error occurs.
    /// </returns>
    public async Task<Result> PublishAsync<TMessage>(string exchange, string routingKey, TMessage message, CancellationToken token = default) where TMessage : class
    {
        var producer = await _pool.AcquireAsync();
        try
        {
            using var activity = ActivitySource.StartActivity("rocketmq.publish", ActivityKind.Producer);
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
            var msg = new Apache.RocketMQ.Client.Message(exchange, body)
            {
                Tag = routingKey
            };
            await producer.SendAsync(msg, token);
            _logger.LogInformation("Published message to {Topic}/{Tag}", exchange, routingKey);
            RocketMqMetrics.MessagesPublished.Add(1);
            return Result.Success;
        }
        catch (Exception ex)
        {
            RocketMqMetrics.MessagesFailed.Add(1);
            _logger.LogError(ex, "Publishing message failed");
            return new Result(ex);
        }
        finally
        {
            _pool.Release(producer);
        }
    }

    /// <summary>
    /// Publishes a message wrapped in a <see cref="MessageContext{TPayload}"/>.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message.</typeparam>
    /// <param name="exchange">The topic name.</param>
    /// <param name="routingKey">Tag or routing key.</param>
    /// <param name="context">Message context including headers.</param>
    /// <param name="token">Cancellation token.</param>
    /// <returns>A <see cref="Result"/> describing the outcome.</returns>
    public Task<Result> PublishAsync<TMessage>(string exchange, string routingKey, MessageContext<TMessage> context, CancellationToken token = default) where TMessage : class
        => PublishAsync(exchange, routingKey, context.Payload, token);
}
