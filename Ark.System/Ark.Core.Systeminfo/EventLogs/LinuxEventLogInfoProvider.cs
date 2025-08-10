using Ark.Infrastructure.Info;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Ark.Infrastructure.Info
{
    /// <summary>
    /// Event log provider implementation for Linux platforms.
    /// </summary>
    internal class LinuxEventLogInfoProvider : IEventLogInfoProvider
    {
        private const string DefaultLogFile = "/var/log/syslog";

        public List<EventLogDto> GetEventLogs(int since = 60, string logSourceName = "syslog", string[]? entryTypes = null)
            => GetApplicationEvents(logSourceName, since, entryTypes);

        public List<EventLogDto> GetApplicationEvents(string applicationName, int since = 60, string[]? entryTypes = null)
        {
            entryTypes ??= new[] { "info", "warning", "err" };
            var logs = new List<EventLogDto>();
            var logFile = File.Exists(DefaultLogFile) ? DefaultLogFile : "/var/log/messages";
            if (!File.Exists(logFile))
                return logs;

            DateTime start = DateTime.UtcNow.AddMinutes(-since);
            foreach (var line in File.ReadLines(logFile))
            {
                if (!line.Contains(applicationName, StringComparison.OrdinalIgnoreCase))
                    continue;

                if (!TryParseSyslogDate(line, out var time))
                    continue;

                if (time < start.ToLocalTime())
                    continue;

                string level = entryTypes.FirstOrDefault(t => line.Contains(t, StringComparison.OrdinalIgnoreCase)) ?? "info";
                logs.Add(new EventLogDto { Level = level, Message = line, Time = time.ToUniversalTime() });
            }

            return logs;
        }

        private static bool TryParseSyslogDate(string line, out DateTime dateTime)
        {
            dateTime = default;
            if (line.Length < 15)
                return false;

            var part = line.Substring(0, 15);
            const string format = "MMM d HH:mm:ss";
            if (!DateTime.TryParseExact(part, format, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var dt))
                return false;

            dateTime = dt;
            return true;
        }
    }
}
