using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetMQ;
using NetMQ.Sockets;

namespace Ark.Net.ZeroMq;

/// <summary>
/// Background service that consumes messages from a topic.
/// </summary>
public class ZeroMqBackgroundConsumer<TMessage> : BackgroundService where TMessage : class
{
    #region Fields
    private readonly ZeroMqSettings _options;
    private readonly ILogger<ZeroMqBackgroundConsumer<TMessage>> _logger;
    private readonly string _topic;
    private readonly Func<TMessage, Task> _onMessage;
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of <see cref="ZeroMqBackgroundConsumer{TMessage}"/>.
    /// </summary>
    public ZeroMqBackgroundConsumer(
        IOptions<ZeroMqSettings> options,
        ILogger<ZeroMqBackgroundConsumer<TMessage>> logger,
        string topic,
        Func<TMessage, Task> onMessage)
    {
        _options = options.Value;
        _logger = logger;
        _topic = topic;
        _onMessage = onMessage;
    }
    #endregion

    #region Methods (Protected)
    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            using var sub = new SubscriberSocket();
            sub.Connect(_options.Endpoint);
            sub.Subscribe(_topic);
            _logger.LogInformation("Background consuming ZeroMQ topic {Topic}", _topic);

            while (!stoppingToken.IsCancellationRequested)
            {
                var msg = await Task.Run(sub.ReceiveFrameString, stoppingToken);
                var message = JsonSerializer.Deserialize<TMessage>(msg);
                if (message != null)
                    await _onMessage(message);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in background consumer for topic {Topic}", _topic);
        }
    }
    #endregion
}
