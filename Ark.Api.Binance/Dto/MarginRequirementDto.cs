using System;

namespace Ark.Api.Binance;

/// <summary>
/// Represents projected margin requirements for a potential futures position.
/// + Provides initial and maintenance margin estimates.
/// - Values are approximations until aligned with symbol-specific rules.
/// Ref: <see href="https://www.binance.com/en/support/faq/360033162192"/>
/// </summary>
public class MarginRequirementDto
{
    /// <summary>
    /// Estimated initial margin in quote asset (e.g. USDT).
    /// + Calculated from notional value divided by leverage.
    /// - Does not include fee or slippage adjustments.
    /// </summary>
    public decimal InitialMargin { get; set; }

    /// <summary>
    /// Estimated maintenance margin in quote asset.
    /// + Baseline threshold before liquidation.
    /// - Placeholder rate pending full rule integration.
    /// </summary>
    public decimal MaintenanceMargin { get; set; }
}

