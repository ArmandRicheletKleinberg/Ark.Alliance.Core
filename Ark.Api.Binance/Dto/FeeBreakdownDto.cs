namespace Ark.Api.Binance;

/// <summary>
/// Detailed fee composition for a trade.
/// + Provides visibility into entry, exit, funding and slippage costs.
/// - Does not account for borrowing or interest charges.
/// </summary>
public class FeeBreakdownDto
{
    /// <summary>Fee paid when entering the position.</summary>
    public decimal EntryFee { get; set; }

    /// <summary>Fee paid when exiting the position.</summary>
    public decimal ExitFee { get; set; }

    /// <summary>Estimated funding fee.</summary>
    public decimal FundingFee { get; set; }

    /// <summary>Approximate slippage cost.</summary>
    public decimal SlippageEstimate { get; set; }
}
