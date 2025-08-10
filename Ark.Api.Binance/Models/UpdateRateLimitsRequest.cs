namespace Ark.Api.Binance;

/// <summary>
/// Request payload to adjust rate limit settings.
/// + Enables dynamic throttling based on expected load.
/// - Incorrect values may violate exchange constraints.
/// </summary>
public class UpdateRateLimitsRequest
{
    /// <summary>Maximum allowed weight per minute.</summary>
    public int WeightLimit { get; set; }

    /// <summary>Maximum allowed orders per minute.</summary>
    public int OrderLimit { get; set; }

    /// <summary>Threshold to trigger alerts.</summary>
    public decimal AlertThreshold { get; set; }

    /// <summary>Threshold under which usage is considered recovered.</summary>
    public decimal RecoveryThreshold { get; set; }
}
