namespace Ark.Core.Api.TradingView.Models
{
    /// <summary>
    /// Represents a single historical data point.
    /// + Encapsulates OHLCV information for one interval.
    /// - Does not adjust for corporate actions like splits.
    /// Ref: <see href="https://www.tradingview.com/support/"/>
    /// </summary>
    public sealed class HistoryPoint
    {
        #region Properties

        /// <summary>Unix timestamp of the period as <see cref="DateTime"/>.</summary>
        public DateTime Timestamp { get; set; }

        /// <summary>Open price.</summary>
        public decimal Open { get; set; }

        /// <summary>High price.</summary>
        public decimal High { get; set; }

        /// <summary>Low price.</summary>
        public decimal Low { get; set; }

        /// <summary>Close price.</summary>
        public decimal Close { get; set; }

        /// <summary>Trading volume.</summary>
        public decimal Volume { get; set; }

        /// <summary>Number of orders traded during the period.</summary>
        public int OrderCount { get; set; }

        #endregion Properties
    }
}
