using System;
using System.Collections.Generic;
using Ark.Alliance.Trading.Shared.Enums;

namespace Ark.Alliance.Trading.Shared.Models.Dtos;

/// <summary>
/// Represents the current risk evaluation for a trading symbol.
/// + Returned by backend risk endpoints and consumed by the dashboard.
/// - Factors provide descriptive hints without weighting details.
/// Ref: <see href="https://www.investopedia.com/terms/r/riskmanagement.asp" />
/// </summary>
public class RiskAssessmentDto
{
    /// <summary>
    /// Trading symbol assessed.
    /// + Identifies which market pair was evaluated.
    /// - Not validated against live exchange listings.
    /// </summary>
    public string Symbol { get; set; } = string.Empty;

    /// <summary>
    /// Time when the assessment was produced.
    /// + Allows clients to gauge staleness.
    /// - Depends on server clock accuracy.
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Computed risk level.
    /// + Simplifies display logic.
    /// - Coarse-grained; see <see cref="RiskScore"/> for nuance.
    /// </summary>
    public RiskLevel RiskLevel { get; set; }

    /// <summary>
    /// Aggregated risk score (implementation specific).
    /// + Enables fine-tuned thresholds.
    /// - Scale varies by backend implementation.
    /// </summary>
    public int RiskScore { get; set; }

    /// <summary>
    /// Primary reason for the risk level.
    /// + Useful for quick diagnosis.
    /// - Free-form text; not localised.
    /// </summary>
    public string Reason { get; set; } = string.Empty;

    /// <summary>
    /// List of contributing factors.
    /// + Provides deeper context for analysts.
    /// - Ordered arbitrarily; no weighting.
    /// </summary>
    public List<string> Factors { get; set; } = new();
}
