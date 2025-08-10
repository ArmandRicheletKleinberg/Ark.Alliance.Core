using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Resilience;
using MQTTnet;
using MQTTnet.Adapter;


namespace Ark.Net.Mqtt.Iot.Emqx5;

/// <summary>
/// Provides a singleton MQTT client connection.
/// </summary>
public class Emqx5BkrMttqConnection : IDisposable
{
    #region Fields
    private readonly ILogger<Emqx5BkrMttqConnection> _logger;
    private readonly Emqx5BkrMttqSettings _options;
    private readonly ResiliencePipeline _pipeline;
    private IMqttClient _client;
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of <see cref="Emqx5BkrMttqConnection"/>.
    /// </summary>
    /// <param name="options">Connection options.</param>
    /// <param name="logger">Logger instance.</param>
    public Emqx5BkrMttqConnection(IOptions<Emqx5BkrMttqSettings> options, ILogger<Emqx5BkrMttqConnection> logger, ResiliencePipelineProvider provider)
    {
        _options = options.Value;
        _logger = logger;
        _pipeline = provider.GetPipeline("emqx5mqtt");
    }
    #endregion

    #region Methods (Public)
    /// <summary>
    /// Gets a connected MQTT client instance.
    /// </summary>
    /// <returns>The connected <see cref="IMqttClient"/>.</returns>
    public async Task<IMqttClient> GetClientAsync()
    {
        if (_client?.IsConnected == true)
            return _client;

       var factory = new MqttClientFactory(); 
        _client = factory.CreateMqttClient();
        var optionsBuilder = new MqttClientOptionsBuilder()
            .WithTcpServer(_options.HostName, _options.Port)
            .WithClientId(_options.ClientId)
            .WithCredentials(_options.UserName, _options.Password)
            .WithCleanSession(_options.CleanSession);
        var mqttOptions = optionsBuilder.Build();
        await _pipeline.ExecuteAsync(async _ =>
        {
            await _client.ConnectAsync(mqttOptions, CancellationToken.None);
            return _client;
        });
        _logger.Log(_options.LogLevel, "MQTT connection opened to {Host}", _options.HostName);
        return _client;
    }

    #endregion

    #region IDisposable
    public void Dispose()
    {
        if (_client?.IsConnected == true)
        {
            _client.DisconnectAsync().GetAwaiter().GetResult();
            _logger.Log(_options.LogLevel, "MQTT connection closed");
        }
    }
    #endregion
}
