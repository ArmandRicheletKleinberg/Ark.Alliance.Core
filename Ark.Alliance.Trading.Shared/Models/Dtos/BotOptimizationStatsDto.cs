using System;

namespace Ark.Alliance.Trading.Shared.Models.Dtos;

/// <summary>
/// Summary statistics for bot optimisation metrics.
/// + Aggregates performance indicators for dashboards.
/// - Snapshot values; historical trend requires external storage.
/// Ref: <see href="https://en.wikipedia.org/wiki/Sharpe_ratio" />
/// </summary>
public class BotOptimizationStatsDto
{
    /// <summary>
    /// Peak-to-trough equity decline.
    /// + Highlights worst recent performance.
    /// - Does not include open positions.
    /// </summary>
    public decimal CurrentDrawdown { get; set; }

    /// <summary>
    /// Ratio of winning trades to total trades.
    /// + Quick gauge of strategy success.
    /// - Ignores profit magnitude.
    /// </summary>
    public double WinRate { get; set; }

    /// <summary>
    /// Number of trades executed in the analysed period.
    /// + Useful for throughput analysis.
    /// - High counts may hide overtrading.
    /// </summary>
    public int TradesExecuted { get; set; }

    /// <summary>
    /// Timestamp of the last statistics update.
    /// + Allows dashboards to show data freshness.
    /// - Depends on server clock synchronisation.
    /// </summary>
    public DateTime LastUpdated { get; set; }
}
