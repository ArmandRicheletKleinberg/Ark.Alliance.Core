using Ark;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ark.Infrastructure.Info
{
    /// <summary>
    /// Entry point to retrieve storage information across platforms.
    /// + Dispatches to platform-specific providers for accurate metrics.
    /// - Each call probes the file system which may be slow on large disks.
    /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.io.driveinfo"/>
    /// </summary>
    public static class StorageInfoProvider
    {
        private static readonly IStorageInfoProvider _provider = PlatformProvider.Create<IStorageInfoProvider>(
            () => new WindowsStorageInfoProvider(),
            () => new LinuxStorageInfoProvider(),
            () => new AndroidStorageInfoProvider(),
            () => new IosStorageInfoProvider(),
            () => new DefaultStorageInfoProvider());

        #region Methods

        /// <summary>
        /// Gets detailed information about available storage devices.
        /// + Synchronous for simple scripts and tooling.
        /// - May block while enumerating slow devices.
        /// </summary>
        /// <returns>
        /// A <see cref="Result{T}"/> containing a list of <see cref="StorageInfoDto"/> objects.
        /// Example:
        /// <code language="json">
        /// [
        ///   { "name": "C:", "totalSize": 500000000000 }
        /// ]
        /// </code>
        /// </returns>
        public static Result<List<StorageInfoDto>> GetStorageInfos() => _provider.GetStorageInfos();

        /// <summary>
        /// Asynchronously gets detailed information about available storage devices.
        /// + Ideal for UI applications.
        /// - Still performs disk I/O which may be expensive.
        /// </summary>
        /// <returns>A task yielding the same payload as <see cref="GetStorageInfos"/>.</returns>
        public static Task<Result<List<StorageInfoDto>>> GetStorageInfosAsync() => _provider.GetStorageInfosAsync();

        #endregion Methods
    }
}
