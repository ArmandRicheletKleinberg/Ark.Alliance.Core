namespace Ark.Api.Binance
{
    /// <summary>
    /// Simple ticker information.
    /// + Captures last price and volume for quick lookups.
    /// - Does not include order book depth or best bid/ask.
    /// </summary>
    public class TickerDto
    {
        /// <summary>Trading symbol.</summary>
        public string Symbol { get; set; } = string.Empty;

        /// <summary>Last traded price.</summary>
        public decimal Price { get; set; }

        /// <summary>Trade volume in base asset.</summary>
        public decimal Volume { get; set; }

        /// <summary>Timestamp of the ticker.</summary>
        public DateTime Timestamp { get; set; }
    }
}
