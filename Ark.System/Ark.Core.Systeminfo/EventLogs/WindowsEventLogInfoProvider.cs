using Ark.Infrastructure.Info;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Runtime.Versioning;

namespace Ark.Infrastructure.Info
{
    /// <summary>
    /// Retrieves Windows event log entries through <see cref="EventLogReader"/>.
    /// + Filters by log level and time using XPath queries.
    /// - Works only on Windows hosts; other platforms require alternative providers.
    /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.diagnostics.eventing.reader.eventlogreader"/>
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal class WindowsEventLogInfoProvider : IEventLogInfoProvider
    {
        /// <summary>
        /// Retrieves event logs emitted by the specified source.
        /// + Convenience wrapper around <see cref="GetApplicationEvents(string,int,string[])"/>.
        /// - Performs synchronous I/O and may be slow for large log files.
        /// </summary>
        /// <param name="since">Minutes of history to include.</param>
        /// <param name="logSourceName">Event log source name, e.g. <c>"Application"</c>.</param>
        /// <param name="entryTypes">Optional entry type names such as <c>"Error"</c>.</param>
        /// <returns>List of <see cref="EventLogDto"/> entries.</returns>
        public List<EventLogDto> GetEventLogs(int since = 60, string logSourceName = "Application", string[]? entryTypes = null)
            => GetApplicationEvents(logSourceName, since, entryTypes);

        /// <summary>
        /// Retrieves application-specific event log records.
        /// + Converts Windows log levels into readable strings.
        /// - Access may require administrative privileges.
        /// Example JSON:
        /// <code language="json">
        /// [
        ///   { "level": "Error", "message": "Failure", "time": "2025-01-01T00:00:00Z" }
        /// ]
        /// </code>
        /// </summary>
        /// <param name="applicationName">The event log provider name.</param>
        /// <param name="since">Minutes of history to include.</param>
        /// <param name="entryTypes">Optional entry type names such as <c>"Error"</c>.</param>
        /// <returns>Collection of <see cref="EventLogDto"/> records.</returns>
        public List<EventLogDto> GetApplicationEvents(string applicationName, int since = 60, string[]? entryTypes = null)
        {
            entryTypes ??= new[] { "Warning", "Error", "Information", "AuditSuccess", "AuditFailure" };
            var levels = entryTypes.Select(ConvertEntryType).Where(i => i >= 0).ToArray();
            var sinceTime = DateTime.UtcNow.AddMinutes(-since).ToString("o");
            var levelFilter = levels.Length > 0 ? $" and ({string.Join(" or ", levels.Select(l => $"Level={l}"))})" : string.Empty;
            var query = $"*[System[Provider[@Name='{applicationName}'] and TimeCreated[@SystemTime>='{sinceTime}']{levelFilter}]]";
            var eventLogs = new List<EventLogDto>();
            var eventQuery = new EventLogQuery("Application", PathType.LogName, query) { ReverseDirection = true };
            using var reader = new EventLogReader(eventQuery);
            for (EventRecord record = reader.ReadEvent(); record != null; record = reader.ReadEvent())
            {
                if (record.TimeCreated == null) continue;
                eventLogs.Add(new EventLogDto
                {
                    Level = ConvertLevel(record.Level ?? 0),
                    Message = record.FormatDescription(),
                    Time = record.TimeCreated.Value
                });
            }
            return eventLogs;
        }

        /// <summary>
        /// Maps textual entry type names to Windows log level integers.
        /// </summary>
        /// <param name="entryType">Entry type name, e.g. <c>"Error"</c>.</param>
        /// <returns>Corresponding level value or <c>-1</c> when unknown.</returns>
        private static int ConvertEntryType(string entryType)
        {
            return entryType.ToLowerInvariant() switch
            {
                "error" => 2,
                "warning" => 3,
                "information" => 4,
                "auditsuccess" => 0,
                "auditfailure" => 1,
                _ => -1
            };
        }

        /// <summary>
        /// Converts numeric Windows event levels into human-readable strings.
        /// </summary>
        /// <param name="level">Numeric level.</param>
        /// <returns>Readable level string.</returns>
        private static string ConvertLevel(int level)
        {
            return level switch
            {
                1 => "Critical",
                2 => "Error",
                3 => "Warning",
                4 => "Information",
                0 => "Verbose",
                _ => "Information",
            };
        }
    }
}
