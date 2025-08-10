namespace Ark.Net.RabbitMq;

/// <summary>
/// Encapsulates a message payload along with optional headers
/// and correlation identifiers.
/// </summary>
/// <typeparam name="TPayload">Type of the message payload.</typeparam>
/// <example>
/// <code>
/// var ctx = new MessageContext<MyPayload>(new MyPayload());
/// </code>
/// </example>
public sealed record MessageContext<TPayload>(
    TPayload Payload,
    IDictionary<string, object>? Headers = null,
    string? CorrelationId = null,
    string? MessageId = null) where TPayload : class
{
    /// <summary>Identifier used to correlate related messages.</summary>
    public string CorrelationId { get; init; } = CorrelationId ?? Guid.NewGuid().ToString();

    /// <summary>Unique identifier for this message.</summary>
    public string MessageId { get; init; } = MessageId ?? Guid.NewGuid().ToString();
}
