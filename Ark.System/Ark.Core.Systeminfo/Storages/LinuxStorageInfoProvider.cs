using Ark;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ark.Infrastructure.Info
{
    /// <summary>
    /// Retrieves storage information on Linux platforms.
    /// </summary>
    internal class LinuxStorageInfoProvider : IStorageInfoProvider
    {
        /// <inheritdoc />
        public Result<List<StorageInfoDto>> GetStorageInfos()
            => Result<List<StorageInfoDto>>.SafeExecute(StorageInfoUtils.GetStorageInfos);

        public Task<Result<List<StorageInfoDto>>> GetStorageInfosAsync() => Task.Run(GetStorageInfos);
    }
}
