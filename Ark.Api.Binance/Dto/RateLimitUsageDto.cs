namespace Ark.Api.Binance;

/// <summary>
/// Represents rate limiter usage information.
/// + Exposes current usage ratio and thresholds.
/// - Does not persist historical consumption.
/// </summary>
public class RateLimitUsageDto
{
    /// <summary>
    /// Ratio of consumed requests (0 to 1).
    /// </summary>
    public double Usage { get; set; }

    /// <summary>
    /// Indicates whether the alert threshold has been exceeded.
    /// </summary>
    public bool Approaching { get; set; }

    /// <summary>
    /// Usage percentage that triggers alert signalling.
    /// </summary>
    public decimal AlertThreshold { get; set; }

    /// <summary>
    /// Usage percentage required before normal operation resumes.
    /// </summary>
    public decimal RecoveryThreshold { get; set; }
}
