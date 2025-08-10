using System;
using Microsoft.Extensions.Logging;

namespace Ark.App.Diagnostics
{
    /// <summary>
    /// The log message to log.
    /// </summary>
    public class LogMessage
    {
        #region Properties (Public)

        /// <summary>
        /// The timestamp where the log has been created.
        /// </summary>
        public DateTimeOffset Timestamp { get; set; }

        /// <summary>
        /// The log level.
        /// </summary>
        public LogLevel LogLevel { get; set; }

        /// <summary>
        /// The category of the message to log.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// The message to log.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// The exception to log.
        /// </summary>
        public Exception Exception { get; set; }

        #endregion Properties (Public)
    }
}