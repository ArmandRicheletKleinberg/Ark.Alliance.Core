
namespace Ark.Infrastructure.Info
{
    /// <summary>
    /// Repository providing access to system service information.
    /// + Wraps <see cref="ServiceInfoProvider"/> for simple consumption.
    /// - Instantiates providers on each call without caching.
    /// </summary>
    public class SystemInfoRepository
    {
        #region Methods (Public)

        /// <summary>
        /// Gets detailed information about installed services.
        /// </summary>
        /// <param name="namePattern">Optional pattern the service name must start with.</param>
        /// <param name="publisher">Optional publisher contained in the executable.</param>
        /// <param name="eventLogMinutes">Time span in minutes for returned event logs.</param>
        /// <returns>A <see cref="Result{T}"/> containing the matching services.</returns>
        public Task<Result<List<DetailedServiceInfoDto>>> GetServicesAsync(string? namePattern = null, string? publisher = null, int eventLogMinutes = 60)
            => new ServiceInfoProvider().GetServicesAsync(namePattern, publisher, eventLogMinutes);

        /// <summary>
        /// Gets detailed information about installed services synchronously.
        /// </summary>
        /// <param name="namePattern">Optional pattern the service name must start with.</param>
        /// <param name="publisher">Optional publisher contained in the executable.</param>
        /// <param name="eventLogMinutes">Time span in minutes for returned event logs.</param>
        /// <returns>A <see cref="Result{T}"/> containing the matching services.</returns>
        public Result<List<DetailedServiceInfoDto>> GetServices(string? namePattern = null, string? publisher = null, int eventLogMinutes = 60)
            => new ServiceInfoProvider().GetServices(namePattern, publisher, eventLogMinutes);

        #endregion Methods (Public)
    }

}
