using Ark.Infrastructure.Info;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace Ark.Infrastructure.Info
{
    /// <summary>
    /// Event log provider implementation for Android platforms.
    /// </summary>
    internal class AndroidEventLogInfoProvider : IEventLogInfoProvider
    {
        public List<EventLogDto> GetEventLogs(int since = 60, string logSourceName = "", string[]? entryTypes = null)
            => GetApplicationEvents(logSourceName, since, entryTypes);

        public List<EventLogDto> GetApplicationEvents(string applicationName, int since = 60, string[]? entryTypes = null)
        {
            entryTypes ??= new[] { "I", "W", "E" };
            var logs = new List<EventLogDto>();
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "logcat",
                    Arguments = $"-d -v time {applicationName}:* *:S",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                using var proc = Process.Start(psi);
                if (proc == null)
                    return logs;
                var start = DateTime.UtcNow.AddMinutes(-since);
                while (!proc.StandardOutput.EndOfStream)
                {
                    var line = proc.StandardOutput.ReadLine();
                    if (line == null || line.Length < 18) continue;
                    var dateStr = line.Substring(0, 18);
                    if (!DateTime.TryParseExact(dateStr, "MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var dt))
                        continue;
                    if (dt < start.ToLocalTime()) continue;
                    var level = line[19].ToString();
                    if (Array.Exists(entryTypes, e => e.Equals(level, StringComparison.OrdinalIgnoreCase)))
                    {
                        var msgIndex = line.IndexOf(": ", StringComparison.Ordinal);
                        var msg = msgIndex > 0 ? line[(msgIndex + 2)..] : line;
                        logs.Add(new EventLogDto { Level = level, Message = msg, Time = dt.ToUniversalTime() });
                    }
                }
                proc.WaitForExit(1000);
            }
            catch
            {
            }
            return logs;
        }
    }
}
