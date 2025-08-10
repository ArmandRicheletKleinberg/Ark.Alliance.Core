using System;
using System.Collections.Generic;



namespace Ark.Infrastructure.Info
{
    /// <summary>
    /// Describes a Windows service with runtime and executable information.
    /// + Combines metrics, start/stop times and related <see cref="EventLogDto"/> entries.
    /// - Some properties are only populated on Windows platforms.
    /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.serviceprocess.servicecontroller"/>.
    /// </summary>
    /// <remarks>
    /// <example>
    /// <code language="json">
    /// {
    ///   "serviceName": "ssh",
    ///   "status": "Running"
    /// }
    /// </code>
    /// </example>
    /// </remarks>
    public class DetailedServiceInfoDto
    {
        /// <summary>
        /// Gets or sets the service name.
        /// </summary>
        public string ServiceName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the display name of the service.
        /// </summary>
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the current status of the service.
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the service start type.
        /// </summary>
        public string StartType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the account used to run the service.
        /// </summary>
        public string Account { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date and time when the service was last started.
        /// </summary>
        public DateTime? LastStartTime { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the service was last stopped.
        /// </summary>
        public DateTime? LastStopTime { get; set; }

        /// <summary>
        /// Gets or sets the CPU usage percentage of the service.
        /// </summary>
        public double CpuUsage { get; set; }

        /// <summary>
        /// Gets or sets the proportion of CPU usage relative to the system.
        /// </summary>
        public double CpuProportion { get; set; }

        /// <summary>
        /// Gets or sets the memory usage in bytes.
        /// </summary>
        public double MemoryUsage { get; set; }

        /// <summary>
        /// Gets or sets the proportion of memory usage relative to the system.
        /// </summary>
        public double MemoryProportion { get; set; }

        /// <summary>
        /// Gets or sets the disk usage in bytes.
        /// </summary>
        public double DiskUsage { get; set; }

        /// <summary>
        /// Gets or sets the proportion of disk usage relative to the system.
        /// </summary>
        public double DiskProportion { get; set; }

        /// <summary>
        /// Gets or sets the network usage in bytes.
        /// </summary>
        public double NetworkUsage { get; set; }

        /// <summary>
        /// Gets or sets the proportion of network usage relative to the system.
        /// </summary>
        public double NetworkProportion { get; set; }

        /// <summary>
        /// Gets or sets the list of recent event logs related to the service.
        /// </summary>
        public List<EventLogDto> Events { get; set; } = new();

        /// <summary>
        /// Gets or sets the version of the service executable.
        /// </summary>
        public string Version { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the signature subject if the executable is digitally signed.
        /// </summary>
        public string Signature { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the publisher name extracted from the executable information.
        /// </summary>
        public string Publisher { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date of the executable file.
        /// </summary>
        public DateTime? ExecutableDate { get; set; }
    }
}
