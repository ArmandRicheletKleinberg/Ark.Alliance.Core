using System;

namespace Ark.Infrastructure.Info
{
    /// <summary>
    /// Represents a single entry in the host event log.
    /// + Captures log level, message and timestamp for auditing.
    /// - Does not include source process or thread information.
    /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.diagnostics.eventlogentry"/>
    /// </summary>
    public class EventLogDto
    {
        #region Properties

        /// <summary>
        /// Event log level.
        /// </summary>
        public string Level { get; set; } = string.Empty;

        /// <summary>
        /// Event log message.
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Time the event was recorded.
        /// </summary>
        public DateTime Time { get; set; }

        #endregion Properties
    }
}
