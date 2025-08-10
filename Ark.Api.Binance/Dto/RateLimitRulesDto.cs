namespace Ark.Api.Binance;

/// <summary>
/// Represents rate limit configuration settings.
/// + Defines thresholds used by rate limiters and alerting.
/// - Does not track real-time consumption metrics.
/// </summary>
public class RateLimitRulesDto
{
    /// <summary>Maximum request weight per minute.</summary>
    public int WeightLimit { get; set; }

    /// <summary>Maximum orders per minute.</summary>
    public int OrderLimit { get; set; }

    /// <summary>Usage percentage triggering alerts.</summary>
    public decimal AlertThreshold { get; set; }

    /// <summary>Usage percentage at which normal operation resumes.</summary>
    public decimal RecoveryThreshold { get; set; }
}
