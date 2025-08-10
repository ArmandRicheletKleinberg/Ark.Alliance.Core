using System.Diagnostics.Metrics;

namespace Ark.Net.RabbitMq.Diagnostics;

/// <summary>
/// Exposes OpenTelemetry metrics related to RabbitMQ connections.
/// </summary>
internal static class RabbitMqMetrics
{
    /// <summary>Meter name for all RabbitMQ metrics.</summary>
    public const string MeterName = "Ark.Mq.RabbitMq";

    /// <summary>Meter instance used to create metrics instruments.</summary>
    public static readonly Meter Meter = new(MeterName);

    /// <summary>Counts opened RabbitMQ connections.</summary>
    public static readonly Counter<long> ConnectionsOpened = Meter.CreateCounter<long>("rabbitmq.connections.opened");

    /// <summary>Counts closed RabbitMQ connections.</summary>
    public static readonly Counter<long> ConnectionsClosed = Meter.CreateCounter<long>("rabbitmq.connections.closed");

    /// <summary>Counts successfully published messages.</summary>
    public static readonly Counter<long> MessagesPublished = Meter.CreateCounter<long>("rabbitmq.messages.published");

    /// <summary>Counts messages that failed to publish.</summary>
    public static readonly Counter<long> MessagesFailed = Meter.CreateCounter<long>("rabbitmq.messages.failed");
}
