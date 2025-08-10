using System;
using Microsoft.Extensions.Logging;

namespace Ark.Net.Mqtt.Iot.Emqx5;

/// <summary>
/// Settings for MQTT connections.
/// </summary>
public class Emqx5BkrMttqSettings
{
    #region Properties
    /// <summary>
    /// Name of the connection pool these settings apply to.
    /// </summary>
    public string? ConnectionPoolName { get; set; }
    /// <summary>MQTT broker host name.</summary>
    public string HostName { get; set; } = "localhost";

    /// <summary>Port of the broker.</summary>
    public int Port { get; set; } = 1883;

    /// <summary>Client identifier used when connecting.</summary>
    public string ClientId { get; set; } = Guid.NewGuid().ToString();

    /// <summary>Whether to start a clean session.</summary>
    public bool CleanSession { get; set; } = true;

    /// <summary>User name for the connection.</summary>
    public string UserName { get; set; } = "guest";

    /// <summary>Password for the connection.</summary>
    public string Password { get; set; } = "guest";

    /// <summary>Default topic to publish/subscribe.</summary>
    public string Topic { get; set; } = string.Empty;

    /// <summary>Minimum log level for diagnostics.</summary>
    public LogLevel LogLevel { get; set; } = LogLevel.Information;

    /// <summary>Maximum number of concurrent connections to keep in the pool.</summary>
    public int MaxConnections { get; set; } = 5;

    /// <summary>Number of retries when establishing a connection.</summary>
    public int RetryCount { get; set; } = 3;
    #endregion
}
