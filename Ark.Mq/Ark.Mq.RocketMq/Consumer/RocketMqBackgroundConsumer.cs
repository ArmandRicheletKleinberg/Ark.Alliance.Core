using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RocketMQ.Client;

namespace Ark.Net.RocketMq;

/// <summary>
/// Background service that consumes messages from a topic.
/// </summary>
public class RocketMqBackgroundConsumer<TMessage> : BackgroundService where TMessage : class
{
    #region Fields
    private readonly RocketMqConnectionPool _pool;
    private readonly ILogger<RocketMqBackgroundConsumer<TMessage>> _logger;
    private readonly string _queue;
    private readonly Func<TMessage, Task> _onMessage;
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of <see cref="RocketMqBackgroundConsumer{TMessage}"/>.
    /// </summary>
    public RocketMqBackgroundConsumer(
        RocketMqConnectionPool pool,
        ILogger<RocketMqBackgroundConsumer<TMessage>> logger,
        string queue,
        Func<TMessage, Task> onMessage)
    {
        _pool = pool;
        _logger = logger;
        _queue = queue;
        _onMessage = onMessage;
    }
    #endregion

    #region Methods (Protected)
    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var client = await _pool.AcquireAsync();
        try
        {
            client.MessageReceived += async (_, msg) =>
            {
                var message = JsonSerializer.Deserialize<TMessage>(msg.Body);
                if (message != null)
                    await _onMessage(message);
            };
            await client.SubscribeAsync(_queue, stoppingToken);
            _logger.LogInformation("Consuming topic {Topic}", _queue);

            while (!stoppingToken.IsCancellationRequested)
                await Task.Delay(1000, stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in background consumer for queue {Queue}", _queue);
        }
        finally
        {
            _pool.Release(client);
        }
    }
    #endregion
}
