using Binance.Net.Enums;


namespace Ark.Api.Binance
{
    /// <summary>
    /// Result returned by Binance when placing or modifying a futures order.
    /// Contains the original order details with additional information from Binance.
    /// + Provides identifiers and status needed for follow-up operations.
    /// - Mirrors Binance responses; schema changes may require updates.
    /// </summary>
    public class OrderResultDto : FuturesOrder
    {
        /// <summary>
        /// Identifier assigned by Binance.
        /// </summary>
        public long OrderId { get; set; }

        /// <summary>
        /// Time the order was recorded by Binance (UTC).
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Status returned by Binance for this order.
        /// </summary>
        public OrderStatus Status { get; set; }
    }
}
