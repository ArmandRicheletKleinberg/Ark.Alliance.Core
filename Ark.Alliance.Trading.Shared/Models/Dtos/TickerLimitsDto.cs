using System;

namespace Ark.Alliance.Trading.Shared.Models.Dtos;

/// <summary>
/// Trading constraints for a Binance Futures symbol.
/// + Exposes leverage and quantity precision.
/// - Values may become outdated as exchange rules evolve.
/// </summary>
public class TickerLimitsDto
{
    /// <summary>Symbol name, e.g. <c>BTCUSDT</c>.</summary>
    public string Symbol { get; set; } = string.Empty;

    /// <summary>
    /// + Maximum leverage permitted by Binance for the symbol.
    /// - Application may cap this value for safety.
    /// </summary>
    public int? MaxLeverage { get; set; }

    /// <summary>
    /// + Allowed decimal precision for order quantities.
    /// - Orders with higher precision will be rejected by Binance.
    /// </summary>
    public int QuantityPrecision { get; set; }
}
