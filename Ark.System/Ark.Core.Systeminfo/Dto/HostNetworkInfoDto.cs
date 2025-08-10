using System.Collections.Generic;

namespace Ark.Infrastructure.Info;

/// <summary>
/// Aggregated network information for the host machine.
/// + Combines adapter, port, and usage metrics.
/// - Transient interfaces may be omitted.
/// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.net.networkinformation"/>
/// </summary>
public class HostNetworkInfoDto
{
    #region Properties

    /// <summary>
    /// Network adapters information.
    /// </summary>
    public List<NetworkAdapterInfoDto> Adapters { get; set; } = new();

    /// <summary>
    /// Ports currently bound by services.
    /// </summary>
    public List<PortInfoDto> OccupiedPorts { get; set; } = new();

    /// <summary>
    /// Open ports reported by the OS.
    /// </summary>
    public List<int> OpenPorts { get; set; } = new();

    /// <summary>
    /// Current network usage percentage.
    /// </summary>
    public double UsagePercent { get; set; }

    #endregion Properties
}

