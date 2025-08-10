using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MQTTnet;
using Ark;

namespace Ark.Net.Mqtt.Iot.Emqx5;

/// <summary>
/// Basic topic consumer using an action callback.
/// </summary>
public class Emqx5BkrMttqConsumer
{
    #region Fields
    private readonly Emqx5BkrMttqConnectionPool _pool;
    private readonly ILogger<Emqx5BkrMttqConsumer> _logger;
    #endregion

    /// <summary>
    /// Initializes a new instance of <see cref="Emqx5BkrMttqConsumer"/>.
    /// </summary>
    /// <param name="pool">Connection pool.</param>
    /// <param name="logger">Logger instance.</param>
    #region Constructors
    public Emqx5BkrMttqConsumer(Emqx5BkrMttqConnectionPool pool, ILogger<Emqx5BkrMttqConsumer> logger)
    {
        _pool = pool;
        _logger = logger;
    }
    #endregion

    /// <summary>
    /// Subscribes to a topic and processes incoming messages.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message.</typeparam>
    /// <param name="topic">Topic to subscribe to.</param>
    /// <param name="onMessage">Callback executed for each received message.</param>
    /// <param name="token">Optional cancellation token.</param>
    /// <returns>
    /// Success : The execution has succeeded.
    /// Unexpected : An unexpected error occurs.
    /// </returns>
    #region Methods (Public)
    public Task<Result> ConsumeAsync<TMessage>(string topic, Func<TMessage, Task> onMessage, CancellationToken token = default) where TMessage : class
        => ConsumeAsync(topic, ctx => onMessage(ctx.Payload), token);

    public async Task<Result> ConsumeAsync<TMessage>(string topic, Func<MessageContext<TMessage>, Task> onMessage, CancellationToken token = default) where TMessage : class
    {
        var client = await _pool.AcquireAsync();
        try
        {
            client.ApplicationMessageReceivedAsync += args =>
            {
                var msg = JsonSerializer.Deserialize<TMessage>(args.ApplicationMessage.Payload);
                if (msg != null)
                {
                    var ctx = new MessageContext<TMessage>(msg);
                    return onMessage(ctx);
                }
                return Task.CompletedTask;
            };

            var subscribeOptions = new MqttClientSubscribeOptionsBuilder()
                .WithTopicFilter(topic)
                .Build();
            await client.SubscribeAsync(subscribeOptions, cancellationToken: token);
            _logger.LogInformation("Consuming topic {Topic}", topic);

            token.Register(async () =>
            {
                var unsubscribeOptions = new MqttClientUnsubscribeOptionsBuilder()
                    .WithTopicFilter(topic)
                    .Build();
                await client.UnsubscribeAsync(unsubscribeOptions);
                _pool.Release(client);
            });

            return Result.Success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while consuming topic {Topic}", topic);
            _pool.Release(client);
            return new Result(ex);
        }
    }
    #endregion
}
