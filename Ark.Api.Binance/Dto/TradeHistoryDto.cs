namespace Ark.Api.Binance
{
    /// <summary>
    /// Represents a trade history entry.
    /// + Captures essential order execution details for auditing.
    /// - Omits link to containing session or related trades.
    /// </summary>
    public class TradeHistoryDto
    {
        /// <summary>
        /// Unique identifier of the trade.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Symbol traded.
        /// </summary>
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// Direction of the trade: "Buy" or "Sell".
        /// </summary>
        public string Side { get; set; } = string.Empty;

        /// <summary>
        /// Executed quantity.
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// Executed price.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Commission paid for the trade.
        /// </summary>
        public decimal Fee { get; set; }

        /// <summary>
        /// Profit and loss realized on this trade.
        /// </summary>
        public decimal RealizedPnl { get; set; }

        /// <summary>
        /// Leverage used for the position.
        /// </summary>
        public decimal Leverage { get; set; }

        /// <summary>
        /// Indicates how the trade was closed (liquidation, market, limit...).
        /// </summary>
        public string CloseType { get; set; } = string.Empty;

        /// <summary>
        /// Trade timestamp.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Status returned by Binance for this trade.
        /// </summary>
        public string Status { get; set; } = string.Empty;
    }
}
