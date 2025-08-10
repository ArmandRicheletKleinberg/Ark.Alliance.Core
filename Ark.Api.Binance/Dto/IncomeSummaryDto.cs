using Binance.Net.Enums;

namespace Ark.Api.Binance
{
    /// <summary>
    /// Summarizes futures income including fees and profit.
    /// + Aggregates raw amounts and net totals for analysis.
    /// - Does not convert values to a common quote asset.
    /// </summary>
    public class IncomeSummaryDto
    {
        /// <summary>
        /// Symbol this income entry relates to.
        /// </summary>
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// Time of the income event.
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// Type of income (commission, realized PnL, funding fee, etc.).
        /// </summary>
        public IncomeType IncomeType { get; set; }

        /// <summary>
        /// Raw amount received from Binance.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Fee associated with this income entry. Negative values indicate a loss.
        /// </summary>
        public decimal Fee { get; set; }

        /// <summary>
        /// Net income after fees (can be negative for a loss).
        /// </summary>
        public decimal NetIncome { get; set; }

        /// <summary>
        /// Status returned by Binance for this income entry.
        /// </summary>
        public string Status { get; set; } = string.Empty;
    }
}
