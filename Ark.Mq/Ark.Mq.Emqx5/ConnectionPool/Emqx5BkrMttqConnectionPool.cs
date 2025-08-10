using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Resilience;
using MQTTnet;
using MQTTnet.AspNetCore;
using Ark.Net.Mqtt.Iot.Emqx5.Diagnostics;

namespace Ark.Net.Mqtt.Iot.Emqx5;

/// <summary>
/// Provides a simple MQTT client pool with retry policies.
/// </summary>
public class Emqx5BkrMttqConnectionPool : IDisposable
{
    #region Fields
    private readonly ConcurrentBag<IMqttClient> _clients = new();
    private readonly Emqx5BkrMttqSettings _options;
    private readonly ILogger<Emqx5BkrMttqConnectionPool> _logger;
    private readonly ResiliencePipeline _pipeline;
    private int _createdClients;
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of <see cref="Emqx5BkrMttqConnectionPool"/>.
    /// </summary>
    /// <param name="options">Pool options.</param>
    /// <param name="logger">Logger instance.</param>
    public Emqx5BkrMttqConnectionPool(IOptions<Emqx5BkrMttqSettings> options, ILogger<Emqx5BkrMttqConnectionPool> logger, ResiliencePipelineProvider provider)
    {
        _options = options.Value;
        _logger = logger;
        _pipeline = provider.GetPipeline("emqx5mqtt");
    }
    #endregion

    #region Methods (Public)
    /// <summary>
    /// Acquires a connected client from the pool.
    /// </summary>
    /// <returns>A connected MQTT client.</returns>
    public async Task<IMqttClient> AcquireAsync()
    {
        if (_clients.TryTake(out var client) && client.IsConnected)
            return client;

        if (Interlocked.Increment(ref _createdClients) <= _options.MaxConnections)
            return await CreateClientAsync();

        while (true)
        {
            if (_clients.TryTake(out client) && client.IsConnected)
                return client;
            await Task.Delay(50);
        }
    }

    /// <summary>Returns a client to the pool.</summary>
    public void Release(IMqttClient client)
    {
        if (client.IsConnected)
        {
            _clients.Add(client);
        }
        else
        {
            client.Dispose();
            Emqx5BkrMttqMetrics.ConnectionsClosed.Add(1);
        }
    }
    #endregion

    #region Methods (Private)
    private async Task<IMqttClient> CreateClientAsync()
    {
        var factory = new MqttClientFactory();
        var client = factory.CreateMqttClient();
        var optionsBuilder = new MqttClientOptionsBuilder()
            .WithTcpServer(_options.HostName, _options.Port)
            .WithClientId(_options.ClientId)
            .WithCredentials(_options.UserName, _options.Password)
            .WithCleanSession(_options.CleanSession);
        var mqttOptions = optionsBuilder.Build();
        await _pipeline.ExecuteAsync(async _ =>
        {
            await client.ConnectAsync(mqttOptions, CancellationToken.None);
            return client;
        });
        _logger.Log(_options.LogLevel, "MQTT connection opened to {Host}", _options.HostName);
        Emqx5BkrMttqMetrics.ConnectionsOpened.Add(1);
        return client;
    }

    #endregion

    #region IDisposable
    public void Dispose()
    {
        while (_clients.TryTake(out var client))
        {
            if (client.IsConnected)
            {
                client.DisconnectAsync().GetAwaiter().GetResult();
                Emqx5BkrMttqMetrics.ConnectionsClosed.Add(1);
            }
        }
    }
    #endregion
}
