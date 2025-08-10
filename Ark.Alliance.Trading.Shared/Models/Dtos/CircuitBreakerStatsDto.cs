using Ark.Alliance.Trading.Shared.Enums;

namespace Ark.Alliance.Trading.Shared.Models.Dtos;

/// <summary>
/// Circuit breaker statistics exposed by the backend.
/// + Provides insight into failure counts and state transitions.
/// - Values may be stale between polls.
/// Ref: <see href="https://learn.microsoft.com/dotnet/core/extensions/circuit-breaker" />
/// </summary>
public class CircuitBreakerStatsDto
{
    /// <summary>
    /// Current state of the circuit.
    /// + Indicates whether requests are permitted.
    /// - Global value; not per-endpoint.
    /// </summary>
    public CircuitState State { get; set; }

    /// <summary>
    /// Number of failures recorded in the current window.
    /// + Helps diagnose instability.
    /// - Counter resets after state transitions.
    /// </summary>
    public int FailureCount { get; set; }

    /// <summary>
    /// Threshold of failures required to open the circuit.
    /// + Mirrors backend configuration.
    /// - Changing runtime values may desync.
    /// </summary>
    public int FailureThreshold { get; set; }

    /// <summary>
    /// Last time an operation failed.
    /// + Useful for troubleshooting.
    /// - Client clock differences may mislead.
    /// </summary>
    public DateTime LastFailureTime { get; set; }

    /// <summary>
    /// Next time the circuit will attempt to close.
    /// + Allows frontends to show countdowns.
    /// - Only approximate; backend may attempt sooner.
    /// </summary>
    public DateTime NextAttemptTime { get; set; }

    /// <summary>
    /// Duration the circuit remains open before retrying.
    /// + Communicates resilience policy.
    /// - Actual retry timing may vary.
    /// </summary>
    public TimeSpan RecoveryTimeout { get; set; }
}
