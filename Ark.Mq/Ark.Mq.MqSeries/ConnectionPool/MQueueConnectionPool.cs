using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IBM.WMQ;

namespace Ark.Net.MqSeries
{
    /// <summary>
    /// These settings are used to connect an existing MQ Series queue using a connection pool.
    /// </summary>
    internal class MQueueConnectionPool
    {
        #region Fields

        /// <summary>
        /// The mutex used to synchronize the search of a connection.
        /// </summary>
        private readonly MutexAsync _mutex;

        /// <summary>
        /// The connections available in the pool.
        /// </summary>
        private readonly List<MQueueConnection> _connections;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Creates a <see cref="MQueueConnectionPool"/> instance.
        /// </summary>
        /// <param name="settings">The settings used to connect the MQ series server.</param>
        public MQueueConnectionPool(MQueueConnectionPoolSettings settings)
        {
            _mutex = new MutexAsync();

            _connections = new List<MQueueConnection>();
            for(var counter = 0; counter < settings.MaxSimultaneousConnections; counter++)
                _connections.Add(new MQueueConnection(settings));
        }

        #endregion Constructors
        
        #region Methods (Public)

        /// <summary>
        /// Execute an action on a queue given the first connection available.
        /// The action does not return any data (PUT request).
        /// </summary>
        /// <param name="queueAccessOptions">The options to access correctly the desired queue.</param>
        /// <param name="action">The action to execute on the accessed queue from a connection from the pool.</param>
        /// <returns>
        /// Success : The execution has succeeded.
        /// Timeout : No available connection could be used to open the queue in the given laps.
        /// NoConnection : No connection can be made to the MQ Series Server.
        /// Unexpected : An unexpected error occurs.
        /// ... and also all the results returned by the action itself.
        /// </returns>
        public async Task<Result> Execute(MQueueAccessOptions queueAccessOptions, Func<MQQueue, Result> action)
        {
            var connection = await GetFirstAvailableConnection(queueAccessOptions.AccessTimeout);
            if (connection == null)
                return Result.Timeout.WithReason(
                    $"Unable to get an available connection to connect the queue {queueAccessOptions.QueueKey}");

            var result = (Result) await connection.Execute(queueAccessOptions, queue => new Result<object>(action(queue)));
            return result;
        }

        /// <summary>
        /// Execute an action on a queue given the first connection available.
        /// The action return some data.
        /// </summary>
        /// <param name="queueAccessOptions">The options to access correctly the desired queue.</param>
        /// <param name="action">The action to execute on the accessed queue from a connection from the pool.</param>
        /// <returns>
        /// Success : The execution has succeeded.
        /// Timeout : No available connection could be used to open the queue in the given laps.
        /// BadParameters : The queue name has not been found in the settings given the key.
        /// NoConnection : No connection can be made to the MQ Series Server.
        /// Unexpected : An unexpected error occurs.
        /// ... and also all the results returned by the action itself.
        /// </returns>
        public async Task<Result<T>> Execute<T>(MQueueAccessOptions queueAccessOptions, Func<MQQueue, Result<T>> action)
        {
            var connection = await GetFirstAvailableConnection(queueAccessOptions.AccessTimeout);
            if (connection == null)
                return Result<T>.Timeout.WithReason($"Unable to get an available connection to connect the queue { queueAccessOptions.QueueKey }");

            var result = await connection.Execute(queueAccessOptions, action);
            return result;
        }

        #endregion Methods (Public)

        #region Methods (Helpers)

        /// <summary>
        /// Get the first available connection from the pool.
        /// The available connection will be prioritized with an already connected one.
        /// </summary>
        /// <remarks>
        /// The calls to this method are synchronized to avoid the same connection given to 2 callers.
        /// If no connection is available, all the callers will be blocked by the mutex.
        /// </remarks>
        /// <param name="timeoutSpan">The maximum time to get a connection until it times out.</param>
        /// <returns>The available and if possible connected connection or null if timeout.</returns>
        private ValueTask<MQueueConnection> GetFirstAvailableConnection(TimeSpan timeoutSpan) 
            => _mutex.WaitAndReleaseAsync(async () =>
            {
                var startTime = DateTime.UtcNow;
                do
                {
                    var connection = _connections.Where(c => c.IsAvailable).OrderByDescending(c => c.IsConnected).FirstOrDefault();
                    if (connection != null)
                        return connection;
                    await Task.Delay(1);
                } while (DateTime.UtcNow.Subtract(startTime) <= timeoutSpan);

                return null;
            });

        #endregion Methods (Helpers)
    }
}