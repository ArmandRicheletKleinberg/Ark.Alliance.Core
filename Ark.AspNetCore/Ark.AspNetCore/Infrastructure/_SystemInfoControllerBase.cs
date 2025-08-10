#nullable enable

using System.Collections.Generic;
using System.Threading.Tasks;
using Ark.Infrastructure.Info;
using Ark.Net.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Ark.AspNetCore
{
    /// <summary>
    /// Base controller exposing system information APIs.
    /// </summary>
    /// <typeparam name="TUserProfileData">The type of the user profile data.</typeparam>
    [ApiExplorerSettings(GroupName = "ο System")]
    public abstract class SystemInfoControllerBase<TUserProfileData> : ControllerBase<TUserProfileData>
        where TUserProfileData : new()
    {
        #region Methods (Public)

        /// <summary>
        /// Gets the services information for the current machine.
        /// </summary>
        /// <param name="namePattern">Optional pattern the service name must start with.</param>
        /// <param name="publisher">Optional publisher contained in the executable.</param>
        /// <param name="eventLogMinutes">Time span in minutes for returned event logs.</param>
        /// <returns>The services information.</returns>
        [HttpGet("system/services")]
        public Task<ResultDto<List<DetailedServiceInfoDto>>> GetServices(string? namePattern = null, string? publisher = null, int eventLogMinutes = 60)
            => ExecuteBlAsync(() => new SystemInfoRepository().GetServicesAsync(namePattern, publisher, eventLogMinutes));

        #endregion Methods (Public)
    }


}
