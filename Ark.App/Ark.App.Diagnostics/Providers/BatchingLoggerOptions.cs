using System;

namespace Ark.App.Diagnostics
{
    /// <summary>
    /// Options that describe the behavior for batch logging.
    /// </summary>
    public class BatchingLoggerOptions
    {
        #region Properties (Public)

        /// <summary>
        /// Whether the logger accepts and queues writes.
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// The period after which logs will be flushed to the store.
        /// Default to <c>00:00:01</c>.
        /// </summary>
        public TimeSpan FlushPeriod { get; set; } = TimeSpan.FromSeconds(1);

        /// <summary>
        /// The maximum size of the background log message queue or null for no limit.
        /// After maximum queue size is reached log event sink would start blocking.
        /// Defaults to <c>1000</c>.
        /// </summary>
        public int? BackgroundQueueSize { get; set; } = 1000;

        /// <summary>
        /// Gets or sets a maximum number of events to include in a single batch or null for no limit.
        /// </summary>
        /// Defaults to <c>null</c>.
        public int? BatchSize { get; set; } = null;

        #endregion Properties (Public)
    }
}