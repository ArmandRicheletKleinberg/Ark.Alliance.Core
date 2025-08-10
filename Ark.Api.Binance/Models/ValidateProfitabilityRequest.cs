namespace Ark.Api.Binance;
/// <summary>
/// Request payload used to validate trade profitability after fees.
/// + Provides inputs for comparing profit and fee costs.
/// - Assumes static fees and ignores slippage.
/// </summary>
public class ValidateProfitabilityRequest
{
    /// <summary>Quantity of contracts traded.</summary>
    public decimal Quantity { get; set; }

    /// <summary>Price at entry.</summary>
    public decimal EntryPrice { get; set; }

    /// <summary>Expected exit price.</summary>
    public decimal ExitPrice { get; set; }

    /// <summary>True if the entry order is maker.</summary>
    public bool EntryIsMaker { get; set; }

    /// <summary>True if the exit order is maker.</summary>
    public bool ExitIsMaker { get; set; }

    /// <summary>Binance VIP level (0-9).</summary>
    public int VipLevel { get; set; } = 0;
}
