using Ark.Infrastructure.Info;
using System.Collections.Generic;

namespace Ark.Infrastructure.Info
{
    /// <summary>
    /// Abstraction for retrieving operating system event log entries.
    /// </summary>
    internal interface IEventLogInfoProvider
    {
        List<EventLogDto> GetEventLogs(int since = 60, string logSourceName = "Application", string[]? entryTypes = null);
        List<EventLogDto> GetApplicationEvents(string applicationName, int since = 60, string[]? entryTypes = null);
    }
}
