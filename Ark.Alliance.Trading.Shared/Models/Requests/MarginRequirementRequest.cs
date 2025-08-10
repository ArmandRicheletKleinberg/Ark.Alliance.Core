using System.ComponentModel.DataAnnotations;

namespace Ark.Alliance.Trading.Shared.Models.Requests;

/// <summary>
/// Request to project margin requirements for a prospective trade.
/// + Supplies trade parameters for projection.
/// - Does not validate symbol-specific leverage caps.
/// Ref: <see href="https://www.binance.com/en/futures/fee" />
/// </summary>
public class MarginRequirementRequest
{
    /// <summary>
    /// Futures contract symbol.
    /// + Determines contract size and tick value.
    /// - Case-sensitive; mismatches cause rejection.
    /// </summary>
    [Required]
    public string Symbol { get; set; } = string.Empty;

    /// <summary>
    /// Number of contracts to open.
    /// + Drives margin calculation.
    /// - Must respect exchange minimums.
    /// </summary>
    [Range(0.0001, double.MaxValue)]
    public decimal Quantity { get; set; }

    /// <summary>
    /// Entry price in quote asset.
    /// + Used to compute notional value.
    /// - High precision may be truncated by exchange.
    /// </summary>
    [Range(0.0001, double.MaxValue)]
    public decimal Price { get; set; }

    /// <summary>
    /// Applied leverage multiplier.
    /// + Allows margin reduction for lower capital usage.
    /// - Excess leverage increases liquidation risk.
    /// </summary>
    [Range(1, 125)]
    public int Leverage { get; set; }
}
