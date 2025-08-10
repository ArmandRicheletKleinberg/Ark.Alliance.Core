using System.Collections.Generic;

namespace Ark.Net.MqSeries
{
    /// <summary>
    /// These settings are used to connect an existing MQ Series queue using a connection pool.
    /// </summary>
    public class MQueueConnectionPoolSettings
    {
        #region Properties (Public)

        /// <summary>
        /// The name of the connection pool
        /// It is used in the repository to select which queue connection use.
        /// </summary>
        public string ConnectionPoolName { get; set; }

        /// <summary>
        /// The name of the queue manager top use.
        /// </summary>
        public string QueueManagerName { get; set; }
        
        /// <summary>
        /// The name of the host server to connect
        /// </summary>
        public string HostName { get; set; }

        /// <summary>
        /// The port of the host server to connect.
        /// Default to 1414.
        /// </summary>
        public int HostPort { get; set; } = 1414;

        /// <summary>
        /// The name of the channel to connect.
        /// </summary>
        public string ChannelName { get; set; }

        /// <summary>
        /// The key/names of the queue to connect.
        /// Used to associate an identifier key to a real queue name coming from the settings.
        /// </summary>
        public Dictionary<string, string> Queues { get; set; }

        /// <summary>
        /// The maximum simultaneous connections allowed.
        /// Default to only one simultaneous connection to access the repository.
        /// </summary>
        public int MaxSimultaneousConnections { get; set; } = 1;

        /// <summary>
        /// If different from null then is is the delay in ms after the last action to wait before closing an existing connection.
        /// If 0 then the connection is closed after each action.
        /// Default to null, no auto close.
        /// </summary>
        public int? ConnectionAutoCloseDelay { get; set; } = null;

        #endregion Properties (Public)
    }
}