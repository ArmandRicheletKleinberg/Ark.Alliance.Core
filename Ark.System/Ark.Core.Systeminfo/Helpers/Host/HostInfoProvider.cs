using System.Collections.Generic;

namespace Ark.Infrastructure.Info
{
    /// <summary>
    /// Aggregates host‑level diagnostics.
    /// + Exposes drive, network and OS details through high‑level helpers.
    /// - Recomputes data on each call without caching.
    /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.environment"/>
    /// </summary>
    public static class HostInfoProvider
    {
        #region Properties
        /// <summary>
        /// Current operating system kind.
        /// + Delegates detection to <see cref="OSHelper.GetCurrentPlatform"/>.
        /// - Unknown platforms return <see cref="OperatingSystemKind.Unknown"/>.
        /// </summary>
        public static OperatingSystemKind CurrentOs => OSHelper.GetCurrentPlatform();
        #endregion

        #region Methods
        /// <summary>
        /// Aggregates disk usage and per‑drive metrics.
        /// + Uses <see cref="DriveHelper.GetDriveUsageDetails"/> for ready drives.
        /// - Ignores unmounted or inaccessible drives.
        /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.io.driveinfo"/>
        /// </summary>
        public static (double GlobalUsagePercentage, List<DriveInfoDto> DriveDetails) GetDriveInfo()
            => DriveHelper.GetDriveUsageDetails();

        /// <summary>
        /// Retrieves IP configuration and occupied ports.
        /// + Leverages <see cref="NetworkHelper"/> to consolidate network details.
        /// - Returns only the first detected address.
        /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.net.networkinformation.ipglobalproperties"/>
        /// </summary>
        public static Result<NetworkInfoDto> GetNetworkInfo()
            => NetworkHelper.GetNetworkInfo();

        #endregion
    }
}
