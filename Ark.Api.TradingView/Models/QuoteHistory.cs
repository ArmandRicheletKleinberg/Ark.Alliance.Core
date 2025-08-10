#nullable enable

namespace Ark.Core.Api.TradingView.Models
{
    /// <summary>
    /// Represents a collection of historical data points.
    /// + Suitable for chart rendering and analysis.
    /// - Volume and order counts may be zero when unavailable.
    /// Ref: <see href="https://www.tradingview.com/support/solutions/43000529350-history/"/>
    /// </summary>
    public sealed class QuoteHistory
    {
        #region Properties

        /// <summary>
        /// Symbol of the history.
        /// + Example: <c>BTCUSDT</c>.
        /// - Case sensitive according to source exchange.
        /// </summary>
        public string Symbol { get; set; } = string.Empty;

        /// <summary>Candle interval as <see cref="TradingViewInterval"/>.</summary>
        public TradingViewInterval Interval { get; set; }

        /// <summary>Data source identifier.</summary>
        public string Source { get; set; } = "TradingView";

        /// <summary>Underlying symbol if applicable.</summary>
        public string? Underlying { get; set; }

        /// <summary>Indicates if the data is for a futures contract.</summary>
        public bool IsFutures { get; set; }

        /// <summary>
        /// Historical points of type <see cref="HistoryPoint"/>.
        /// + Ordered chronologically by <see cref="HistoryPoint.Timestamp"/>.
        /// - May be empty when no data is available for the period.
        /// </summary>
        public List<HistoryPoint> Points { get; set; } = new();

        #endregion Properties
    }
}
