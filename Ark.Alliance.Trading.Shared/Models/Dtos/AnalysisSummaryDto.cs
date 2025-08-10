using System;
using Ark.Alliance.Trading.Shared.Enums;

namespace Ark.Alliance.Trading.Shared.Models.Dtos;

/// <summary>
/// Technical analysis summary returned by TradingView.
/// + Provides high-level market signal for UI and strategy tuning.
/// - Relies on third-party data; confirm before executing trades.
/// Ref: <see href="https://www.tradingview.com/support/solutions/43000534721-technical-analysis-overview/" />
/// </summary>
public class AnalysisSummaryDto
{
    /// <summary>
    /// Ticker symbol.
    /// + Identifies the market pair analysed.
    /// - Not validated against exchange listings.
    /// </summary>
    public string Symbol { get; set; } = string.Empty;

    /// <summary>
    /// Overall recommendation.
    /// + Facilitates unified "Buy"/"Sell" semantics.
    /// - Does not replace full technical analysis.
    /// </summary>
    public AnalysisRecommendation Recommendation { get; set; } = AnalysisRecommendation.Neutral;

    /// <summary>
    /// Timestamp of the analysis.
    /// + Allows sorting and staleness checks.
    /// - Based on client clock of provider.
    /// </summary>
    public DateTime Timestamp { get; set; }
}
