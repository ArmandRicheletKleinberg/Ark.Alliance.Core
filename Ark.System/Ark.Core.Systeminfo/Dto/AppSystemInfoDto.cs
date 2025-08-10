
using System.Collections.Generic;

namespace Ark.Infrastructure.Info
{
    /// <summary>
    /// Aggregates runtime diagnostics for the current application and host machine.
    /// + Centralizes framework, service, and resource metrics in a single payload.
    /// - Collecting values may incur overhead on constrained devices.
    /// Ref: <see href="https://learn.microsoft.com/dotnet/core/diagnostics/"/>
    /// </summary>
    /// <example>
    /// {
    ///   "framework": "NET 9.0",
    ///   "operatingSystem": "Linux",
    ///   "memoryUsage": 64.2
    /// }
    /// </example>

    public class AppSystemInfoDto
    {
        #region Properties

        /// <summary>
        /// CLR or runtime identifier (e.g., <c>.NET 9.0</c>).
        /// + Enables telemetry correlation across services.
        /// - Does not include patch level information.
        /// </summary>
        public string Framework { get; set; } = string.Empty;

        /// <summary>
        /// Programming language used to build the application.
        /// </summary>
        public string ProgrammingLanguage { get; set; } = string.Empty;

        /// <summary>
        /// Human-readable operating system name.
        /// </summary>
        public string OperatingSystem { get; set; } = string.Empty;

        /// <summary>
        /// OS family kind such as <see cref="OperatingSystemKind"/>.
        /// </summary>
        public string OSType { get; set; } = string.Empty;

        /// <summary>
        /// Version number of the host operating system.
        /// </summary>
        public string OSVersion { get; set; } = string.Empty;

        /// <summary>
        /// Services currently running on the host. Each entry is a <see cref="ServiceInfoDto"/>.
        /// </summary>
        public List<ServiceInfoDto> RunningServices { get; set; } = [];

        /// <summary>
        /// Host machine identifier.
        /// </summary>
        public string MachineName { get; set; } = string.Empty;

        /// <summary>
        /// Domain or workgroup name.
        /// </summary>
        public string DomainName { get; set; } = string.Empty;

        /// <summary>
        /// Detailed network information about the host.
        /// + Includes IP configuration and port usage via <see cref="HostNetworkInfoDto"/>.
        /// - Retrieval may be slow on systems with many adapters.
        /// </summary>
        public HostNetworkInfoDto HostNetworkInfo { get; set; } = new();

        /// <summary>
        /// Gets or sets the network usage percentage.
        /// + Derived from IPv4 statistics.
        /// - IPv6 traffic is not included.
        /// </summary>
        public double NetworkUsage { get; set; }

        /// <summary>
        /// Disk utilization percentage.
        /// + Aggregates space across all ready drives.
        /// - Removable media are ignored.
        /// </summary>
        public double DiskUsage { get; set; }

        /// <summary>
        /// Memory utilization percentage.
        /// </summary>
        public double MemoryUsage { get; set; }

        /// <summary>
        /// CPU usage for the current assembly or process.
        /// </summary>
        public double AssemblyCPUUsage { get; set; }

        /// <summary>
        /// Event logs related to the binary.
        /// </summary>
        public List<EventLogDto> EventLogs { get; set; } = [];

        /// <summary>
        /// Network information for the host machine.
        /// </summary>
        public HostNetworkInfoDto NetworkInfo { get; set; } = new();

        #endregion Properties
    }
}
