using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using Ark.Net.Models;
using Microsoft.Extensions.Logging;

// ReSharper disable UnusedMember.Global

namespace Ark.App.Diagnostics
{
    /// <summary>
    /// This repository is used to manage the Windows event log.
    /// </summary>
    public class WindowsEventLogRepository
    {
        #region Methods (Public)

        /// <summary>
        /// Gets the filtered application logs.
        /// </summary>
        /// <param name="sourceName">The name of the Windows event log source.</param>
        /// <param name="logName">The name of the Windows event log used by the source.</param>
        /// <param name="machineName">The name of computer where to get the log. Default to "." (this machine)</param>
        /// <param name="filterTimeFrom">The optional filter used to get the logs from a specific time.</param>
        /// <param name="filterTimeTo">The optional filter used to get the logs until a specific time.</param>
        /// <param name="filterSeverity">The optional filter to get only some logs matching one or more severity(flags).</param>
        /// <param name="filterCategory">The optional filter on the category. This filter is not used in the query and so the performance could suffer a little.</param>
        /// <param name="maxLogsNumber">The maximum number of logs to take. Optional, by default gets all the logs.</param>
        /// <returns>
        /// Success : The filtered logs found are returned.
        /// BadParameters : Either the source or the log name.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
        public Task<Result<LogDto[]>> GetLogs(string sourceName, string logName = "DSS", string machineName = ".", DateTime? filterTimeFrom = null, DateTime? filterTimeTo = null,
            LogLevel? filterSeverity = null, string filterCategory = null, int maxLogsNumber = int.MaxValue) => Task.Run(() =>
            {
                try
                {
                    if (sourceName == null || logName == null)
                        return Result<LogDto[]>.BadParameters.WithReason("The source and log name must be provided.");

                    // Creates the event log query given the filters
                    var filters = new List<string> { $"*[System[Provider[@Name='{sourceName}']]]" };
                    if (filterSeverity.HasValue)
                    {
                        var levels = ConvertSeverityToEventLogLevels(filterSeverity.Value);
                        filters.Add($"*[System[{string.Join(" and ", levels.Select(l => $"Severity={l}"))}]]");
                    }
                    if (filterTimeFrom.HasValue)
                        filters.Add($"*[System[TimeCreated[@SystemTime>='{filterTimeFrom.Value.ToUniversalTime():o}']]]");
                    if (filterTimeTo.HasValue)
                        filters.Add($"*[System[TimeCreated[@SystemTime<='{filterTimeTo.Value.ToUniversalTime():o}']]]");
                    var queryString = string.Join(" and ", filters);
                    var session = new EventLogSession(machineName);
                    var query = new EventLogQuery(logName, PathType.LogName, queryString) { ReverseDirection = true, Session = session };

                    var logs = ReadEventRecords(query, filterCategory, maxLogsNumber).ToArray();

                    return new Result<LogDto[]>(logs);
                }
                catch (Exception exception)
                {
                    return new Result<LogDto[]>(exception);
                }
            });

        /// <summary>
        /// Creates a source in a possibly new log if not already exists.
        /// It also sets up the limit of the new Windows Event log.
        /// </summary>
        /// <param name="sourceName">The name of the source to search for or to create.</param>
        /// <param name="logName">The name of the Windows event log that owns the source to create. It maybe a new log. Default to DSS for DSS application.</param>
        /// <param name="machineName">The name of computer where to log. Default to "." (this machine)</param>
        /// <param name="maximumKilobytes">The maximum storage size for the logs in kb. Default to 64000Kb.</param>
        /// <param name="overflowPolicy">The policy to apply when an overflow occurs in the event log. Default to overwrite the older.</param>
        /// <param name="retentionDays">The number of days to retain the logs. Default to 60.</param>
        /// <returns>
        /// Success : The source has been successfully created.
        /// Already : The source already exists.
        /// Unauthorized : The user has no permission to create a new event log source.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
        public Result CreateSourceIfNotExists(string sourceName, string logName = "DSS", string machineName = ".", int maximumKilobytes = 64000, OverflowAction overflowPolicy = OverflowAction.OverwriteOlder, int retentionDays = 60)
        {
            try
            {
                if (EventLog.SourceExists(sourceName, machineName))
                    return Result.Already.WithReason("The source already exists in the Windows event log.");

                EventLog.CreateEventSource(new EventSourceCreationData(sourceName, logName) { MachineName = machineName });
                var eventLog = EventLog.GetEventLogs().First(l => l.Log == logName);
                eventLog.MaximumKilobytes = maximumKilobytes / 64 * 64; // Forces the value to be a multiple of 64
                eventLog.ModifyOverflowPolicy(overflowPolicy, retentionDays);

                return Result.Success;
            }
            catch (SecurityException exception)
            {
                return Result.Unauthorized.WithException(exception).WithReason("The user used to run the application has not the permission to create a new source in the Windows event log.");
            }
            catch (Exception exception)
            {
                return new Result(exception);
            }
        }

        #endregion Methods (Public)

        #region Methods (Helpers)

        /// <summary>
        /// Read the events records one after the other until no other event can be read for the query or an optional maximum logs number.
        /// The category is filtered also here to take only the max logs number that matches the category.
        /// </summary>
        /// <param name="eventLogQuery">The query to use to get the event records to read.</param>
        /// <param name="filterCategory">The optional filter on the category.</param>
        /// <param name="maxLogsNumber">The maximum logs number to return.</param>
        /// <returns>The enumerable with the created log matching the category filter if any.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
        private static IEnumerable<LogDto> ReadEventRecords(EventLogQuery eventLogQuery, string filterCategory, int maxLogsNumber)
        {
            using var reader = new EventLogReader(eventLogQuery);
            var counter = 0;
            while (counter < maxLogsNumber)
            {
                var record = reader.ReadEvent();
                if (record == null)
                    yield break;

                var log = ConvertEventRecordToLog(record);
                if (filterCategory != null && log.Category != filterCategory)
                    continue;

                counter++;
                yield return log;
            }
        }

        /// <summary>
        /// Converts an event record to a LogDto.
        /// </summary>
        /// <param name="eventRecord">The event record to convert.</param>
        /// <returns>The converted event record.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
        private static LogDto ConvertEventRecordToLog(EventRecord eventRecord)
        {
            var description = eventRecord.FormatDescription();
            return new LogDto
            {
                CreationTime = eventRecord.TimeCreated ?? DateTime.MinValue,
                Severity = ConvertEventLogLevelToSeverity(eventRecord.Level ?? 0),
                Category = description.SubstringUntil(Environment.NewLine),
                Details = description.SubstringFrom(Environment.NewLine)
            };
        }

        /// <summary>
        /// Converts a log severity into the event record level to filter the records.
        /// The severity is a flagged enumeration and can possess more than one value.
        /// </summary>
        /// <param name="severity">The severity to convert to event record level.</param>
        /// <returns>The levels that should be used to query the event logs.</returns>
        private static IEnumerable<int> ConvertSeverityToEventLogLevels(LogLevel severity)
        {
            if (severity.HasFlag(LogLevel.Debug))
                yield return 0;
            if (severity.HasFlag(LogLevel.Critical))
                yield return 1;
            if (severity.HasFlag(LogLevel.Error))
                yield return 2;
            if (severity.HasFlag(LogLevel.Warning))
                yield return 3;
            if (severity.HasFlag(LogLevel.Information))
                yield return 4;
        }

        /// <summary>
        /// Converts a event record level to a log severity.
        /// </summary>
        /// <param name="eventRecordLevel">The level of the event record.</param>
        /// <returns>The severity converted depending on the event record level.</returns>
        private static LogSeverityEnum ConvertEventLogLevelToSeverity(int eventRecordLevel)
        {
            switch (eventRecordLevel)
            {
                case 0: return LogSeverityEnum.Debug;
                case 1: return LogSeverityEnum.Critical;
                case 2: return LogSeverityEnum.Error;
                case 3: return LogSeverityEnum.Warning;
                case 4: return LogSeverityEnum.Information;
                default: return LogSeverityEnum.None;
            }
        }

        #endregion Methods (Helpers)
    }
}