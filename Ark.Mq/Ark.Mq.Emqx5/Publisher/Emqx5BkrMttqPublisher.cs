using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MQTTnet;
using Ark;
using System.Diagnostics;
using Ark.Net.Mqtt.Iot.Emqx5.Diagnostics;

namespace Ark.Net.Mqtt.Iot.Emqx5;

/// <summary>
/// Simple MQTT message publisher.
/// </summary>
/// <summary>
/// Simple MQTT message publisher for the EMQX5 broker.
/// </summary>
using Ark.Cqrs.Messaging.Abstractions;

public class Emqx5BkrMttqPublisher : IBrokerProducer
{
    #region Fields
    private readonly Emqx5BkrMttqConnectionPool _pool;
    private readonly ILogger<Emqx5BkrMttqPublisher> _logger;
    private static readonly ActivitySource ActivitySource = new("Ark.Mq.Emqx5.Publisher");
    #endregion

    /// <summary>
    /// Initializes a new instance of <see cref="Emqx5BkrMttqPublisher"/>.
    /// </summary>
    /// <param name="pool">Pool used to acquire clients.</param>
    /// <param name="logger">Logger instance.</param>
    #region Constructors
    public Emqx5BkrMttqPublisher(Emqx5BkrMttqConnectionPool pool, ILogger<Emqx5BkrMttqPublisher> logger)
    {
        _pool = pool;
        _logger = logger;
    }
    #endregion

    /// <summary>
    /// Publishes a message to the specified topic.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message.</typeparam>
    /// <param name="topic">Target topic.</param>
    /// <param name="message">Message payload.</param>
    /// <param name="token">Optional cancellation token.</param>
    /// <returns>
    /// Success : The execution has succeeded.
    /// Unexpected : An unexpected error occurs.
    /// </returns>
    #region Methods (Public)
    public Task PublishAsync<T>(T msg, BrokerMetadata meta, CancellationToken token = default)
        => PublishAsync(meta.Topic, msg!, token);

    public async Task<Result> PublishAsync<TMessage>(string topic, TMessage message, CancellationToken token = default) where TMessage : class
    {
        var client = await _pool.AcquireAsync();
        try
        {
            using var activity = ActivitySource.StartActivity("emqx5.publish", ActivityKind.Producer);
            var payload = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
            var msg = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(payload)
                .Build();
            await client.PublishAsync(msg, token);
            _logger.LogInformation("Published message to {Topic}", topic);
            Emqx5BkrMttqMetrics.MessagesPublished.Add(1);
            return Result.Success;
        }
        catch (Exception ex)
        {
            Emqx5BkrMttqMetrics.MessagesFailed.Add(1);
            _logger.LogError(ex, "Publishing message failed");
            return new Result(ex);
        }
        finally
        {
            _pool.Release(client);
        }
    }

    /// <summary>
    /// Publishes a message wrapped in a <see cref="MessageContext{TPayload}"/>.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message.</typeparam>
    /// <param name="topic">Target topic.</param>
    /// <param name="context">Message context including headers.</param>
    /// <param name="token">Cancellation token.</param>
    /// <returns>A <see cref="Result"/> describing the outcome.</returns>
    public Task<Result> PublishAsync<TMessage>(string topic, MessageContext<TMessage> context, CancellationToken token = default) where TMessage : class
        => PublishAsync(topic, context.Payload, token);
    #endregion
}
