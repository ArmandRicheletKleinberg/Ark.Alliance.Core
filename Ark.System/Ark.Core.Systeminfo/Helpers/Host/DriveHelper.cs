using System.Collections.Generic;
using System.IO;

namespace Ark.Infrastructure.Info;

/// <summary>
/// Utility for retrieving drive metrics using <see cref="DriveInfo"/>.
/// + Consolidates per‑drive statistics and computes global usage.
/// - Skips drives that are not ready, such as removable media.
/// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.io.driveinfo"/>
/// </summary>
public static class DriveHelper
{
    #region Methods
    /// <summary>
    /// Gets detailed information about each ready drive and calculates global disk usage.
    /// + Iterates drives once to collect totals and details.
    /// - Uses <see cref="DriveInfo"/> which may be slow on network shares.
    /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.io.driveinfo"/>
    /// </summary>
    /// <returns>
    /// Tuple with <c>GlobalUsagePercentage</c> and collection of <see cref="DriveInfoDto"/>.
    /// Example JSON:
    /// <code language="json">
    /// {
    ///   "globalUsagePercentage": 72.5,
    ///   "driveDetails": [
    ///     { "name": "C:\\", "usagePercentage": 70.1 }
    ///   ]
    /// }
    /// </code>
    /// </returns>
    public static (double GlobalUsagePercentage, List<DriveInfoDto> DriveDetails) GetDriveUsageDetails()
    {
        DriveInfo[] drives = DriveInfo.GetDrives();
        long totalSpace = 0;
        long freeSpace = 0;
        List<DriveInfoDto> driveDetails = new(drives.Length);

        foreach (DriveInfo d in drives)
        {
            if (!d.IsReady)
                continue;

            totalSpace += d.TotalSize;
            freeSpace += d.TotalFreeSpace;

            driveDetails.Add(new DriveInfoDto
            {
                AvailableFreeSpace = d.AvailableFreeSpace,
                DriveFormat = d.DriveFormat,
                DriveType = d.DriveType,
                IsReady = true,
                Name = d.Name,
                RootDirectory = d.RootDirectory,
                TotalFreeSpace = d.TotalFreeSpace,
                TotalSize = d.TotalSize,
                VolumeLabel = d.VolumeLabel,
                UsagePercentage = (double)(d.TotalSize - d.TotalFreeSpace) / d.TotalSize * 100
            });
        }

        double globalUsagePercentage = totalSpace == 0 ? 0 : (double)(totalSpace - freeSpace) / totalSpace * 100;
        return (globalUsagePercentage, driveDetails);
    }
    #endregion Methods
}

