using Ark;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ark.Infrastructure.Info
{
    /// <summary>
    /// Retrieves storage information on Android platforms.
    /// </summary>
    internal class AndroidStorageInfoProvider : IStorageInfoProvider
    {
        /// <inheritdoc />
        public Result<List<StorageInfoDto>> GetStorageInfos()
            => (StorageInfoUtils.GetStorageInfos());

        public Task<Result<List<StorageInfoDto>>> GetStorageInfosAsync() => Task.Run(GetStorageInfos);
    }
}
