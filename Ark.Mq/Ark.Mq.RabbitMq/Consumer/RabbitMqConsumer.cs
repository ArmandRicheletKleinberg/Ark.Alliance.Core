using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Ark;

namespace Ark.Mq.RabbitMq;

/// <summary>
/// Basic queue consumer using an action callback.
/// </summary>
public class RabbitMqConsumer
{
    private readonly RabbitMqChannelPool _pool;
    private readonly ILogger<RabbitMqConsumer> _logger;
    private readonly RabbitMqSettings _settings;
    private static readonly ActivitySource ActivitySource = new("Ark.Mq.RabbitMq.Consumer");


    public RabbitMqConsumer(RabbitMqChannelPool pool, ILogger<RabbitMqConsumer> logger, IOptions<RabbitMqSettings> options)

    {
        _pool = pool;
        _logger = logger;
        _settings = options.Value;
    }

    /// <summary>
    /// Consumes messages from a queue.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message.</typeparam>
    /// <param name="queue">The queue name.</param>
    /// <param name="onMessage">Callback executed for each message.</param>
    /// <param name="token">Cancellation token.</param>
    /// <returns>
    ///     <see cref="Result.Success"/> when the consumer has started.
    /// </returns>
    public Task<Result> ConsumeAsync<TMessage>(string queue, Func<TMessage, Task> onMessage, CancellationToken token = default) where TMessage : class
        => ConsumeAsync(queue, ctx => onMessage(ctx.Payload), token);

    /// <summary>
    /// Consumes messages from a queue and provides the message context.
    /// <code>
    /// await consumer.ConsumeAsync("orders", ctx => Handle(ctx));
    /// </code>
    /// </summary>
    /// <typeparam name="TMessage">Type of the message.</typeparam>
    /// <param name="queue">Target queue to consume.</param>
    /// <param name="onMessage">Callback invoked with each message context.</param>
    /// <param name="token">Cancellation token to stop consumption.</param>
    /// <returns>A <see cref="Result"/> indicating the subscription status.</returns>
    public async Task<Result> ConsumeAsync<TMessage>(string queue, Func<MessageContext<TMessage>, Task> onMessage, CancellationToken token = default) where TMessage : class
    {
        await using var lease = await _pool.AcquireAsync();
        var channel = lease.Channel;
        channel.BasicQos(0, _settings.Prefetch, false);

        try
        {

            CreateConsumer(channel, queue, onMessage);

            _logger.LogInformation("Consuming queue {Queue}", queue);

            token.Register(() => CloseLease(channel, lease));
            return Result.Success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while consuming queue {Queue}", queue);
            channel.Close();
            await lease.DisposeAsync();
            return new Result(ex);
        }
    }

    /// <summary>

    /// Registers a basic consumer on the specified channel.
    /// </summary>
    private void CreateConsumer<TMessage>(IModel channel, string queue, Func<MessageContext<TMessage>, Task> onMessage) where TMessage : class
    {
        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.Received += async (_, ea) => await ProcessDelivery(channel, ea, onMessage, queue);
        channel.BasicConsume(queue, false, consumer);
    }

    /// Handles a single message delivery event.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message.</typeparam>
    /// <param name="ea">Event arguments containing the raw message.</param>
    /// <param name="channel">Channel used to send acknowledgements.</param>
    /// <param name="onMessage">Callback invoked with the deserialized message context.</param>
    /// <param name="queue">Name of the consumed queue.</param>
    private async Task HandleReceived<TMessage>(BasicDeliverEventArgs ea, IModel channel, Func<MessageContext<TMessage>, Task> onMessage, string queue) where TMessage : class

    {
        using var activity = ActivitySource.StartActivity("rabbitmq.consume", ActivityKind.Consumer);
        var body = ea.Body.ToArray();
        if (_settings.MaxMessageSizeKb > 0 && body.Length > _settings.MaxMessageSizeKb * 1024)
            _logger.LogWarning("Received large message of {Size} bytes from {Queue}", body.Length, queue);


        if (message is null)
            return;

        var ctx = new MessageContext<TMessage>(message, ea.BasicProperties.Headers, ea.BasicProperties.CorrelationId, ea.BasicProperties.MessageId);
        var result = await Result.SafeExecute(async () =>
        {
            await handler(ctx);
            return Result.Success;
        }, ex => _logger.LogError(ex, "Error handling message"));

        if (result.IsSuccess)
            channel.BasicAck(ea.DeliveryTag, false);
        else
            channel.BasicNack(ea.DeliveryTag, false, true);
    }

    /// <summary>Closes the underlying channel and disposes the lease.</summary>
    private static void CloseLease(IModel channel, RabbitMqChannelLease lease)
    {
        channel.Close();
        lease.DisposeAsync().AsTask().Wait();

        if (message == null)
            return;

        try
        {
            var ctx = new MessageContext<TMessage>(
                message,
                ea.BasicProperties.Headers,
                ea.BasicProperties.CorrelationId,
                ea.BasicProperties.MessageId);
            await onMessage(ctx);
            channel.BasicAck(ea.DeliveryTag, false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling message");
            channel.BasicNack(ea.DeliveryTag, false, true);
        }

    }
}
