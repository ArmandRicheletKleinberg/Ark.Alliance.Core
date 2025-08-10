using System;

namespace Ark.Alliance.Trading.Shared.Enums;

/// <summary>
/// Defines overall risk exposure levels.
/// + Shared between backend risk management and frontend UI indicators.
/// - Does not quantify precise exposure; consult backend logs for details.
/// Ref: <see href="https://www.investopedia.com/terms/r/risk-level.asp" />
/// </summary>
public enum RiskLevel
{
    Low,
    Medium,
    High,
    Critical
}
