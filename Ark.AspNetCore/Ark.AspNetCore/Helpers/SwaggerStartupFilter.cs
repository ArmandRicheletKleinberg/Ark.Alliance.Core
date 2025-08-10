using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.OpenApi.Models;

namespace Ark.AspNetCore
{
    /// <summary>
    /// This class is used as an helper to setup quickly a default Swagger implementation as a Middleware.
    /// </summary>
    internal class SwaggerStartupFilter : IStartupFilter
    {
        #region Fields

        /// <summary>
        /// The swagger document info to display in the Swagger UI.
        /// </summary>
        private readonly OpenApiInfo _info;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Creates a <see cref="SwaggerStartupFilter"/> instance.
        /// </summary>
        /// <param name="info">The swagger document info to display in the Swagger UI.</param>
        public SwaggerStartupFilter(OpenApiInfo info)
        {
            _info = info;
        }

        #endregion Constructors

        #region IStartupFilter

        /// <inheritdoc />
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return builder =>
            {
                builder.UseSwagger();
                builder.UseSwaggerUI(c => { c.SwaggerEndpoint($"v{_info.Version}/swagger.json", $"{_info.Title} V{_info.Version}"); });
                next(builder);
            };
        }

        #endregion IStartupFilter
    }
}