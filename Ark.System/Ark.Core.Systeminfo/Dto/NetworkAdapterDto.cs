using System.Collections.Generic;

namespace Ark.Infrastructure.Info;

/// <summary>
/// Describes a network adapter.
/// + Captures addressing and throughput details.
/// - Provides no live monitoring information.
/// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.net.networkinformation.networkinterface"/>
/// </summary>
public class NetworkAdapterDto
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
    /// IP addresses assigned to the adapter.
    /// </summary>
    public List<string> IpAddresses { get; set; } = new();

    /// <summary>
    /// Indicates whether DHCP is enabled.
    /// </summary>
    public bool IsDhcpEnabled { get; set; }

    /// <summary>
    /// Physical MAC address.
    /// </summary>
    public string MacAddress { get; set; } = string.Empty;

    /// <summary>
    /// Link speed in bits per second.
    /// </summary>
    public long Speed { get; set; }

    #endregion Properties
}

