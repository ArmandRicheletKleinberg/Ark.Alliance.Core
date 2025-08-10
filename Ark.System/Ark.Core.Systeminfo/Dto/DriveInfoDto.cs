using System.IO;

namespace Ark.Infrastructure.Info;

/// <summary>
/// Snapshot of <see cref="DriveInfo"/> values.
/// + Aggregates capacity and usage metrics for reporting.
/// - Does not refresh values automatically after creation.
/// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.io.driveinfo"/>
/// </summary>
public class DriveInfoDto
{
    #region Properties

    /// <summary>
    /// Available free space on the drive, in bytes.
    /// + Useful for capacity planning.
    /// - May be outdated if the drive changes after retrieval.
    /// </summary>
    public long AvailableFreeSpace { get; set; }

    /// <summary>
    /// Name of the file system such as <c>NTFS</c> or <c>FAT32</c>.
    /// </summary>
    public string DriveFormat { get; set; } = string.Empty;

    /// <summary>
    /// Drive type (e.g., <see cref="DriveType.Fixed"/>).
    /// </summary>
    public DriveType DriveType { get; set; }

    /// <summary>
    /// Indicates whether the drive is ready.
    /// </summary>
    public bool IsReady { get; set; }

    /// <summary>
    /// Drive name such as <c>C:\</c>.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Root directory of the drive.
    /// </summary>
    public DirectoryInfo RootDirectory { get; set; } = null!;

    /// <summary>
    /// Total free space on the drive, in bytes.
    /// </summary>
    public long TotalFreeSpace { get; set; }

    /// <summary>
    /// Total size of the drive, in bytes.
    /// </summary>
    public long TotalSize { get; set; }

    /// <summary>
    /// Drive identifier or volume path.
    /// </summary>
    public string Drive { get; set; } = string.Empty;

    /// <summary>
    /// Volume label assigned to the drive.
    /// </summary>
    public string VolumeLabel { get; set; } = string.Empty;

    /// <summary>
    /// Percentage of drive space in use.
    /// </summary>
    public double UsagePercentage { get; set; }

    #endregion Properties
}

