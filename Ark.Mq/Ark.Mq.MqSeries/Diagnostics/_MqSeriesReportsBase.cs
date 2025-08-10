using System.Collections.Generic;
using System.Threading.Tasks;
using Ark.App.Diagnostics;

namespace Ark.Net.MqSeries
{
    /// <summary>
    /// This class is the base class used to access to the diagnostics reports.
    /// The reports can return some data useful to diagnose problems.
    /// A report is simply a method that returns a Task{Result{}} with no parameters.
    /// The report name will be simply the method name.
    /// </summary>
    /// <example>
    /// public async Task{Result{string[]}} ConnectedUsers() => Task.Run(() => new [] { "John Smith" });
    /// </example>
    public abstract class MqSeriesReportsBase : ReportsBase
    {
        #region Methods (Helpers)

        /// <summary>
        /// Get the messages from a defined queue in a defined connection pool.
        /// Extracts the messages up to a maximum number due to performance purpose.
        /// </summary>
        /// <param name="connectionPoolName">The name of the connection pool to use (set in the app.settings).</param>
        /// <param name="queueKey">The name of the queue to use (set in the app.settings).</param>
        /// <param name="maxMessagesNumber">The maximum number of messages to return.</param>
        /// <returns>
        /// Success : The execution has succeeded and the data of raw string messages is returned.
        /// BadPrerequisites : The connection pool name has not been found in the pre initialized settings.
        /// NotFound : No message was found in the queue event with the optional wait time.
        /// Timeout : No available connection could be used to open the queue in the given laps.
        /// NoConnection : No connection can be made to the MQ Series Server.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        protected async Task<Result<List<string>>> GetReportQueueMessages(string connectionPoolName, string queueKey, int maxMessagesNumber)
        {
            var diagnosticsMqSeriesRepository = new DiagnosticsMqSeriesRepository(connectionPoolName);
            var resultGetMessages = await diagnosticsMqSeriesRepository.GetQueueXFirstMessages(queueKey, maxMessagesNumber);

            if (resultGetMessages.IsNotSuccess)
                return new Result<List<string>>(resultGetMessages);

            return new Result<List<string>>(resultGetMessages.Data);
        }

        #endregion Methods (Helpers)
    }
}