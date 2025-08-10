using System.Collections.Generic;

namespace Ark.Infrastructure.Info;

/// <summary>
/// Snapshot of CPU, network, disk and assembly usage metrics for the host machine.
/// + Centralizes high-level performance counters for quick diagnostics.
/// - Values are instantaneous and may fluctuate rapidly.
/// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.diagnostics.performancecounter"/>
/// </summary>
public class HostMachinePerfInfosDto
{
    #region Properties

    /// <summary>
    /// CPU usage percentage.
    /// </summary>
    public double CPUUsage { get; set; }

    /// <summary>
    /// Network usage percentage.
    /// </summary>
    public double NetworkUsage { get; set; }

    /// <summary>
    /// Disk usage percentage.
    /// </summary>
    public double DiskUsage { get; set; }

    /// <summary>
    /// CPU usage of the current assembly.
    /// </summary>
    public double AssemblyCPUUsage { get; set; }

    /// <summary>
    /// Usage of individual drives.
    /// </summary>
    public List<DriveInfoDto> DrivesUsage { get; set; } = new();

    /// <summary>
    /// Overall drive space usage.
    /// </summary>
    public List<DriveInfoDto> GlobalDriveSpaceUsage { get; set; } = new();

    #endregion Properties
}

