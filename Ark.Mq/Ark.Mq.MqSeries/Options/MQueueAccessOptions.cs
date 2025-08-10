using System;
using IBM.WMQ;

namespace Ark.Net.MqSeries
{
    /// <summary>
    /// These options are used to access and to open a queue.
    /// </summary>
    public class MQueueAccessOptions
    {
        #region Constructors

        /// <summary>
        /// Creates a <see cref="MQueueAccessOptions"/> instance.
        /// </summary>
        /// <param name="queueKey">The key of the queue with is linked to the queue name in the settings. Mandatory to access a queue.</param>
        /// <param name="accessTimeout">The maximum duration to access a queue from the connection pool. Default to 5 minutes.</param>
        public MQueueAccessOptions(string queueKey, TimeSpan accessTimeout = default)
        {
            QueueKey = queueKey;
            AccessTimeout = accessTimeout != default ? accessTimeout : TimeSpan.FromMinutes(5);
        }

        #endregion Constructors

        #region Properties (Public)

        /// <summary>
        /// The key of the queue with is linked to the queue name in the settings.
        /// Mandatory to access a queue.
        /// </summary>
        public string QueueKey { get; set; }

        /// <summary>
        /// Whether the queue as the access to get messages from the queue.
        /// If you want to allow input messages then set as QueueDefault to respect the server setting.
        /// The Exclusive mode is useful to lock exclusively a queue in multi threaded environments.
        /// Default to None.
        /// </summary>
        public MQueueAccessInputModeEnum InputModeModeAllowed { get; set; }

        /// <summary>
        /// Whether the queue can be browsed.
        /// The browsing is needed to read messages without removing it from the queue.
        /// Default to false.
        /// </summary>
        public bool BrowsingAllowed { get; set; }

        /// <summary>
        /// Whether the queue can be inquired.
        /// The browsing is needed to inquire the queues to get some messages info.
        /// Default to false.
        /// </summary>
        public bool InquiringAllowed { get; set; }

        /// <summary>
        /// Whether the queue has the access to put messages on the queue.
        /// Default to false.
        /// </summary>
        public bool OutputAllowed { get; set; }

        /// <summary>
        /// The queue access fails with an exception when the queue manager is quiescing (stopping).
        /// It is better to leave this parameter as true to avoid blocking the quiescing of the queue manager.
        /// But it can be useful to force the access for some very important tasks.
        /// Default to true.
        /// </summary>
        public bool FailIfQuiescing { get; set; } = true;

        /// <summary>
        /// The maximum duration to access a queue from the connection pool.
        /// Default to 5 minutes.
        /// </summary>
        public TimeSpan AccessTimeout { get; set; }

        #endregion Properties (Public)

        #region Methods (Internal)

        /// <summary>
        /// Extract from the options the MQ Open Options needed to access the queue.
        /// </summary>
        /// <returns>The IBM MQ Open Options computed from the parameters.</returns>
        internal int GetQueueAccessOptions()
        {
            var options = 0;
            switch (InputModeModeAllowed)
            {
                case MQueueAccessInputModeEnum.QueueDefault: options += MQC.MQOO_INPUT_AS_Q_DEF; break;
                case MQueueAccessInputModeEnum.Shared: options += MQC.MQOO_INPUT_SHARED; break;
                case MQueueAccessInputModeEnum.Exclusive: options += MQC.MQOO_INPUT_EXCLUSIVE; break;
                    
              
            }
            if (BrowsingAllowed)
                options += MQC.MQOO_BROWSE;
            if (InquiringAllowed)
                options += MQC.MQOO_INQUIRE + MQC.MQOO_BIND_AS_Q_DEF;
            if (OutputAllowed)
                options += MQC.MQOO_OUTPUT;
            if (FailIfQuiescing)
                options += MQC.MQOO_FAIL_IF_QUIESCING;

            return options;
        }

        #endregion Methods (Internal)
    }
}