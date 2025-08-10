using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedTypeParameter

namespace Ark.Data.EFCore
{
    /// <summary>
    /// This class extend the <see cref="IHostBuilder"/> class.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public static class IHostBuilderExtensions
    {
        #region Methods (UseDatabase)

        /// <summary>
        /// Use a database in the application given the type of its context and the configuration key to search the options in the app settings.
        /// If no configuration key is set, then it takes the name of the context type where it removes the "DbContext" suffix.
        /// The <see cref="DbServices{TEntity,TContext}"/> are set in the services collection as transient as well as the <typeparamref name="TContext"/>.
        /// Optionally the <see cref="SettingsDbServices{TContext,TEnum}"/> are also set in the services collection as transient also.
        /// Optionally, a migration can be done synchronously depending on the options.
        /// </summary>
        /// <typeparam name="TContext">The type of the context of the database to use.</typeparam>
        /// <param name="builder">The host builder.</param>
        /// <param name="configurationKey">
        /// The configuration key to search the options in the app settings.
        /// If no configuration key is set, then it takes the name of the context type where it removes the "DbContext" suffix.
        /// </param>
        /// <returns>The same host builder to chain.</returns>
        public static IHostBuilder UseDatabase<TContext>(this IHostBuilder builder, string configurationKey = null)
            where TContext : DbContextEx, new()
            => builder.ConfigureServices((context, services) =>
            {
                configurationKey ??= typeof(TContext).Name.Replace(nameof(DbContext), "");
                var options = context.Configuration.GetSection($"Databases:{configurationKey}").Get<DatabaseOptions>();
                if (options == null)
                    throw new Exception($"No settings have been found in the configuration section Databases:{configurationKey} for db context {typeof(TContext)}");

                RegisterServices<TContext>(options, services);
            });

        /// <summary>
        /// Use a database in the application given the type of its context and and hardcoded configuration.
        /// The <see cref="DbServices{TEntity,TContext}"/> are set in the services collection as transient as well as the <typeparamref name="TContext"/>.
        /// Optionally the <see cref="SettingsDbServices{TContext,TEnum}"/> are also set in the services collection as transient also.
        /// Optionally, a migration can be done synchronously depending on the options.
        /// </summary>
        /// <typeparam name="TContext">The type of the context of the database to use.</typeparam>
        /// <param name="builder">The host builder.</param>
        /// <param name="configure">The hardcoded configuration.</param>
        /// <returns>The same host builder to chain.</returns>
        public static IHostBuilder UseDatabase<TContext>(this IHostBuilder builder, Action<DatabaseOptions> configure)
            where TContext : DbContextEx, new()
            => builder.ConfigureServices((context, services) =>
            {
                var options = new DatabaseOptions();
                configure(options);

                RegisterServices<TContext>(options, services);
            });

        /// <summary>
        /// Register the needs services.
        /// The <see cref="DbServices{TEntity,TContext}"/> are set in the services collection as transient as well as the <typeparamref name="TContext"/>.
        /// Optionally the <see cref="SettingsDbServices{TContext,TEnum}"/> are also set in the services collection as transient also.
        /// Optionally, a migration can be done synchronously depending on the options.
        /// </summary>
        /// <typeparam name="TContext">The type of the context of the database to use.</typeparam>
        /// <param name="options">The database options.</param>
        /// <param name="services">The services collection to register the services.</param>
        private static void RegisterServices<TContext>(DatabaseOptions options, IServiceCollection services)
            where TContext : DbContextEx, new()
        {
            if (options.ConnectionString == null)
                throw new Exception($"The connection string is mandatory in the options for the db context {typeof(TContext)}");

            DbContextEx.OptionsByType.AddOrUpdate(typeof(TContext), options);

            services.AddTransient(typeof(DbServices<,>));
            services.AddDbContext<TContext>(ServiceLifetime.Transient);

            if (options.UseSettingsTable)
                services.AddTransient(typeof(SettingsDbServices<,>));


            if (options.Migrate)
                new TContext().Database.Migrate();

        }

        #endregion Methods (UseDatabase)
    }
}