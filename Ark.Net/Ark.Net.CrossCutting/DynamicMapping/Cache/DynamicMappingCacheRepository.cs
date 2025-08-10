using Ark.Data;

namespace Ark.Net.CrossCutting
{
    /// <inheritdoc />
    /// <summary>
    /// The cache with all dynamic mapping data.
    /// </summary>
    internal class DynamicMappingCacheRepository : MemoryWithEnumKeyRepositoryBase<DynamicMappingEnum, DynamicMappingCache>
    {
    }
}