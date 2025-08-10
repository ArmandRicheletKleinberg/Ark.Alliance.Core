using Ark;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ark.Infrastructure.Info
{
    /// <summary>
    /// Default storage provider when the operating system is not recognized.
    /// </summary>
    internal class DefaultStorageInfoProvider : IStorageInfoProvider
    {
        /// <inheritdoc />
        public Result<List<StorageInfoDto>> GetStorageInfos() => StorageInfoUtils.GetStorageInfos();

        public Task<Result<List<StorageInfoDto>>> GetStorageInfosAsync() => Task.Run(GetStorageInfos);
    }
}
