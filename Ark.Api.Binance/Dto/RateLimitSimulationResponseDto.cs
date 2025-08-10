namespace Ark.Api.Binance;

/// <summary>
/// Result of a rate limit simulation.
/// + Summarises weight usage and order rate checks.
/// - Provides estimates only; real execution may differ.
/// </summary>
public class RateLimitSimulationResponseDto
{
    /// <summary>Total weight consumed by the batch.</summary>
    public int TotalWeight { get; set; }

    /// <summary>True if the minute weight limit is exceeded.</summary>
    public bool WeightLimitExceeded { get; set; }

    /// <summary>True if any 60s window exceeds the weight limit.</summary>
    public bool BurstViolation { get; set; }

    /// <summary>Message describing order rate safety or violation.</summary>
    public string OrderRateMessage { get; set; } = string.Empty;

    /// <summary>Suggested delay in milliseconds between orders.</summary>
    public double SuggestedDelayMs { get; set; }
}
