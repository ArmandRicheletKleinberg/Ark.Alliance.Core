namespace Ark.Alliance.Trading.Shared.Enums;

/// <summary>
/// Overall technical recommendation from TradingView.
/// + Enables consistent Buy/Sell indicators across backend and frontend.
/// - Represents external analysis; verify before trading.
/// Ref: <see href="https://www.tradingview.com/support/solutions/43000534721-technical-analysis-overview/" />
/// </summary>
public enum AnalysisRecommendation
{
    /// <summary>Indicators strongly advise selling.</summary>
    StrongSell,

    /// <summary>Most indicators suggest a sell.</summary>
    Sell,

    /// <summary>Indicators are inconclusive.</summary>
    Neutral,

    /// <summary>Most indicators suggest buying.</summary>
    Buy,

    /// <summary>Indicators strongly advise buying.</summary>
    StrongBuy
}
