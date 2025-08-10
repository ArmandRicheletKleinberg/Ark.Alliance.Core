using Ark.Infrastructure.Info;
using System;
using System.Collections.Generic;
using System.Runtime.Versioning;

namespace Ark.Infrastructure.Info
{
    /// <summary>
    /// Provides an entry point to retrieve events from the underlying operating system.
    /// + Centralizes platform-specific providers behind a single API.
    /// - Requires access to OS event logs which may demand elevated privileges.
    /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.diagnostics.eventlog"/>
    /// </summary>
    public static class EventLogInfoProvider
    {
        #pragma warning disable CA1416 // Platform validation
        private static readonly IEventLogInfoProvider _provider = PlatformProvider.Create<IEventLogInfoProvider>(
            CreateWindowsEventLogInfoProvider,
            () => new LinuxEventLogInfoProvider(),
            () => new AndroidEventLogInfoProvider(),
            () => new IosEventLogInfoProvider(),
            () => new DefaultEventLogInfoProvider());
        #pragma warning restore CA1416

        /// <summary>
        /// Retrieves event logs emitted by the current process.
        /// + Supports platform-specific log stores.
        /// - Large log files may incur allocation overhead.
        /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.diagnostics.eventlogentry"/>
        /// </summary>
        /// <param name="since">Timespan in minutes from which to retrieve event logs.</param>
        /// <param name="logSourceName">Event log source, e.g. <c>"Application"</c>.</param>
        /// <param name="entryTypes">Optional array of entry types to filter.</param>
        /// <returns>
        /// List of <see cref="EventLogDto"/> items.
        /// Example JSON:
        /// <code language="json">
        /// [
        ///   { "source": "Application", "message": "Started" }
        /// ]
        /// </code>
        /// </returns>
        public static List<EventLogDto> GetEventLogs(int since = 60, string logSourceName = "Application", string[]? entryTypes = null)
            => _provider.GetEventLogs(since, logSourceName, entryTypes);

        /// <summary>
        /// Retrieves event logs for a specific application filtered by entry type.
        /// + Allows targeted diagnostics for a given application.
        /// - Application-specific logs may not exist on all platforms.
        /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.diagnostics.eventlog"/>
        /// </summary>
        /// <param name="applicationName">The name of the application.</param>
        /// <param name="since">Timespan in minutes from which to retrieve event logs.</param>
        /// <param name="entryTypes">Optional array of entry types to filter.</param>
        /// <returns>
        /// List of <see cref="EventLogDto"/> objects representing the event logs.
        /// Example JSON:
        /// <code language="json">
        /// [
        ///   { "source": "MyApp", "level": "Error" }
        /// ]
        /// </code>
        /// </returns>
        public static List<EventLogDto> GetApplicationEvents(string applicationName, int since = 60, string[]? entryTypes = null)
            => _provider.GetApplicationEvents(applicationName, since, entryTypes);

        private class DefaultEventLogInfoProvider : IEventLogInfoProvider
        {
            public List<EventLogDto> GetEventLogs(int since, string logSourceName, string[]? entryTypes) => [];
            public List<EventLogDto> GetApplicationEvents(string applicationName, int since, string[]? entryTypes) => [];
        }

        [SupportedOSPlatform("windows")]
        private static IEventLogInfoProvider CreateWindowsEventLogInfoProvider() => new WindowsEventLogInfoProvider();
    }
}
