using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ark.Infrastructure.Info
{
    /// <summary>
    /// Default implementation of <see cref="IServiceInfoProvider"/> using <see cref="ServiceInfo"/>.
    /// + Offers synchronous and asynchronous querying helpers.
    /// - Performs no caching; each call queries the system again.
    /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.serviceprocess.servicecontroller"/>.
    /// </summary>
    public class ServiceInfoProvider : IServiceInfoProvider
    {
        #region Methods (Public)

        /// <inheritdoc />
        public Result<List<DetailedServiceInfoDto>> GetServices(string? namePattern = null, string? publisher = null, int eventLogMinutes = 60)
            => ServiceInfo.Get(namePattern, publisher, eventLogMinutes);

        /// <inheritdoc />
        public Task<Result<List<DetailedServiceInfoDto>>> GetServicesAsync(string? namePattern = null, string? publisher = null, int eventLogMinutes = 60)
            => ServiceInfo.GetAsync(namePattern, publisher, eventLogMinutes);

        #endregion Methods (Public)
    }
}
