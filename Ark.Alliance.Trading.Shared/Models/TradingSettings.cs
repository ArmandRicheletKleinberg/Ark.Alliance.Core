namespace Ark.Alliance.Trading.Shared.Models;

/// <summary>
/// Application settings controlling trading rules.
/// + Centralizes configurable parameters for the bot.
/// - Default values are indicative and may not suit production.
/// Ref: <see href="https://binance-docs.github.io/apidocs/futures/en/" />
/// </summary>
public class TradingSettings
{
    /// <summary>
    /// Trading symbol traded by the bot.
    /// + Defaults to BTCUSDT for quick testing.
    /// - Must be a valid Binance Futures symbol.
    /// Ref: <see href="https://binance-docs.github.io/apidocs/futures/en/#public-endpoints-info" />
    /// </summary>
    public string Symbol { get; set; } = "BTCUSDT";
    /// <summary>
    /// Initial protective order distance (R2).
    /// + Sets the first stop-loss distance.
    /// - Excessive values reduce profitability.
    /// </summary>
    public decimal InitialProtectionPct { get; set; } = 0.05m;
    /// <summary>
    /// Percentage part of minimal net gain (R1).
    /// + Ensures trades meet yield expectations.
    /// - Ignores absolute fees; see <see cref="MinNetGainAbsolute"/>.
    /// </summary>
    public decimal MinNetYieldPct { get; set; } = 0.01m;
    /// <summary>
    /// Absolute part of minimal net gain in USDT (R1).
    /// + Protects against tiny profit trades.
    /// - Assumes quote asset is USDT.
    /// </summary>
    public decimal MinNetGainAbsolute { get; set; } = 1m;
    /// <summary>
    /// Estimated fees for round trip.
    /// + Influences net gain calculation.
    /// - Actual exchange fees may vary.
    /// </summary>
    public decimal FeeRate { get; set; } = 0.0004m;
    /// <summary>
    /// Estimated funding rate.
    /// + Accounts for periodic funding payments.
    /// - Only an estimate; check live rate.
    /// </summary>
    public decimal FundingRateEst { get; set; } = 0.0001m;
    /// <summary>
    /// Leverage used when opening the initial position.
    /// + Higher leverage reduces capital requirement.
    /// - Increases liquidation risk.
    /// </summary>
    public int InitialLeverage { get; set; } = 20;
    /// <summary>
    /// Transfer threshold for profits (R5).
    /// + Automates profit sweeping.
    /// - Frequent transfers may incur fees.
    /// </summary>
    public decimal TransferThresholdPct { get; set; } = 0.05m;
    /// <summary>
    /// Timeout before cancelling partial fills (R6).
    /// + Prevents hanging orders.
    /// - Too low may cancel valid fills.
    /// </summary>
    public int PartialFillTimeoutSec { get; set; } = 3;
    /// <summary>
    /// Safety margin applied when sizing positions.
    /// + Provides buffer against rapid moves.
    /// - Reduces potential profit.
    /// </summary>
    public decimal SafetyMarginFactor { get; set; } = 0.8m;
    /// <summary>
    /// Pause trading when rate limit usage exceeds this ratio.
    /// + Avoids hitting Binance hard limits.
    /// - May delay trades during spikes.
    /// </summary>
    public decimal RateLimitThresholdPct { get; set; } = 0.7m;
    /// <summary>
    /// Resume trading when usage falls below this ratio.
    /// + Restores activity once safe.
    /// - Requires accurate limiter stats.
    /// </summary>
    public decimal RateLimitRecoveryPct { get; set; } = 0.3m;
    /// <summary>
    /// Time window in seconds to compute the average spread for entry.
    /// + Smooths spread fluctuations.
    /// - Longer windows reduce reactivity.
    /// </summary>
    public int AvgSpreadWindowSec { get; set; } = 30;
    /// <summary>
    /// Critical latency threshold in milliseconds that may trigger emergency liquidation.
    /// </summary>
    /// <remarks>
    /// + Increase to tolerate slower networks.
    /// - Lower values can cause premature liquidation.
    /// Reference: <see cref="Ark.Api.Binance.Services.LatencyOptions.CriticalLatencyThresholdMs"/>.
    /// </remarks>
    public decimal MaxLatencyThresholdMs { get; set; } = 3000m;
}
