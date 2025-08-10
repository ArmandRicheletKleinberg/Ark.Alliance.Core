using System.Collections.Generic;

namespace Ark.Infrastructure.Info;

/// <summary>
/// DTO describing a network adapter and its current usage.
/// + Exposes both configuration and performance data.
/// - Requires administrative permissions on some platforms.
/// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.net.networkinformation.ipinterfaceproperties"/>
/// </summary>
public class NetworkAdapterInfoDto
{
    #region Properties

    /// <summary>
    /// Adapter name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Adapter description.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Interface identifier.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether DHCP is enabled.
    /// </summary>
    public bool IsDhcpEnabled { get; set; }

    /// <summary>
    /// IP addresses assigned to the adapter.
    /// </summary>
    public List<string> Addresses { get; set; } = new();

    /// <summary>
    /// Interface speed in bits per second.
    /// </summary>
    public long Speed { get; set; }

    /// <summary>
    /// Current network utilization percentage.
    /// </summary>
    public double UsagePercentage { get; set; }

    #endregion Properties
}

