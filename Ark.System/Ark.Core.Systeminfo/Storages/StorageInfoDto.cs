using System;

namespace Ark.Infrastructure.Info
{
    /// <summary>
    /// Represents information about a storage device or logical drive.
    /// + Aggregates key metrics like capacity and permissions.
    /// - Captures only a snapshot at the time of collection.
    /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.io.driveinfo"/>
    /// </summary>
    public class StorageInfoDto
    {
        #region Properties

        /// <summary>
        /// Gets or sets the display name of the drive.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the physical path of the drive if known.
        /// </summary>
        public string PhysicalPath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the logical path such as the mount point.
        /// </summary>
        public string LogicalPath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets an optional alias for the drive.
        /// </summary>
        public string Alias { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the access permissions for the drive root.
        /// </summary>
        public string Permissions { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the remaining free space in bytes.
        /// </summary>
        public long AvailableFreeSpace { get; set; }

        /// <summary>
        /// Gets or sets the total size in bytes.
        /// </summary>
        public long TotalSize { get; set; }

        /// <summary>
        /// Gets or sets the hardware drive type if available.
        /// </summary>
        public string DriveType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the measured read performance in MB/s.
        /// </summary>
        public double ReadSpeed { get; set; }

        /// <summary>
        /// Gets or sets the measured write performance in MB/s.
        /// </summary>
        public double WriteSpeed { get; set; }

        #endregion Properties
    }
}
