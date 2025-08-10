using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet;

namespace Ark.Net.Mqtt.Iot.Emqx5;

/// <summary>
/// Background service that consumes messages from a topic.
/// </summary>
public class Emqx5BkrMttqBackgroundConsumer<TMessage> : BackgroundService where TMessage : class
{
    #region Fields
    private readonly Emqx5BkrMttqConnectionPool _pool;
    private readonly ILogger<Emqx5BkrMttqBackgroundConsumer<TMessage>> _logger;
    private readonly string _topic;
    private readonly Func<TMessage, Task> _onMessage;
    #endregion

    /// <summary>
    /// Initializes a new instance of <see cref="Emqx5BkrMttqBackgroundConsumer{TMessage}"/>.
    /// </summary>
    /// <param name="pool">Connection pool.</param>
    /// <param name="logger">Logger instance.</param>
    /// <param name="topic">Topic to subscribe to.</param>
    /// <param name="onMessage">Callback to execute for each message.</param>
    #region Constructors
    public Emqx5BkrMttqBackgroundConsumer(Emqx5BkrMttqConnectionPool pool, ILogger<Emqx5BkrMttqBackgroundConsumer<TMessage>> logger, string topic, Func<TMessage, Task> onMessage)
    {
        _pool = pool;
        _logger = logger;
        _topic = topic;
        _onMessage = onMessage;
    }
    #endregion

    #region Methods (Protected)
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var client = await _pool.AcquireAsync();
        client.ApplicationMessageReceivedAsync += args =>
        {
            var msg = JsonSerializer.Deserialize<TMessage>(args.ApplicationMessage.Payload);
            if (msg != null)
                return _onMessage(msg);
            return Task.CompletedTask;
        };

        var subscribeOptions = new MqttClientSubscribeOptionsBuilder()
            .WithTopicFilter(_topic)
            .Build();
        await client.SubscribeAsync(subscribeOptions, cancellationToken: stoppingToken);
        _logger.LogInformation("Consuming topic {Topic}", _topic);

        while (!stoppingToken.IsCancellationRequested)
            await Task.Delay(1000, stoppingToken);

        var unsubscribeOptions = new MqttClientUnsubscribeOptionsBuilder()
            .WithTopicFilter(_topic)
            .Build();
        await client.UnsubscribeAsync(unsubscribeOptions, cancellationToken: CancellationToken.None);
        _pool.Release(client);
    }
    #endregion
}
