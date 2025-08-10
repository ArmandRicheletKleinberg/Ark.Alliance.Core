using Ark;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Ark.Infrastructure.Info
{
    /// <summary>
    /// Retrieves storage information on Windows platforms.
    /// </summary>
    internal class WindowsStorageInfoProvider : IStorageInfoProvider
    {
        /// <inheritdoc />
        public Result<List<StorageInfoDto>> GetStorageInfos()
            => Result<List<StorageInfoDto>>.SafeExecute(StorageInfoUtils.GetStorageInfos);

        public Task<Result<List<StorageInfoDto>>> GetStorageInfosAsync() => Task.Run(GetStorageInfos);
    }
}
