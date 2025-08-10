using System.ComponentModel.DataAnnotations;

namespace Ark.Api.Binance.Dto;

/// <summary>
/// Request to project margin requirements for a prospective trade.
/// + Supplies trade parameters for projection.
/// - Does not validate symbol-specific leverage caps.
/// </summary>
public class MarginRequirementRequest
{
    /// <summary>
    /// Futures contract symbol.
    /// </summary>
    [Required]
    public string Symbol { get; set; } = string.Empty;

    /// <summary>
    /// Number of contracts to open.
    /// </summary>
    [Range(0.0001, double.MaxValue)]
    public decimal Quantity { get; set; }

    /// <summary>
    /// Entry price in quote asset.
    /// </summary>
    [Range(0.0001, double.MaxValue)]
    public decimal Price { get; set; }

    /// <summary>
    /// Applied leverage multiplier.
    /// </summary>
    [Range(1, 125)]
    public int Leverage { get; set; }
}

