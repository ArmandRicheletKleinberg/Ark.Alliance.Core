namespace Ark.Api.Binance.Models;

/// <summary>
/// Internal representation of total fees for a trade.
/// + Breaks down entry, exit and funding costs.
/// - Ignores slippage and borrowing interest.
/// </summary>
public class CompleteTradeFees
{
    /// <summary>Fee paid when entering the trade.</summary>
    public decimal EntryFee { get; set; }

    /// <summary>Fee paid when exiting the trade.</summary>
    public decimal ExitFee { get; set; }

    /// <summary>Funding fee incurred during the position.</summary>
    public decimal FundingFee { get; set; }

    /// <summary>Sum of all fees related to the trade.</summary>
    public decimal TotalFees { get; set; }
}
