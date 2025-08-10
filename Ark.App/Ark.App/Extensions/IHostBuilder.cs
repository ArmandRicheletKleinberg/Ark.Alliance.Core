using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Ark;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace Ark.App
{
    /// <summary>
    /// Convenience extensions for <see cref="IHostBuilder"/> that configure the
    /// host using conventions employed across Ark projects. These helpers
    /// register injectable services, load configuration, set default culture and
    /// wire up logging.
    ///
    /// Using these methods saves several lines of boilerplate in each
    /// application. Overhead is minimal and occurs only during host start.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public static class IHostBuilderExtensions
    {
        #region Methods (Public)

        /// <summary>
        /// Injects automatically all the classes from assembly which has the <see cref="InjectableAttribute"/> attribute.
        /// If no assembly is provided then the entry assembly will be used instead.
        /// </summary>
        /// <param name="builder">The host builder.</param>
        /// <param name="assembly">The assembly to search for injectable classes.</param>
        /// <returns>The host builder to chain.</returns>
        public static IHostBuilder UseInjectables(this IHostBuilder builder, Assembly assembly = null)
            => builder.ConfigureServices((context, services) =>
            {
                assembly ??= Assembly.GetEntryAssembly();
                if (assembly == null)
                    return;

                assembly.GetTypes().ForEach(type =>
                {
                    var attribute = type.GetCustomAttribute<InjectableAttribute>();
                    if (attribute == null)
                        return;

                    switch (attribute.ServiceLifetime)
                    {
                        case ServiceLifetimeEnum.Transient: services.AddTransient(type); break;
                        case ServiceLifetimeEnum.Scoped: services.AddScoped(type); break;
                        case ServiceLifetimeEnum.Singleton: services.AddSingleton(type); break;
                    }
                });
            });

        /// <summary>
        /// Configure the app configuration by adding the app settings JSON files along with the environments variable to the app configuration.
        /// </summary>
        /// <remarks>
        /// This will not work on IIS Express unless appsettings.* files are copied into the output directory
        /// as the path is the AppDomain.CurrentDomain.BaseDirectory.
        /// </remarks>
        /// <param name="builder">The host builder.</param>
        /// <returns>The same builder in order to chain with the configuration.</returns>
        public static IHostBuilder UseAppConfiguration(this IHostBuilder builder)
            => builder.ConfigureAppConfiguration((context, configurationBuilder) =>
            {
                Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", EnvironmentHelper.Current.ToString());
                configurationBuilder
                    .SetBasePath(context.HostingEnvironment.ContentRootPath)
                    .AddJsonFile("appsettings.json", true, true)
                    .AddJsonFile($"appsettings.{EnvironmentHelper.Current}.json", true, true)
                    .AddEnvironmentVariables();
            });

        /// <summary>
        /// Sets the default application culture.
        /// By default the culture is the culture on the running computer.
        /// </summary>
        /// <param name="builder">The host builder.</param>
        /// <param name="defaultCulture">The default culture to apply.</param>
        /// <returns>The same builder in order to chain.</returns>
        public static IHostBuilder UseDefaultCulture(this IHostBuilder builder, string defaultCulture)
        {
            var culture = new CultureInfo(defaultCulture);
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            return builder;
        }

        /// <summary>
        /// Adds and uses a single hosted service.
        /// The settings of the hosted service must be set in the application configuration.
        /// </summary>
        /// <param name="builder">The host builder to add the hosted the hosted service.</param>
        /// <returns>The same builder in order to chain.</returns>
        public static IHostBuilder UseHostedService<THostedService>(this IHostBuilder builder)
            where THostedService : HostedService
            => builder.ConfigureServices((context, services) => services.AddHostedService<THostedService>());

        /// <summary>
        /// Configures application logging based on the <c>Logging</c> section of the configuration.
        /// <para>+ Centralizes provider setup and respects per-provider <c>IsEnabled</c> flags.</para>
        /// <para>- The EventLog provider is only available on Windows.</para>
        /// <para>Ref: <see href="https://learn.microsoft.com/dotnet/core/extensions/logging">Logging in .NET</see></para>
        /// </summary>
        /// <param name="builder">The builder used to build the host.</param>
        /// <returns>The same builder to chain.</returns>
        public static IHostBuilder UseLogging(this IHostBuilder builder)
            => builder
                .ConfigureLogging((context, logging) =>
                {
                    logging.AddConfiguration(context.Configuration.GetSection("Logging"));
                    var loggersTypes = context.Configuration.GetSection("Logging").GetChildren()
                        .Where(c => c.GetValue<bool?>("IsEnabled") ?? true)
                        .Select(s => s.Key)
                        .ToHashSet();

                    logging.ClearProviders();
                    if (loggersTypes.Contains("Console"))
                        logging.AddConsole();

                    if (loggersTypes.Contains("Debug"))
                        logging.AddDebug();

                    if (loggersTypes.Contains("EventLog") && OperatingSystem.IsWindows())
                        logging.AddEventLog();

                });

        #endregion Methods (Public)
    }
}