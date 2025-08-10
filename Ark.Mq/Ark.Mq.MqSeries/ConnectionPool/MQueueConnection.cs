using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using IBM.WMQ;

namespace Ark.Net.MqSeries
{
    /// <summary>
    /// This manages the action done on a queue.
    /// It uses a queue manager used to connect the MQ server.
    /// It also synchronize the action to be executed using a mutex. Only one action should be executed at one time.
    /// </summary>
    internal class MQueueConnection
    {
        #region Fields

        /// <summary>
        /// The application pool settings used to connect.
        /// </summary>
        private readonly MQueueConnectionPoolSettings _settings;

        /// <summary>
        /// The queue manager  used to connect the MQ server.
        /// </summary>
        private MQQueueManager _queueManager;

        /// <summary>
        /// The mutex used to synchronize the action executed on the connection.
        /// </summary>
        private readonly MutexAsync _mutex;

        /// <summary>
        /// This timer is used to close a connection when a auto close delay is specified.
        /// </summary>
        private Timer _autoCloseTimer;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Creates a <see cref="MQueueConnection"/> instance given the pool settings.
        /// </summary>
        /// <param name="settings">The connection pool settings.</param>
        public MQueueConnection(MQueueConnectionPoolSettings settings)
        {
            _settings = settings;
            _mutex = new MutexAsync();
        }

        #endregion Constructors

        #region Properties (Public)

        /// <summary>
        /// Whether the queue is available to execute some actions.
        /// </summary>
        public bool IsAvailable => _mutex.CurrentCount > 0;

        /// <summary>
        /// Whether the queue is currently connected.
        /// </summary>
        public bool IsConnected => _queueManager?.IsConnected ?? false;

        #endregion Properties (Public)

        #region Methods (Override)

        /// <summary>
        /// Executes an action on a queue.
        /// It first accesses the queue given the access options then the action is executed and then the queue is properly closed.
        /// </summary>
        /// <typeparam name="T">The type of the action result.</typeparam>
        /// <param name="accessOptions">The options needed to access the queue.</param>
        /// <param name="action">The action to execute on the accessed queue.</param>
        /// <returns>
        /// Success : The execution has succeeded.
        /// Timeout : No available connection could be used to open the queue in the given laps.
        /// BadParameters : The queue name has not been found in the settings given the key.
        /// NoConnection : No connection can be made to the MQ Series Server.
        /// Unexpected : An unexpected error occurs.
        /// ... and also all the results returned by the action itself.
        /// </returns>
        public ValueTask<Result<T>> Execute<T>(MQueueAccessOptions accessOptions, Func<MQQueue, Result<T>> action)
        {
            return _mutex.WaitAndReleaseAsync(async () =>
            {
                MQQueue queue = null;
                try
                {
                    _autoCloseTimer?.Dispose();
                    _autoCloseTimer = null;

                    await CreateOrGetQueueManager();

                    var accessQueueResult = await AccessQueue(accessOptions);
                    if (accessQueueResult.IsNotSuccess)
                        return new Result<T>(accessQueueResult);
                    queue = accessQueueResult.Data;

                    var result = action(queue);

                    ManageAutoClose();

                    return result;
                }
                catch (Exception exception)
                {
                    return ManageException<T>(exception);
                }
                finally
                {
                    queue?.Close();
                }
            });
        }

        #endregion Methods (Override)

        #region Methods (Helpers)

        /// <summary>
        /// Creates or get the queue manager if already created and connected.
        /// </summary>
        /// <returns>The connected queue manager ready to serve.</returns>
        private Task<MQQueueManager> CreateOrGetQueueManager() => Task.Run(() =>
        {
            if (_queueManager?.IsConnected ?? false)
                return _queueManager;

            var properties = new Hashtable
            {
                {MQC.TRANSPORT_PROPERTY, MQC.TRANSPORT_MQSERIES_MANAGED},
                {MQC.HOST_NAME_PROPERTY, _settings.HostName},
                {MQC.PORT_PROPERTY, _settings.HostPort},
                {MQC.CHANNEL_PROPERTY, _settings.ChannelName}
            };
            _queueManager = new MQQueueManager(_settings.QueueManagerName, properties);

            return _queueManager;
        });

        /// <summary>
        /// Accesses a queue using the manager given some options.
        /// The queue real name will be found using the queues defined in the settings with the given key.
        /// </summary>
        /// <param name="accessOptions">The options to access the queue such as the queue key, permissions.</param>
        /// <returns>
        /// Success : The queue has been accessed and is returned.
        /// BadParameters : The queue name has not been found in the settings given the key.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        private Task<Result<MQQueue>> AccessQueue(MQueueAccessOptions accessOptions) => Task.Run(() =>
        {
            try
            {
                var queueName = _settings.Queues.GetValue(accessOptions.QueueKey);
                if (queueName == null)
                    return Result<MQQueue>.BadParameters.WithReason($"The queue key {accessOptions.QueueKey} has not been found in the settings.");

                var queue = _queueManager.AccessQueue(queueName, accessOptions.GetQueueAccessOptions());
                return new Result<MQQueue>(queue);
            }
            catch (MQException exception)
            {
                _queueManager = null;
                return new Result<MQQueue>(exception);
            }
        });

        /// <summary>
        /// Manage the auto close feature of the connection.
        /// If no auto close delay is set then does nothing.
        /// If the timer is already running then it is reset to extend the close time.
        /// If the timer is not already running then creates one with the auto close delay.
        /// Once the timer expires then the queue manager is disconnected and disposed as well as the timer.
        /// </summary>
        private void ManageAutoClose()
        {
            if (_settings.ConnectionAutoCloseDelay == null)
                return;

            if (_settings.ConnectionAutoCloseDelay == 0)
            {
                Disconnect();
                return;
            }


            if (_autoCloseTimer != null)
            {
                _autoCloseTimer.Change(_settings.ConnectionAutoCloseDelay.Value, -1);
                return;
            }

            _autoCloseTimer = new Timer(state => _mutex.WaitAndRelease(Disconnect), null, _settings.ConnectionAutoCloseDelay.Value, -1);
        }

        /// <summary>
        /// Disconnects from the queue manager.
        /// Synchronous.
        /// </summary>
        private void Disconnect()
        {
            Debug.WriteLine($@"{DateTime.Now:O} : Disconnected");
            _queueManager?.Disconnect();
            _queueManager = null;
            _autoCloseTimer?.Dispose();
            _autoCloseTimer = null;
        }

        /// <summary>
        /// Manages the exception that can occur during the execution.
        /// This will refine the error code to the more representative result.
        /// </summary>
        /// <typeparam name="T">The type fo the object to return.</typeparam>
        /// <param name="exception">The exception to interpret.</param>
        /// <returns>The result interpreted by the exception received.</returns>
        private static Result<T> ManageException<T>(Exception exception)
        {
            if (!(exception is MQException mqException))
                return new Result<T>(exception);

            var reason = $"MQException - {mqException.ReasonCode} - {mqException.Reason}";
            if (mqException.ReasonCode == 2033)
                return Result<T>.NotFound.WithException(mqException).WithReason(reason);

            return new Result<T>(exception).WithReason(reason);
        }

        #endregion Methods (Helpers)
    }
}