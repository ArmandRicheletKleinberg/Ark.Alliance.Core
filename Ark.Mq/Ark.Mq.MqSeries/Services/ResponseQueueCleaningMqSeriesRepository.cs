using System;
using System.Threading.Tasks;

namespace Ark.Net.MqSeries
{
    /// <inheritdoc />
    /// <summary>
    /// This repository is specific for the response queue cleaning service.
    /// </summary>
    internal class ResponseQueueCleaningMqSeriesRepository : MqSeriesRepositoryBase
    {
        #region Constructors

        /// <inheritdoc />
        public ResponseQueueCleaningMqSeriesRepository(string connectionPoolName, TimeSpan queueAccessTimeout = default) 
            : base(connectionPoolName, queueAccessTimeout)
        { }

        #endregion Constructors

        #region Methods (Public)

        /// <summary>
        /// Cleans some old messages to clean up a queue.
        /// </summary>
        /// <param name="queueKey">The key of the queue to clean.</param>
        /// <param name="validityTimeSpan">The timespan during which the message is still valid and should not be deleted.</param>
        /// <returns>
        /// Success : The queue has been cleaned.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public new Task<Result> CleanOldMessages(string queueKey, TimeSpan validityTimeSpan)
            => base.CleanOldMessages(queueKey, validityTimeSpan);

        #endregion Methods (Public)
    }
}