#nullable enable

namespace Ark.Core.Api.TradingView.Models
{
    /// <summary>
    /// Represents a real time quote value from TradingView.
    /// + Captures last traded price, volume and metadata.
    /// - Lacks order book depth and trade history details.
    /// Ref: <see href="https://www.tradingview.com/support/solutions/43000529354-real-time-data/"/>
    /// </summary>
    public sealed class Quote
    {
        #region Properties

        /// <summary>Last traded price.</summary>
        public decimal Price { get; set; }

        /// <summary>Trading volume for the current period.</summary>
        public decimal Volume { get; set; }

        /// <summary>Timestamp of the quote as <see cref="DateTime"/>.</summary>
        public DateTime Timestamp { get; set; }

        /// <summary>Currency of the quote price.</summary>
        public string Currency { get; set; } = string.Empty;

        /// <summary>Exchange or trading platform.</summary>
        public string Exchange { get; set; } = string.Empty;

        /// <summary>Data source identifier.</summary>
        public string Source { get; set; } = "TradingView";

        /// <summary>Underlying symbol if applicable.</summary>
        public string? Underlying { get; set; }

        /// <summary>Indicates if the data is for a futures contract.</summary>
        public bool IsFutures { get; set; }

        #endregion Properties
    }
}
