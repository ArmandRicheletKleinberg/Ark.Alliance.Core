using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace Ark.AspNetCore
{
    /// <summary>
    /// This class is used as an helper to setup quickly a default MVC implementation as a Middleware.
    /// </summary>
    internal class MvcStartupFilter : IStartupFilter
    {
        #region IStartupFilter

        /// <inheritdoc />
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return builder =>
            {
                next(builder);
                if (EnvironmentHelper.IsEnvironment(EnvironmentEnum.Debug, EnvironmentEnum.Dev))
                    builder.UseDeveloperExceptionPage();

                builder
                    .UseRouting()
                    .UseAuthentication()
                    .UseAuthorization()
                    .UseEndpoints(endpoints => endpoints.MapControllers());
            };
        }

        #endregion IStartupFilter
    }
}