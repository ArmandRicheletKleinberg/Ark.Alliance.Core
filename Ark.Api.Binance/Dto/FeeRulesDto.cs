namespace Ark.Api.Binance;

/// <summary>
/// Represents persisted fee rule settings.
/// + Stores maker, taker and funding rates for reuse.
/// - Values may become stale if not refreshed from the exchange.
/// </summary>
public class FeeRulesDto
{
    /// <summary>Maker fee rate.</summary>
    public decimal MakerFee { get; set; }

    /// <summary>Taker fee rate.</summary>
    public decimal TakerFee { get; set; }

    /// <summary>Current funding rate.</summary>
    public decimal FundingRate { get; set; }
}
