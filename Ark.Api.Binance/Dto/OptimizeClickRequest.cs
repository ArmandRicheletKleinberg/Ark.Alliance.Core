using System;

namespace Ark.Api.Binance;

/// <summary>
/// Request payload to compute the optimal click amount for a given symbol.
/// + Includes current price, desired profit and leverage configuration.
/// - Does not account for symbol filters or precision limits.
/// </summary>
public class OptimizeClickRequest
{
    /// <summary>Current market price.</summary>
    public decimal CurrentPrice { get; set; }

    /// <summary>Target profit percentage (e.g. 0.1 for 0.1%).</summary>
    public decimal TargetProfitPct { get; set; }

    /// <summary>Leverage applied to the position.</summary>
    public int Leverage { get; set; }

    /// <summary>True when the position is long, false for short.</summary>
    public bool IsLong { get; set; }

    /// <summary>Binance VIP level (0-9).</summary>
    public int VipLevel { get; set; } = 0;

    /// <summary>Use BNB for fee discounts.</summary>
    public bool UseBnbDiscount { get; set; } = false;
}
