using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ark.Infrastructure.Info
{
    /// <summary>
    /// Default implementation of <see cref="IEnvironmentInfoProvider"/> that delegates
    /// to <see cref="EnvironmentInfoProvider"/>.
    /// + Simplifies consumption by exposing a single entry point.
    /// - Provides no additional caching or optimizations.
    /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.environment"/>
    /// </summary>
    public class DefaultEnvironmentInfoProvider : IEnvironmentInfoProvider
    {
        #region Methods

        /// <summary>
        /// Gets detailed information about the execution environment.
        /// + Synchronous and straightforward.
        /// - Performs immediate metric collection which may be slow on some hosts.
        /// </summary>
        /// <param name="since">The number of minutes of event logs to include.</param>
        /// <returns>
        /// A <see cref="Result{T}"/> containing an <see cref="AppSystemInfoDto"/>.
        /// </returns>
        public Result<AppSystemInfoDto> GetEnvironmentInfo(int since = 60)
            => Result<AppSystemInfoDto>.Success.WithData(EnvironmentInfoProvider.GetEnvironmentInfo(since));

        /// <summary>
        /// Asynchronously gets detailed information about the execution environment.
        /// + Non-blocking for responsive applications.
        /// - Spawns a task and may allocate additional threads.
        /// </summary>
        /// <param name="since">The number of minutes of event logs to include.</param>
        /// <returns>A task producing the same payload as <see cref="GetEnvironmentInfo"/>.</returns>
        public Task<Result<AppSystemInfoDto>> GetEnvironmentInfoAsync(int since = 60)
            => Task.Run(() => GetEnvironmentInfo(since));

        /// <summary>
        /// Gets partial information about the execution environment.
        /// + Faster than <see cref="GetEnvironmentInfo"/> when only minimal data is required.
        /// - Omits event logs and other costly metrics.
        /// </summary>
        /// <param name="since">The number of minutes of event logs to include.</param>
        /// <returns>
        /// A <see cref="Result{T}"/> with an <see cref="AppSystemInfoDto"/> that has a reduced set of fields.
        /// </returns>
        public Result<AppSystemInfoDto> GetLazyEnvironmentInfo(int since = 60)
            => Result<AppSystemInfoDto>.Success.WithData(EnvironmentInfoProvider.GetLazyEnvironmentInfo(since));

        /// <summary>
        /// Asynchronously gets partial information about the execution environment.
        /// + Offloads work to a background task.
        /// - Still gathers a limited set of metrics.
        /// </summary>
        /// <param name="since">The number of minutes of event logs to include.</param>
        /// <returns>A task producing the same reduced payload as <see cref="GetLazyEnvironmentInfo"/>.</returns>
        public Task<Result<AppSystemInfoDto>> GetLazyEnvironmentInfoAsync(int since = 60)
            => Task.Run(() => GetLazyEnvironmentInfo(since));

        #endregion Methods
    }
}
