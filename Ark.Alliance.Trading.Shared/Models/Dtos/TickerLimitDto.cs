namespace Ark.Alliance.Trading.Shared.Models.Dtos;

/// <summary>
/// Describes exchange-imposed limits for a trading symbol.
/// + Provides max leverage and quantity precision to clients.
/// - Values may require periodic refresh.
/// </summary>
public class TickerLimitDto
{
    /// <summary>Trading pair symbol.</summary>
    public string Symbol { get; set; } = string.Empty;

    /// <summary>Highest leverage allowed.</summary>
    public int? MaxLeverage { get; set; }

    /// <summary>Permitted quantity decimal places.</summary>
    public int QuantityPrecision { get; set; }
}
