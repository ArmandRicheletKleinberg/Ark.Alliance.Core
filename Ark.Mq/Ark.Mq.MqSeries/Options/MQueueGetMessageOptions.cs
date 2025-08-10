using System;
using IBM.WMQ;

namespace Ark.Net.MqSeries
{
    /// <summary>
    /// These options are used to get/browse a message from the queue.
    /// </summary>
    public class MQueueGetMessageOptions
    {
        #region Properties (Public)

        /// <summary>
        /// The interval of time to wait to get the message from the queue.
        /// If 0 then no wait. If no corresponding message is there, an exception is thrown.
        /// BEWARE ! this will keep a connection active during this time so use it parsimoniously.
        /// Default to 0 (no wait).
        /// </summary>
        public TimeSpan WaitInterval { get; set; }

        /// <summary>
        /// Whether to only browse the message and to let it in the queue.
        /// Otherwise the message is removed from the queue.
        /// default to false.
        /// </summary>
        public bool IsBrowsing { get; set; }

        /// <summary>
        /// Whether to only browse the next message and to let it in the queue.
        /// default to false.
        /// </summary>
        public bool IsBrowsingNext { get; set; }

        /// <summary>
        /// Gets/Browses a message with this correlation identifier if set.
        /// </summary>
        public byte[] CorrelationId { get; set; }

        /// <summary>
        /// The get message fails with an exception when the queue manager is quiescing (stopping).
        /// It is better to leave this parameter as true to avoid blocking the quiescing of the queue manager.
        /// But it can be useful to force the access for some very important tasks.
        /// Default to true.
        /// </summary>
        public bool FailIfQuiescing { get; set; } = true;

        #endregion Properties (Public)

        #region Methods (Internal)

        /// <summary>
        /// Extract from the options the MQ Open Options needed to get the messages from the queue.
        /// </summary>
        /// <returns>The IBM MQ Open Options computed from the parameters.</returns>
        internal MQGetMessageOptions GetMqGetMessageOptions()
        {
            var options = new MQGetMessageOptions { WaitInterval = (int)WaitInterval.TotalMilliseconds };
            if (FailIfQuiescing)
                options.Options += MQC.MQGMO_FAIL_IF_QUIESCING;
            if (WaitInterval.Ticks > 0)
                options.Options += MQC.MQGMO_WAIT;
            if (IsBrowsing)
                options.Options += MQC.MQGMO_BROWSE_FIRST;
            if (IsBrowsingNext)
                options.Options += MQC.MQGMO_BROWSE_NEXT;
            if (CorrelationId != null)
                options.MatchOptions = MQC.MQMO_MATCH_CORREL_ID;

            return options;
        }

        #endregion Methods (Internal)
    }
}