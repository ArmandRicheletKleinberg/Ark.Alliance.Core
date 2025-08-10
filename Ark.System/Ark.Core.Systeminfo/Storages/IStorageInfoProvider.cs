using Ark;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ark.Infrastructure.Info
{
    /// <summary>
    /// Abstraction for retrieving storage information on the current platform.
    /// </summary>
    internal interface IStorageInfoProvider
    {
        /// <summary>
        /// Retrieves detailed storage information for all available drives.
        /// </summary>
        /// <returns>A <see cref="Result{T}"/> containing a list of <see cref="StorageInfoDto"/> objects.</returns>
        Result<List<StorageInfoDto>> GetStorageInfos();

        /// <summary>
        /// Asynchronously retrieves detailed storage information for all available drives.
        /// </summary>
        Task<Result<List<StorageInfoDto>>> GetStorageInfosAsync();
    }
}
