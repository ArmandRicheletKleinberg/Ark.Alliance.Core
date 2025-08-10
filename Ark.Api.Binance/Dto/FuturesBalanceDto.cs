namespace Ark.Api.Binance
{
    /// <summary>
    /// Represents the available balance for a futures quote asset and transfer limits.
    /// + Indicates transferable amounts and remaining quotas.
    /// - Does not include margin or isolated wallet balances.
    /// </summary>
    public class FuturesBalanceDto
    {
        /// <summary>Asset ticker (USDT or USDC).</summary>
        public string Asset { get; set; } = string.Empty;

        /// <summary>Amount currently available for trading.</summary>
        public decimal Available { get; set; }

        /// <summary>Maximum amount that can be transferred at once when known.</summary>
        public decimal MaxTransfer { get; set; }

        /// <summary>Remaining transfers allowed in the next hour if limited.</summary>
        public int TransfersRemaining { get; set; }

        /// <summary>Timestamp of the balance retrieval.</summary>
        public DateTime Timestamp { get; set; }
    }
}
