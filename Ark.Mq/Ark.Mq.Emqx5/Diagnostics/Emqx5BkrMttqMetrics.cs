using System.Diagnostics.Metrics;

namespace Ark.Net.Mqtt.Iot.Emqx5.Diagnostics;

/// <summary>
/// OpenTelemetry metrics for the Emqx5 MQTT integration.
/// </summary>
internal static class Emqx5BkrMttqMetrics
{
    /// <summary>Meter name for all Emqx5 metrics.</summary>
    public const string MeterName = "Ark.Mq.Emqx5";

    /// <summary>Meter instance used to create metrics instruments.</summary>
    public static readonly Meter Meter = new(MeterName);

    /// <summary>Counts opened MQTT connections.</summary>
    public static readonly Counter<long> ConnectionsOpened = Meter.CreateCounter<long>("emqx5.connections.opened");

    /// <summary>Counts closed MQTT connections.</summary>
    public static readonly Counter<long> ConnectionsClosed = Meter.CreateCounter<long>("emqx5.connections.closed");

    /// <summary>Counts successfully published messages.</summary>
    public static readonly Counter<long> MessagesPublished = Meter.CreateCounter<long>("emqx5.messages.published");

    /// <summary>Counts messages that failed to publish.</summary>
    public static readonly Counter<long> MessagesFailed = Meter.CreateCounter<long>("emqx5.messages.failed");
}
