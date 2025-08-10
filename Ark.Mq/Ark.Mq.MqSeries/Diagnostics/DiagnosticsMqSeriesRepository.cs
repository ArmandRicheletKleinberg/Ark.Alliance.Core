using Ark.Net.MqSeries;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ark.App.Diagnostics
{
    /// <inheritdoc />
    /// <summary>
    /// Ce repository est utilisé pour communiquer avec un serveur MQ Series.
    /// </summary>
    public class DiagnosticsMqSeriesRepository : MqSeriesRepositoryBase
    {
        #region Constructors

        /// <inheritdoc />
        /// <summary>
        /// Creates a <see cref="DiagnosticsMqSeriesRepository" /> instance.
        /// </summary>
        public DiagnosticsMqSeriesRepository(string connectionPoolName)
            : base(connectionPoolName)
        { }

        #endregion Constructors

        #region Methods (Public)

        /// <summary>
        /// Read the X First messages of the queue. 
        /// </summary>
        /// <param name="queueKey">The key of the queue with is linked to the queue name in the settings.</param>
        /// /// <param name="maxMessagesNumber">Number of messages to read.</param>
        /// <returns>
        /// Success : The execution has succeeded and the list of messages has been returner.
        /// BadPrerequisites : The connection pool name has not been found in the pre initialized settings.
        /// NotFound : No message was found in the queue event with the optional wait time.
        /// Timeout : No available connection could be used to open the queue in the given laps.
        /// NoConnection : No connection can be made to the MQ Series Server.
        /// Unexpected : An unexpected error occurs. 
        /// </returns>
        public new Task<Result<List<string>>> GetQueueXFirstMessages(string queueKey, int maxMessagesNumber)
            => base.GetQueueXFirstMessages(queueKey, maxMessagesNumber);

        #endregion Methods (Public)
    }
}
