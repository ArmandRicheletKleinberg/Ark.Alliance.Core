using System.Diagnostics.Metrics;

namespace Ark.Net.ZeroMq.Diagnostics;

/// <summary>
/// OpenTelemetry metrics for the ZeroMQ integration.
/// </summary>
internal static class ZeroMqMetrics
{
    public const string MeterName = "Ark.Mq.ZeroMq";
    public static readonly Meter Meter = new(MeterName);
    public static readonly Counter<long> ConnectionsOpened = Meter.CreateCounter<long>("zeromq.connections.opened");
    public static readonly Counter<long> ConnectionsClosed = Meter.CreateCounter<long>("zeromq.connections.closed");
    public static readonly Counter<long> MessagesPublished = Meter.CreateCounter<long>("zeromq.messages.published");
    public static readonly Counter<long> MessagesFailed = Meter.CreateCounter<long>("zeromq.messages.failed");
}
