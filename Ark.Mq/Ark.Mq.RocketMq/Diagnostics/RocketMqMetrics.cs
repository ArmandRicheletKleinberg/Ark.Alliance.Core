using System.Diagnostics.Metrics;

namespace Ark.Net.RocketMq.Diagnostics;

/// <summary>
/// OpenTelemetry metrics for the RocketMQ integration.
/// </summary>
internal static class RocketMqMetrics
{
    /// <summary>Meter name for all RocketMQ metrics.</summary>
    public const string MeterName = "Ark.Mq.RocketMq";

    /// <summary>Meter instance used to create metrics instruments.</summary>
    public static readonly Meter Meter = new(MeterName);

    /// <summary>Counts opened RocketMQ connections.</summary>
    public static readonly Counter<long> ConnectionsOpened = Meter.CreateCounter<long>("rocketmq.connections.opened");

    /// <summary>Counts closed RocketMQ connections.</summary>
    public static readonly Counter<long> ConnectionsClosed = Meter.CreateCounter<long>("rocketmq.connections.closed");

    /// <summary>Counts successfully published messages.</summary>
    public static readonly Counter<long> MessagesPublished = Meter.CreateCounter<long>("rocketmq.messages.published");

    /// <summary>Counts messages that failed to publish.</summary>
    public static readonly Counter<long> MessagesFailed = Meter.CreateCounter<long>("rocketmq.messages.failed");
}
