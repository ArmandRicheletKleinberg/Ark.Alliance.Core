using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Ark;
using Ark.Cqrs.Messaging.Abstractions;
using NetMQ;
using System.Diagnostics;
using Ark.Net.ZeroMq.Diagnostics;

namespace Ark.Net.ZeroMq;

/// <summary>
/// Simple message publisher.
/// </summary>
public class ZeroMqPublisher : IBrokerProducer
{
    #region Fields
    private readonly ZeroMqConnectionPool _pool;
    private readonly ILogger<ZeroMqPublisher> _logger;
    private static readonly ActivitySource ActivitySource = new("Ark.Mq.ZeroMq.Publisher");
    #endregion

    #region Constructors
    public ZeroMqPublisher(ZeroMqConnectionPool pool, ILogger<ZeroMqPublisher> logger)
    {
        _pool = pool;
        _logger = logger;
    }

    public Task PublishAsync<T>(T msg, BrokerMetadata meta, CancellationToken token = default)
        => PublishAsync(meta.Topic, meta.Topic, msg!, token);
    #endregion

    #region Methods (Public)
    /// <summary>
    /// Publishes a message to the configured endpoint.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message.</typeparam>
    /// <param name="exchange">Unused for ZeroMQ.</param>
    /// <param name="routingKey">Unused for ZeroMQ.</param>
    /// <param name="message">The message instance.</param>
    /// <param name="token">Cancellation token.</param>
    /// <returns>
    /// Success : The execution has succeeded.
    /// Unexpected : An unexpected error occurs.
    /// </returns>
    public async Task<Result> PublishAsync<TMessage>(string exchange, string routingKey, TMessage message, CancellationToken token = default) where TMessage : class
    {
        var socketResult = await _pool.AcquireAsync();
        if (socketResult.IsNotSuccess)
            return new Result(socketResult);

        var socket = socketResult.Data;
        try
        {
            using var activity = ActivitySource.StartActivity("zeromq.publish", ActivityKind.Producer);
            socket.SendFrame(JsonSerializer.Serialize(message));
            _logger.LogInformation("Published ZeroMQ message");
            ZeroMqMetrics.MessagesPublished.Add(1);
            return Result.Success;
        }
        catch (Exception ex)
        {
            ZeroMqMetrics.MessagesFailed.Add(1);
            _logger.LogError(ex, "Publishing message failed");
            return new Result(ex);
        }
        finally
        {
            _pool.Release(socket);
        }
    }

    /// <summary>
    /// Publishes a message wrapped in a <see cref="MessageContext{TPayload}"/>.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message.</typeparam>
    /// <param name="exchange">Unused for ZeroMQ.</param>
    /// <param name="routingKey">Unused for ZeroMQ.</param>
    /// <param name="context">Message context including headers.</param>
    /// <param name="token">Cancellation token.</param>
    /// <returns>A <see cref="Result"/> describing the outcome.</returns>
    public Task<Result> PublishAsync<TMessage>(string exchange, string routingKey, MessageContext<TMessage> context, CancellationToken token = default) where TMessage : class
        => PublishAsync(exchange, routingKey, context.Payload, token);
    #endregion
}
