using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Apache.RocketMQ.Client;
using Ark;

namespace Ark.Net.RocketMq;

/// <summary>
/// Basic queue consumer using an action callback.
/// </summary>
public class RocketMqConsumer
{
    private readonly RocketMqConnectionPool _pool;
    private readonly ILogger<RocketMqConsumer> _logger;

    public RocketMqConsumer(RocketMqConnectionPool pool, ILogger<RocketMqConsumer> logger)
    {
        _pool = pool;
        _logger = logger;
    }

    /// <summary>
    /// Consumes messages from a topic.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message.</typeparam>
    /// <param name="queue">The topic name.</param>
    /// <param name="onMessage">Callback executed for each message.</param>
    /// <param name="token">Cancellation token.</param>
    /// <returns>
    /// Success : The execution has succeeded.
    /// Unexpected : An unexpected error occurs.
    /// </returns>
    public Task<Result> ConsumeAsync<TMessage>(string queue, Func<TMessage, Task> onMessage, CancellationToken token = default) where TMessage : class
        => ConsumeAsync(queue, ctx => onMessage(ctx.Payload), token);

    public async Task<Result> ConsumeAsync<TMessage>(string queue, Func<MessageContext<TMessage>, Task> onMessage, CancellationToken token = default) where TMessage : class
    {
        var consumer = await _pool.AcquireAsync();
        try
        {
            consumer.MessageReceived += async (_, msg) =>
            {
                var message = JsonSerializer.Deserialize<TMessage>(msg.Body);
                if (message != null)
                    await onMessage(new MessageContext<TMessage>(message));
            };
            await consumer.SubscribeAsync(queue, token);
            _logger.LogInformation("Consuming topic {Topic}", queue);

            token.Register(() =>
            {
                _pool.Release(consumer);
            });

            return Result.Success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while consuming topic {Topic}", queue);
            _pool.Release(consumer);
            return new Result(ex);
        }
    }
}
