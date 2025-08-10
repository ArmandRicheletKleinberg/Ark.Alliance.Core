using Ark.Infrastructure.Info;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Ark.Infrastructure.Info
{
    /// <summary>
    /// Event log provider implementation for iOS/macOS platforms.
    /// </summary>
    internal class IosEventLogInfoProvider : IEventLogInfoProvider
    {
        public List<EventLogDto> GetEventLogs(int since = 60, string logSourceName = "", string[]? entryTypes = null)
            => GetApplicationEvents(logSourceName, since, entryTypes);

        public List<EventLogDto> GetApplicationEvents(string applicationName, int since = 60, string[]? entryTypes = null)
        {
            entryTypes ??= new[] { "Default", "Info", "Error", "Fault" };
            var logs = new List<EventLogDto>();
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "log",
                    Arguments = $"show --style syslog --last {since}m",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                using var proc = Process.Start(psi);
                if (proc == null)
                    return logs;
                while (!proc.StandardOutput.EndOfStream)
                {
                    var line = proc.StandardOutput.ReadLine();
                    if (line == null || !line.Contains(applicationName, StringComparison.OrdinalIgnoreCase))
                        continue;
                    if (!TryParseIosLogDate(line, out var dt))
                        continue;
                    var level = entryTypes.FirstOrDefault(t => line.Contains(t, StringComparison.OrdinalIgnoreCase)) ?? "Info";
                    logs.Add(new EventLogDto { Level = level, Message = line, Time = dt });
                }
                proc.WaitForExit(1000);
            }
            catch
            {
            }
            return logs;
        }

        private static bool TryParseIosLogDate(string line, out DateTime dt)
        {
            dt = default;
            var parts = line.Split(' ', 3);
            if (parts.Length < 2)
                return false;
            var datePart = $"{parts[0]} {parts[1]}";
            return DateTime.TryParse(datePart, out dt);
        }
    }
}
