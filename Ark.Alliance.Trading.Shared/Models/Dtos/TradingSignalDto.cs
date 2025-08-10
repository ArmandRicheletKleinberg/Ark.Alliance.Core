namespace Ark.Alliance.Trading.Shared.Models.Dtos;

/// <summary>
/// Represents a trading signal prediction.
/// + Used to transfer AI trading predictions between backend and frontend.
/// - Does not include execution metadata.
/// </summary>
public class TradingSignalDto
{
    /// <summary>
    /// Numeric prediction value produced by the model.
    /// </summary>
    public decimal Prediction { get; set; }

    /// <summary>
    /// Confidence score for the prediction.
    /// </summary>
    public decimal Confidence { get; set; }

    /// <summary>
    /// Suggested trade direction such as BUY or SELL.
    /// </summary>
    public string Direction { get; set; } = string.Empty;
}
