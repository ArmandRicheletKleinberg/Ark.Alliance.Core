using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetMQ.Sockets;
using Ark;

namespace Ark.Net.ZeroMq;

/// <summary>
/// Basic queue consumer using an action callback.
/// </summary>
public class ZeroMqConsumer
{
    #region Fields
    private readonly ZeroMqSettings _options;
    private readonly ILogger<ZeroMqConsumer> _logger;
    #endregion

    #region Constructors
    public ZeroMqConsumer(IOptions<ZeroMqSettings> options, ILogger<ZeroMqConsumer> logger)
    {
        _options = options.Value;
        _logger = logger;
    }
    #endregion

    /// <summary>
    /// Consumes messages from a topic.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message.</typeparam>
    /// <param name="topic">The topic name.</param>
    /// <param name="onMessage">Callback executed for each message.</param>
    /// <param name="token">Cancellation token.</param>
    /// <returns>
    /// Success : The execution has succeeded.
    /// Unexpected : An unexpected error occurs.
    /// </returns>
    #region Methods (Public)
    public Task<Result> ConsumeAsync<TMessage>(string topic, Func<TMessage, Task> onMessage, CancellationToken token = default) where TMessage : class
        => ConsumeAsync(topic, ctx => onMessage(ctx.Payload), token);

    public async Task<Result> ConsumeAsync<TMessage>(string topic, Func<MessageContext<TMessage>, Task> onMessage, CancellationToken token = default) where TMessage : class
    {
        try
        {
            using var sub = new SubscriberSocket();
            sub.Connect(_options.Endpoint);
            sub.Subscribe(topic);
            _logger.LogInformation("Consuming ZeroMQ topic {Topic}", topic);

            while (!token.IsCancellationRequested)
            {
                var msg = await Task.Run(sub.ReceiveFrameString, token);
                var message = JsonSerializer.Deserialize<TMessage>(msg);
                if (message != null)
                    await onMessage(new MessageContext<TMessage>(message));
            }

            return Result.Success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while consuming ZeroMQ topic {Topic}", topic);
            return new Result(ex);
        }
    }
    #endregion
}
