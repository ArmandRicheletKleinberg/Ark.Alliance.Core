using System.Collections.Generic;

namespace Ark.Infrastructure.Info
{
    /// <summary>
    /// Represents host network configuration and occupied ports.
    /// + Useful for troubleshooting connectivity issues.
    /// - Does not resolve DNS names for addresses.
    /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.net.networkinformation.ipglobalproperties"/>
    /// </summary>
    /// <example>
    /// {
    ///   "ipAddress": "192.168.1.10",
    ///   "occupiedPorts": [{"portNumber": 80, "processId": 1234}]
    /// }
    /// </example>
    public class NetworkInfoDto
    {
        #region Properties

        /// <summary>
        /// Host IPv4 or IPv6 address.
        /// </summary>
        public string IPAddress { get; set; } = string.Empty;

        /// <summary>
        /// Ports currently in use on the host. Each entry is a <see cref="PortInfoDto"/>.
        /// </summary>
        public List<PortInfoDto> OccupiedPorts { get; set; } = new();

        #endregion Properties
    }
}
