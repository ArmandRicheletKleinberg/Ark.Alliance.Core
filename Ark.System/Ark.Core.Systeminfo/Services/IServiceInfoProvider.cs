
using Ark;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ark.Infrastructure.Info
{
    /// <summary>
    /// Provides an abstraction to query service information.
    /// + Enables mocking and testability for service queries.
    /// - Implementations may rely on platform-specific APIs.
    /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.serviceprocess.servicecontroller"/>.
    /// </summary>
    public interface IServiceInfoProvider
    {
        /// <summary>
        /// Gets detailed information about installed services.
        /// + Returns CPU, memory and log data when supported by the platform.
        /// - May require administrative rights on Windows.
        /// </summary>
        /// <param name="namePattern">Optional pattern the service name must start with.</param>
        /// <param name="publisher">Optional publisher contained in the executable.</param>
        /// <param name="eventLogMinutes">Time span in minutes for returned event logs.</param>
        /// <returns>A <see cref="Result{T}"/> containing the matching services.</returns>
        Result<List<DetailedServiceInfoDto>> GetServices(string? namePattern = null, string? publisher = null, int eventLogMinutes = 60);

        /// <summary>
        /// Asynchronously gets detailed information about installed services.
        /// + Uses <see cref="Task"/> to avoid blocking callers.
        /// - Still subject to the same permissions as <see cref="GetServices"/>.
        /// </summary>
        /// <param name="namePattern">Optional pattern the service name must start with.</param>
        /// <param name="publisher">Optional publisher contained in the executable.</param>
        /// <param name="eventLogMinutes">Time span in minutes for returned event logs.</param>
        /// <returns>A task returning a <see cref="Result{T}"/> containing the matching services.</returns>
        Task<Result<List<DetailedServiceInfoDto>>> GetServicesAsync(string? namePattern = null, string? publisher = null, int eventLogMinutes = 60);
    }
}
