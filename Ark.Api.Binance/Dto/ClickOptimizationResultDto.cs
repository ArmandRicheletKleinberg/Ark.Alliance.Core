namespace Ark.Api.Binance;

/// <summary>
/// Result returned after computing the optimal click amount.
/// + Summarizes the quantity, fees and profit for a single trade iteration.
/// - Estimates ignore slippage and sudden fee changes.
/// </summary>
public class ClickOptimizationResultDto
{
    /// <summary>Optimal quantity to place.</summary>
    public decimal OptimalAmount { get; set; }

    /// <summary>Estimated total fees for the operation.</summary>
    public decimal EstimatedTotalFees { get; set; }

    /// <summary>Estimated profit based on target.</summary>
    public decimal EstimatedProfit { get; set; }

    /// <summary>Required price movement.</summary>
    public decimal RequiredPriceMove { get; set; }

    /// <summary>Additional safety margin applied.</summary>
    public decimal SafetyMarginUsed { get; set; }

    /// <summary>Breakdown of fees by category.</summary>
    public FeeBreakdownDto FeeBreakdown { get; set; } = new();
}
