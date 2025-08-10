using Microsoft.Extensions.Logging;

namespace Ark.Net.RocketMq;

/// <summary>
/// Settings for the RocketMQ producers.
/// </summary>
public class RocketMqSettings
{
    /// <summary>
    /// Name of the connection pool these settings apply to.
    /// </summary>
    public string? ConnectionPoolName { get; set; }
    /// <summary>Name server address.</summary>
    public string NameServerAddress { get; set; } = "localhost:9876";

    /// <summary>Producer group.</summary>
    public string ProducerGroup { get; set; } = "default";

    /// <summary>Default topic name.</summary>
    public string TopicName { get; set; } = string.Empty;

    /// <summary>Minimum log level for diagnostics.</summary>
    public LogLevel LogLevel { get; set; } = LogLevel.Information;

    /// <summary>Maximum number of concurrent connections to keep in the pool.</summary>
    public int MaxConnections { get; set; } = 5;

    /// <summary>Number of retries when establishing a connection.</summary>
    public int RetryCount { get; set; } = 3;
}
