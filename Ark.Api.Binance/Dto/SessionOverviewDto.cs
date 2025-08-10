using System.Collections.Generic;

namespace Ark.Api.Binance
{
    /// <summary>
    /// Aggregated session data including orders, positions and ticker history.
    /// + Provides a single snapshot of session state for diagnostics.
    /// - Potentially heavy object; avoid returning for frequent polling.
    /// </summary>
    public class SessionOverviewDto
    {
        /// <summary>Open orders related to the session.</summary>
        public List<OrderResultDto> Orders { get; set; } = new();

        /// <summary>Open positions for the session.</summary>
        public List<PositionDto> Positions { get; set; } = new();

        /// <summary>
        /// Historical tickers keyed by trading symbol.
        /// </summary>
        public Dictionary<string, List<TickerDto>> Tickers { get; set; } = new();

        /// <summary>
        /// Available futures balances for the session.
        /// </summary>
        public List<FuturesBalanceDto> Balances { get; set; } = new();

        /// <summary>
        /// Income summaries recorded for the session.
        /// </summary>
        public List<IncomeSummaryDto> IncomeSummaries { get; set; } = new();

        /// <summary>
        /// Total stablecoins currently available for futures trading.
        /// </summary>
        public decimal FuturesTradingAvailable { get; set; }

        /// <summary>
        /// Environment used when the session was created.
        /// </summary>
        public BinanceEnvironment Environment { get; set; }
    }
}
