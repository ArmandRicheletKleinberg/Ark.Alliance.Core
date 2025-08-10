namespace Ark.Api.Binance;

/// <summary>
/// Request payload to update trading fee rules.
/// + Supports adjusting maker/taker rates and funding assumptions.
/// - Updates apply globally; per-user tiers are not handled.
/// </summary>
public class UpdateFeeRulesRequest
{
    /// <summary>Maker fee rate.</summary>
    public decimal MakerFee { get; set; }

    /// <summary>Taker fee rate.</summary>
    public decimal TakerFee { get; set; }

    /// <summary>Estimated funding rate.</summary>
    public decimal FundingRate { get; set; }
}
