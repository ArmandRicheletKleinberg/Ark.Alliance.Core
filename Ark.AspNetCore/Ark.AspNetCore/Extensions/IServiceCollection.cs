using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable UnusedMember.Global

namespace Ark.AspNetCore
{
    /// <summary>
    /// This class extend the <see cref="IServiceCollection"/> class.
    /// This contains a lot of useful ASP .NET Core features out of the box.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public static class IServiceCollectionExtensions
    {
        #region Nested Classes

        /// <summary>
        /// This helper generic class is used to add startup filters more easily.
        /// </summary>
        private class GenericStartupFilter : IStartupFilter
        {
            #region Fields

            /// <summary>
            /// The function to execute at application startup.
            /// </summary>
            private readonly Func<Action<IApplicationBuilder>, Action<IApplicationBuilder>> _startupFunction;

            #endregion Fields

            #region Constructors

            /// <summary>
            /// Creates a <see cref="GenericStartupFilter"/> instance.
            /// </summary>
            /// <param name="startupFunction"> The function to execute at application startup.</param>
            public GenericStartupFilter(Func<Action<IApplicationBuilder>, Action<IApplicationBuilder>> startupFunction)
            {
                _startupFunction = startupFunction;
            }

            #endregion Constructors

            #region IStartupFilter

            /// <inheritdoc />
            public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
                => _startupFunction(next);

            #endregion IStartupFilter
        }

        #endregion Nested Classes

        #region Methods (Public)

        /// <summary>
        /// Adds a function to execute on application startup.
        /// The next delegate is used to execute the other startup filter either before or after this code.
        /// Mainly used as syntactic sugar to avoid creating a IStartupFilter implemented class.
        /// Used it like next => app => { ...; next(app); return app; }.
        /// </summary>
        /// <param name="services">The services collection to add the startup filter to.</param>
        /// <param name="startupFunction">The function to execute at application startup.</param>
        public static void AddStartupFilter(this IServiceCollection services, Func<Action<IApplicationBuilder>, Action<IApplicationBuilder>> startupFunction)
            => services.AddTransient<IStartupFilter, GenericStartupFilter>(isp => new GenericStartupFilter(startupFunction));

        /// <summary>
        /// Registers default Ark.AspNetCore services such as MVC controllers and Swagger.
        /// </summary>
        /// <param name="services">The service collection to configure.</param>
        /// <returns>The configured service collection.</returns>
        public static IServiceCollection AddArkAspNetCore(this IServiceCollection services)
        {
            // TODO: extend this method with additional Ark.AspNetCore features when available
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                // Use fully qualified type names to avoid schema ID collisions
                c.CustomSchemaIds(type => type.FullName.Replace(".", "_"));
            });

            return services;
        }

        #endregion Methods (Public)
    }
}