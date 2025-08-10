using Binance.Net.Enums;

namespace Ark.Api.Binance
{
    /// <summary>
    /// Represents a futures order request.
    /// + Encapsulates parameters for Binance futures placement.
    /// - Does not validate against symbol-specific filters.
    /// Ref: <see href="https://binance-docs.github.io/apidocs/futures/en/#new-order-trade"/>
    /// </summary>
    public class FuturesOrder
    {
        /// <summary>
        /// Trading symbol (e.g. BTCUSDT).
        /// </summary>
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// Order side (Buy or Sell).
        /// </summary>
        public OrderSide Side { get; set; } = OrderSide.Buy;

        /// <summary>
        /// Order type (Market, Limit, Stop, ...).
        /// </summary>
        public FuturesOrderType Type { get; set; } = FuturesOrderType.Limit;

        /// <summary>
        /// Quantity to trade.
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// Price for limit orders.
        /// </summary>
        public decimal? Price { get; set; }

        /// <summary>
        /// Stop price when applicable.
        /// </summary>
        public decimal? StopPrice { get; set; }

        /// <summary>
        /// Time in force policy.
        /// </summary>
        public TimeInForce TimeInForce { get; set; } = TimeInForce.FillOrKill;

        /// <summary>
        /// Indicates if the order reduces an existing position.
        /// </summary>
        public bool ReduceOnly { get; set; }

        /// <summary>
        /// Position side (Long, Short or Both).
        /// </summary>
        public PositionSide PositionSide { get; set; } = PositionSide.Both;

        /// <summary>
        /// Optional client order identifier.
        /// </summary>
        public string ClientOrderId { get; set; } = string.Empty;
    }
}
