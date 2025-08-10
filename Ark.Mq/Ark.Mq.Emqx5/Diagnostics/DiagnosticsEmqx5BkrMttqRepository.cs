using System.Threading.Tasks;
using Ark;
using Microsoft.Extensions.Logging;
using MQTTnet;

namespace Ark.Net.Mqtt.Iot.Emqx5;

/// <summary>
/// Repository used to perform diagnostics operations on MQTT.
/// </summary>
public class DiagnosticsEmqx5BkrMttqRepository : Emqx5BkrMttqRepositoryBase
{
    #region Constructors
    /// <summary>
    /// Initializes a new instance of <see cref="DiagnosticsEmqx5BkrMttqRepository"/>.
    /// </summary>
    /// <param name="pool">Connection pool.</param>
    /// <param name="logger">Logger instance.</param>
    public DiagnosticsEmqx5BkrMttqRepository(Emqx5BkrMttqConnectionPool pool, ILogger<DiagnosticsEmqx5BkrMttqRepository> logger)
        : base(pool, logger)
    {
    }
    #endregion

    /// <summary>
    /// Checks that a topic is reachable by publishing and waiting for acknowledgment.
    /// </summary>
    /// <summary>
    /// Checks that a topic is reachable by publishing and unsubscribing.
    /// </summary>
    /// <param name="topic">Topic to check.</param>
    /// <returns>
    /// Success : The execution has succeeded.
    /// Unexpected : An unexpected error occurs.
    /// </returns>
    #region Methods (Public)
    public Task<Result> PingAsync(string topic)
        => Execute(async client =>
        {
            var subscribeOptions = new MqttClientSubscribeOptionsBuilder()
                .WithTopicFilter(topic)
                .Build();
            await client.SubscribeAsync(subscribeOptions);
            var unsubscribeOptions = new MqttClientUnsubscribeOptionsBuilder()
                .WithTopicFilter(topic)
                .Build();
            await client.UnsubscribeAsync(unsubscribeOptions);
            return Result.Success;
        });
    #endregion
}
