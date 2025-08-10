using System;
using System.Threading;
using System.Threading.Tasks;
using Ark.App;
using Ark;
using Ark.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Ark.Net.MqSeries
{
    /// <inheritdoc />
    /// <summary>
    /// Service permettant de recevoir et de traiter les message d'avis ou de matières provenant du MainFrame.
    /// </summary>
    public abstract class MqSeriesQueueCleaningService : ScheduledHostedService<MqSeriesQueueCleaningService.DataSettings>
    {
        #region Nested Classes

        /// <summary>
        /// The settings for this service.
        /// </summary>
        public class DataSettings
        {
            #region Properties (Public)

            /// <summary>
            /// The connection pool name.
            /// </summary>
            public string ConnectionPoolName { get; set; }

            /// <summary>
            /// The queue key.
            /// </summary>
            public string QueueKey { get; set; }

            /// <summary>
            /// The validity time span for the messages before cleaning them.
            /// </summary>
            public TimeSpan ValidityTimeSpan { get; set; } = TimeSpan.FromMinutes(5);

            #endregion Properties (Public)
        }

        #endregion Nested Classes

        #region Constructors

        /// <summary>
        /// Creates a <see cref="ScheduledHostedService"/> instance.
        /// </summary>
        /// <param name="configuration">The application configuration. Injected.</param>
        /// <param name="logger">The logger used to log the service executions and errors.</param>
        protected MqSeriesQueueCleaningService(IConfiguration configuration, ILogger logger = null)
            : base(configuration, logger)
        { }

        #endregion Constructors

        #region Fields

        /// <summary>
        /// The MQ series repository is needed to clean the queue.
        /// </summary>
        internal ResponseQueueCleaningMqSeriesRepository ResponseQueueCleaningMqSeriesRepository;

        #endregion Fields

        #region Methods (Override)

        /// <inheritdoc />
        protected override Task Execute(CancellationToken cancellationToken)
            => (ResponseQueueCleaningMqSeriesRepository = ResponseQueueCleaningMqSeriesRepository ?? new ResponseQueueCleaningMqSeriesRepository(Data.ConnectionPoolName))
                .CleanOldMessages(Data.QueueKey, Data.ValidityTimeSpan);

        #endregion Methods (Override)
    }
}