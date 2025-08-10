using System;
using System.Runtime.Versioning;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.EventLog;

namespace Ark.App.Diagnostics
{
    /// <summary>
    /// This is an extension class to add Windows event logging feature to an Core app.
    /// This must be set in the .NET core program main builder because the log system must be defined before the Startup initialization.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public static class ILoggingBuilderExtension
    {
        #region Static methods

        /// <summary>
        /// Adds the windows event logging feature to the app.
        /// It initializes a specific Windows event log source if not already exists.
        /// The event source will be set in the DSS section.
        /// </summary>
        /// <remarks>BEWARE ! This feature needs the right to access the Windows event log, so give the system user the rights to access the event log of its machine.</remarks>
        /// <param name="loggingBuilder">The builder used to configure the logging feature.</param>
        /// <param name="sourceName">The name of the source to create in the Application section.</param>
        [SupportedOSPlatform("windows")]
        public static void AddWindowsEventLogging(this ILoggingBuilder loggingBuilder, string sourceName)
        {
            if (!OperatingSystem.IsWindows())
                return;

            loggingBuilder.AddEventLog(new EventLogSettings { SourceName = sourceName, LogName = "DSS" });

            var checkSourceResult = new WindowsEventLogRepository().CreateSourceIfNotExists(sourceName);
            if (checkSourceResult.IsNotSuccess && checkSourceResult.IsNotAlready)
                throw checkSourceResult.ToException();
        }

        #endregion Static methods
    }
}