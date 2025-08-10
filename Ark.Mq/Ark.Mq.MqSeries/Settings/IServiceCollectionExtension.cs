using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ark.Net.MqSeries
{
    /// <summary>
    /// This is an extension class to add MQ Series support for a .NET Core app.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public static class IServiceCollectionExtension
    {
        #region Static methods

        /// <summary>
        /// Adds the MQ Series support to the app.
        /// It mainly initializes the connection pools used by the MQSeriesRepositoryBase repositories.
        /// </summary>
        /// <param name="services">The services to add the use of the MQ Series service to the app.</param>
        /// <param name="configuration">The configuration to search for MQ Series applications pool settings.</param>
        /// <param name="sectionKey">The key of the app settings section to get the connection pools settings from. It must be in the root of the settings.</param>
        public static void AddMqSeries(this IServiceCollection services, IConfiguration configuration, string sectionKey = "MQSeriesConnectionPools")
        {
            var connectionPoolsSettings = configuration.GetSection(sectionKey).Get<Dictionary<string, MQueueConnectionPoolSettings>>();
            connectionPoolsSettings?.ForEach(cp => cp.Value.ConnectionPoolName ??= cp.Key);
            MqSeriesRepositoryBase.Initialize(connectionPoolsSettings?.Values.ToArray() ?? new MQueueConnectionPoolSettings[0]);
        }

        /// <summary>
        /// Adds the MQ Series support to the app.
        /// It mainly initializes the connection pools used by the MQSeriesRepositoryBase repositories.
        /// </summary>
        /// <param name="services">The services to add the use of the MQ Series service to the app.</param>
        /// <param name="connectionPoolsSettings">The connection pools to use by the MQ Series repositories.</param>
        public static void AddMqSeries(this IServiceCollection services, MQueueConnectionPoolSettings[] connectionPoolsSettings)
        {
            MqSeriesRepositoryBase.Initialize(connectionPoolsSettings);
        }

        #endregion Static methods
    }
}