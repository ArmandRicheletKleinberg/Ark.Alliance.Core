#nullable enable

namespace Ark.Core.Api.TradingView.Models
{
    /// <summary>
    /// Describes a ticker symbol returned by TradingView.
    /// + Includes exchange and currency metadata for integration.
    /// - Not all fields are guaranteed to be populated by the upstream service.
    /// Ref: <see href="https://www.tradingview.com/support/"/>
    /// </summary>
    public sealed class TickerInfo
    {
        #region Properties

        /// <summary>Symbol identifier (e.g. BINANCE:BTCUSDT).</summary>
        public string Symbol { get; set; } = string.Empty;

        /// <summary>Display name.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Exchange or platform.</summary>
        public string Exchange { get; set; } = string.Empty;

        /// <summary>Currency of the quote.</summary>
        public string Currency { get; set; } = string.Empty;

        /// <summary>International security identification number if available.</summary>
        public string? Isin { get; set; }

        #endregion Properties
    }
}
