using Ark;
using System.Threading.Tasks;

namespace Ark.Infrastructure.Info
{
    /// <summary>
    /// Provides rich environment diagnostics including running services and resource usage.
    /// + Enables cross-platform monitoring via pluggable providers.
    /// - May require elevated permissions on restricted systems.
    /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.environment"/>
    /// </summary>
    public interface IEnvironmentInfoProvider
    {
        #region Methods

        /// <summary>
        /// Gets detailed environment information.
        /// + Synchronous call suitable for scripts.
        /// - Blocks the calling thread while gathering metrics.
        /// </summary>
        /// <param name="since">The number of minutes of event logs to include.</param>
        /// <returns>
        /// A <see cref="Result{T}"/> containing an <see cref="AppSystemInfoDto"/> payload.
        /// Example:
        /// <code language="json">
        /// {
        ///   "services": ["svc1"],
        ///   "cpuUsage": 0.15
        /// }
        /// </code>
        /// </returns>
        Result<AppSystemInfoDto> GetEnvironmentInfo(int since = 60);

        /// <summary>
        /// Asynchronously gets detailed environment information.
        /// + Does not block the calling thread.
        /// - Incurred overhead from context switches.
        /// </summary>
        /// <param name="since">The number of minutes of event logs to include.</param>
        /// <returns>A task returning a <see cref="Result{T}"/> with the same payload as <see cref="GetEnvironmentInfo"/>.</returns>
        Task<Result<AppSystemInfoDto>> GetEnvironmentInfoAsync(int since = 60);

        /// <summary>
        /// Gets limited environment information.
        /// + Lightweight when full diagnostics are unnecessary.
        /// - Omits detailed metrics like event logs.
        /// </summary>
        /// <param name="since">The number of minutes of event logs to include.</param>
        /// <returns>
        /// A <see cref="Result{T}"/> containing an <see cref="AppSystemInfoDto"/> with reduced fields.
        /// </returns>
        Result<AppSystemInfoDto> GetLazyEnvironmentInfo(int since = 60);

        /// <summary>
        /// Asynchronously gets limited environment information.
        /// + Suitable for UI threads or web APIs.
        /// - Still limited in returned metrics.
        /// </summary>
        /// <param name="since">The number of minutes of event logs to include.</param>
        /// <returns>A task returning a <see cref="Result{T}"/> with reduced environment details.</returns>
        Task<Result<AppSystemInfoDto>> GetLazyEnvironmentInfoAsync(int since = 60);

        #endregion Methods
    }
}
