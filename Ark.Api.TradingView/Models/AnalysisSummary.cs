namespace Ark.Core.Api.TradingView.Models;

/// <summary>
/// Lightweight technical analysis summary from TradingView.
/// + Aggregates multiple indicators into a single recommendation.
/// - Availability depends on TradingView data coverage.
/// Ref: <see href="https://www.tradingview.com/support/"/>
/// </summary>
public sealed class AnalysisSummary
{

    #region Properties

    /// <summary>Symbol identifier.</summary>
    public string Symbol { get; set; } = string.Empty;

    /// <summary>
    /// Recommendation text, e.g. <c>BUY</c>.
    /// </summary>
    public string Recommendation { get; set; } = string.Empty;

    /// <summary>Time of the analysis as <see cref="DateTime"/>.</summary>
    public DateTime Timestamp { get; set; }

    #endregion Properties
}
