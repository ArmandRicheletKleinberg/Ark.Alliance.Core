using System.ComponentModel;

namespace Ark.Alliance.Trading.Shared.Models.Dtos;

/// <summary>
/// Represents projected margin requirements for a futures order.
/// + Used by front-end margin calculators.
/// - Values depend on exchange-provided parameters and may vary.
/// </summary>
public class MarginRequirementDto
{
    /// <summary>
    /// Gets or sets the initial margin required to open the position.
    /// </summary>
    public decimal InitialMargin { get; set; }

    /// <summary>
    /// Gets or sets the maintenance margin required to keep the position open.
    /// </summary>
    public decimal MaintenanceMargin { get; set; }
}

