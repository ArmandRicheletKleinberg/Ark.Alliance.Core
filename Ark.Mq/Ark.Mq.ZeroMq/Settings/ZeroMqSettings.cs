using Microsoft.Extensions.Logging;

namespace Ark.Net.ZeroMq;

/// <summary>
/// Settings for ZeroMQ sockets.
/// </summary>
public class ZeroMqSettings
{
    /// <summary>
    /// Name of the connection pool these settings apply to.
    /// </summary>
    public string? ConnectionPoolName { get; set; }
    /// <summary>Endpoint used for socket connections.</summary>
    public string Endpoint { get; set; } = "tcp://localhost:5555";

    /// <summary>Minimum log level for diagnostics.</summary>
    public LogLevel LogLevel { get; set; } = LogLevel.Information;

    /// <summary>Maximum number of concurrent connections to keep in the pool.</summary>
    public int MaxConnections { get; set; } = 5;

    /// <summary>Number of retries when establishing a connection.</summary>
    public int RetryCount { get; set; } = 3;
}
