using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Ark.App.Diagnostics
{
    /// <summary>
    /// Extensions for adding the <see cref="FileLoggerProvider" /> to the <see cref="ILoggingBuilder" />
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public static class ILoggingBuilderExtensions
    {
        /// <summary>
        /// Adds a file logger named 'File' to the factory.
        /// </summary>
        /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
        public static ILoggingBuilder AddFile(this ILoggingBuilder builder)
        {
            builder.Services.AddSingleton<ILoggerProvider, FileLoggerProvider>();
            return builder;
        }

        /// <summary>
        /// Adds a file logger named 'File' to the factory.
        /// </summary>
        /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
        /// <param name="options">Configure an instance of the <see cref="FileLoggerOptions" /> to set logging options</param>
        public static ILoggingBuilder AddFile(this ILoggingBuilder builder, Action<FileLoggerOptions> options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            builder.AddFile();
            builder.Services.Configure(options);

            return builder;
        }

        /// <summary>
        /// Allows to log to a SQL SERVER database.
        /// </summary>
        /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
        /// <param name="configuration"></param>
        /// <param name="sqlServerConnectionString"></param>
        public static ILoggingBuilder AddSqlServer(this ILoggingBuilder builder, IConfiguration configuration, string sqlServerConnectionString = null)
        {
            var settings = configuration.GetSection("Logging:SqlServer").Get<SqlServerLoggerOptions>();
            settings.SqlServerConnectionString = sqlServerConnectionString ?? settings.SqlServerConnectionString;
            builder.Services.Configure<SqlServerLoggerOptions>(options =>
            {
                options.IsEnabled = settings.IsEnabled;
                options.BackgroundQueueSize = settings.BackgroundQueueSize;
                options.BatchSize = settings.BatchSize;
                options.FlushPeriod = settings.FlushPeriod;
                options.SqlServerConnectionString = settings.SqlServerConnectionString;
                options.SqlServerTableName = settings.SqlServerTableName;
            });
            if (settings.IsEnabled)
            {
                var logDbServices = new LogDbServices(settings.SqlServerConnectionString, settings.SqlServerTableName);
                // ensures the logs table exists at startup
                logDbServices.CreateLogsTableIfNotExists().ConfigureAwait(false).GetAwaiter().GetResult();
            }
            builder.Services.AddSingleton<ILoggerProvider, SqlServerLoggerProvider>();
            return builder;
        }

        /// <summary>
        /// Allows to log to a SQL SERVER database.
        /// </summary>
        /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
        /// <param name="options">Configure an instance of the <see cref="FileLoggerOptions" /> to set logging options</param>
        public static ILoggingBuilder AddSqlServer(this ILoggingBuilder builder, Action<SqlServerLoggerOptions> options)
        {
            builder.Services.Configure(options);
            builder.Services.AddSingleton<ILoggerProvider, SqlServerLoggerProvider>();
            return builder;
        }
    }
}