using Binance.Net.Enums;

namespace Ark.Api.Binance
{
    /// <summary>
    /// Represents an open futures position.
    /// + Encapsulates symbol, side and valuation metrics.
    /// - Does not include margin or liquidation details.
    /// </summary>
    public class PositionDto
    {
        /// <summary>Trading symbol.</summary>
        public string Symbol { get; set; } = string.Empty;

        /// <summary>Position side.</summary>
        public PositionSide Side { get; set; }

        /// <summary>Quantity of the position.</summary>
        public decimal Quantity { get; set; }

        /// <summary>Entry price.</summary>
        public decimal EntryPrice { get; set; }

        /// <summary>Current mark price.</summary>
        public decimal MarkPrice { get; set; }

        /// <summary>Unrealized profit or loss.</summary>
        public decimal UnrealizedPnl { get; set; }

        /// <summary>Leverage used.</summary>
        public decimal Leverage { get; set; }

        /// <summary>Timestamp of the data.</summary>
        public DateTime Timestamp { get; set; }
    }
}
