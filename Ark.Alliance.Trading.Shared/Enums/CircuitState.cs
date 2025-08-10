namespace Ark.Alliance.Trading.Shared.Enums;

/// <summary>
/// Represents the state of a circuit breaker.
/// + Mirrors <c>Ark.Api.Binance.Helpers.CircuitState</c>.
/// + Used by clients to display circuit breaker status.
/// - Aggregates all endpoints; individual circuit details are unavailable.
/// </summary>
public enum CircuitState
{
    /// <summary>Normal operation - requests allowed.</summary>
    Closed,

    /// <summary>Failure threshold exceeded - requests blocked.</summary>
    Open,

    /// <summary>Testing recovery - limited requests allowed.</summary>
    HalfOpen
}
