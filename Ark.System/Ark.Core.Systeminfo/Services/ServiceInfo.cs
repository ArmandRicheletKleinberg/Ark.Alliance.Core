

using Ark;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security.Cryptography.X509Certificates;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks;


namespace Ark.Infrastructure.Info
{
    /// <summary>
    /// Provides utilities to query system services on the current operating system.
    /// + Aggregates runtime data and event logs across platforms.
    /// - Requires elevated permissions to access some service metadata.
    /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.serviceprocess.servicecontroller"/>.
    /// On Windows it relies on <see href="https://learn.microsoft.com/dotnet/api/system.serviceprocess.servicecontroller">ServiceController</see> and related APIs.
    /// On Linux it executes <c>systemctl</c> commands.
    /// </summary>
    public static class ServiceInfo
    {
        #region Public API
        /// <summary>
        /// Gets detailed information about installed services on the current system.
        /// + Includes CPU, memory and disk usage when available.
        /// - Windows-specific details may not be accessible without administrator rights.
        /// Ref: <see href="https://learn.microsoft.com/windows/win32/services/service-controller-manager"/>.
        /// </summary>
        /// <param name="namePattern">Optional pattern the service name must start with.</param>
        /// <param name="publisher">Publisher contained in the executable for Windows services.</param>
        /// <param name="eventLogMinutes">Time span in minutes for returned event logs.</param>
        /// <returns>
        /// A <see cref="Result{T}"/> containing a list of <see cref="DetailedServiceInfoDto"/>.
        /// The result serializes to:
        /// <example>
        /// <code language="json">
        /// [
        ///   {
        ///     "serviceName": "ssh",
        ///     "status": "Running"
        ///   }
        /// ]
        /// </code>
        /// </example>
        /// </returns>
        /// <example>
        /// <code>
        /// Result&lt;List&lt;DetailedServiceInfoDto&gt;&gt; result = ServiceInfo.Get("ssh");
        /// </code>
        /// </example>
        public static Result<List<DetailedServiceInfoDto>> Get(string? namePattern = null, string? publisher = null, int eventLogMinutes = 60)
            => RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? GetWindowsServices(namePattern, publisher, eventLogMinutes)
                : RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                    ? GetLinuxServices(namePattern)
                    : new Result<List<DetailedServiceInfoDto>>(new NotSupportedException("Unsupported OS"));

        /// <summary>
        /// Asynchronously gets detailed information about installed services on the current system.
        /// + Offloads work to a background thread to avoid blocking the caller.
        /// - Still subject to the same platform limitations as <see cref="Get"/>.
        /// Ref: <see href="https://learn.microsoft.com/dotnet/standard/parallel-programming/task-parallel-library-tpl"/>.
        /// </summary>
        /// <param name="namePattern">Optional pattern the service name must start with.</param>
        /// <param name="publisher">Publisher contained in the executable for Windows services.</param>
        /// <param name="eventLogMinutes">Time span in minutes for returned event logs.</param>
        /// <returns>
        /// A task returning a <see cref="Result{T}"/> containing the matching services.
        /// The payload is identical to <see cref="Get"/>:
        /// <example>
        /// <code language="json">
        /// [
        ///   {
        ///     "serviceName": "ssh",
        ///     "status": "Running"
        ///   }
        /// ]
        /// </code>
        /// </example>
        /// </returns>
        /// <example>
        /// <code>
        /// Result&lt;List&lt;DetailedServiceInfoDto&gt;&gt; result = await ServiceInfo.GetAsync("ssh");
        /// </code>
        /// </example>
        public static Task<Result<List<DetailedServiceInfoDto>>> GetAsync(string? namePattern = null, string? publisher = null, int eventLogMinutes = 60)
            => Task.Run(() => Get(namePattern, publisher, eventLogMinutes));
        #endregion

        #region Windows Implementation

        /// <summary>
        /// Retrieves services using the Windows <see href="https://learn.microsoft.com/dotnet/api/system.serviceprocess.servicecontroller">ServiceController</see> API.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private static Result<List<DetailedServiceInfoDto>> GetWindowsServices(string? namePattern, string? publisher, int eventLogMinutes)
            => Result<List<DetailedServiceInfoDto>>.SafeExecute(() =>
            {
                List<DetailedServiceInfoDto> infos = ServiceController.GetServices()
                    .Where(s => string.IsNullOrWhiteSpace(namePattern) ||
                                s.ServiceName.StartsWith(namePattern!, StringComparison.OrdinalIgnoreCase) ||
                                s.DisplayName.StartsWith(namePattern!, StringComparison.OrdinalIgnoreCase))
                    .Select(s => BuildWindowsInfo(s, eventLogMinutes, publisher))
                    .Where(i => i != null)
                    .Select(i => i!)
                    .ToList();

                return new Result<List<DetailedServiceInfoDto>>(infos);
            });

        /// <summary>
        /// Builds a <see cref="DetailedServiceInfoDto"/> for a specific Windows service.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private static DetailedServiceInfoDto? BuildWindowsInfo(ServiceController svc, int eventLogMinutes, string? publisher)
        {
            string? exePath = GetServiceExecutablePath(svc.ServiceName);
            if (!string.IsNullOrWhiteSpace(publisher) && !string.IsNullOrWhiteSpace(exePath))
            {
                try
                {
                    FileVersionInfo v = FileVersionInfo.GetVersionInfo(exePath!);
                    if (string.IsNullOrWhiteSpace(v.CompanyName) || !v.CompanyName.Contains(publisher!, StringComparison.OrdinalIgnoreCase))
                        return null;
                }
                catch
                {
                }
            }

            int? pid = GetProcessId(svc.ServiceName);
            Process? process = null;
            if (pid.HasValue)
            {
                try
                {
                    process = Process.GetProcessById(pid.Value);
                }
                catch
                {
                    pid = null;
                }
            }

            double cpuUsage = 0;
            double memUsage = 0;
            if (process != null)
            {
                try
                {
                    memUsage = process.WorkingSet64;
                    if (OperatingSystem.IsWindows())
                    {
                        using PerformanceCounter cpuCounter = new("Process", "% Processor Time", process.ProcessName, true);
                        cpuCounter.NextValue();
                        Thread.Sleep(500);
                        cpuUsage = cpuCounter.NextValue() / Environment.ProcessorCount;
                    }
                }
                catch { }
            }

            double totalMem = GetTotalMemory();
            double memProp = totalMem > 0 ? memUsage / totalMem * 100 : 0;

            DateTime? startTime = GetLastServiceEvent(svc.ServiceName, true);
            DateTime? stopTime = GetLastServiceEvent(svc.ServiceName, false);
            List<EventLogDto> evts = GetServiceEvents(svc.ServiceName, eventLogMinutes);

            string version = string.Empty;
            string company = string.Empty;
            string signature = string.Empty;
            DateTime? exeDate = null;
            if (!string.IsNullOrWhiteSpace(exePath) && File.Exists(exePath!))
            {
                try
                {
                    FileVersionInfo fileInfo = FileVersionInfo.GetVersionInfo(exePath!);
                    version = fileInfo.FileVersion ?? string.Empty;
                    company = fileInfo.CompanyName ?? string.Empty;
                    exeDate = File.GetLastWriteTime(exePath!);
                    try
                    {
                        X509Certificate2 cert = X509CertificateLoader.LoadCertificateFromFile(exePath!);
                        signature = cert.Subject;
                    }
                    catch { }
                }
                catch { }
            }

            return ServiceInfoMapper.Map(svc, GetServiceAccount(svc.ServiceName), startTime, stopTime, cpuUsage, cpuUsage, memUsage, memProp, evts, version, signature, company, exeDate);
        }

        /// <summary>
        /// Retrieves the account used to run a Windows service.
        /// </summary>
        /// <param name="serviceName">Name of the service.</param>
        /// <returns>The start account or an empty string.</returns>
        [SupportedOSPlatform("windows")]
        private static string GetServiceAccount(string serviceName)
        {
            try
            {
                using ManagementObject service = new($"Win32_Service.Name='{serviceName}'");
                return service["StartName"]?.ToString() ?? string.Empty;
            }
            catch { return string.Empty; }
        }

        /// <summary>
        /// Gets the path to the service executable.
        /// </summary>
        /// <param name="serviceName">Name of the service.</param>
        /// <returns>The executable path or <see langword="null"/>.</returns>
        [SupportedOSPlatform("windows")]
        private static string? GetServiceExecutablePath(string serviceName)
        {
            try
            {
                using ManagementObject service = new($"Win32_Service.Name='{serviceName}'");
                return service["PathName"]?.ToString();
            }
            catch { return null; }
        }

        /// <summary>
        /// Resolves the process identifier for a service.
        /// </summary>
        /// <param name="serviceName">Name of the service.</param>
        /// <returns>The process id or <see langword="null"/>.</returns>
        [SupportedOSPlatform("windows")]
        private static int? GetProcessId(string serviceName)
        {
            try
            {
                using ManagementObject service = new($"Win32_Service.Name='{serviceName}'");
                return service["ProcessId"] is null ? null : Convert.ToInt32(service["ProcessId"]);
            }
            catch { return null; }
        }

        /// <summary>
        /// Returns the amount of physical memory in bytes.
        /// </summary>
        /// <returns>The total memory or zero if unavailable.</returns>
        private static double GetTotalMemory()
        {
            try
            {
                return Ark.Infrastructure.Info.NativeMemory.GetTotalPhysicalMemory();
            }
            catch { return 0; }
        }

        /// <summary>
        /// Reads the system event log to locate the last start or stop event.
        /// </summary>
        /// <param name="serviceName">Name of the service.</param>
        /// <param name="start">True to search for start events, otherwise stop events.</param>
        /// <returns>The event time or <see langword="null"/>.</returns>
        [SupportedOSPlatform("windows")]
        private static DateTime? GetLastServiceEvent(string serviceName, bool start)
        {
            try
            {
                EventLog log = new("System");
                foreach (EventLogEntry e in log.Entries.Cast<EventLogEntry>()
                           .Where(e => e.Source == "Service Control Manager" && e.Message.Contains(serviceName))
                           .Where(e => e.InstanceId == 7036)
                         .OrderByDescending(e => e.TimeGenerated))
                {
                    bool running = e.Message.Contains("running", StringComparison.OrdinalIgnoreCase);
                    if (start && running) return e.TimeGenerated;
                    if (!start && !running) return e.TimeGenerated;
                }
            }
            catch { }
            return null;
        }

        /// <summary>
        /// Retrieves application log events related to the service within the given time window.
        /// </summary>
        /// <param name="serviceName">Name of the service.</param>
        /// <param name="minutes">Time window in minutes.</param>
        /// <returns>The list of related events.</returns>
        [SupportedOSPlatform("windows")]
        private static List<EventLogDto> GetServiceEvents(string serviceName, int minutes)
        {
            List<EventLogDto> events = new();
            try
            {
                DateTime since = DateTime.Now.AddMinutes(-minutes);
                EventLog app = new("Application");
                events.AddRange(app.Entries.Cast<EventLogEntry>()
                    .Where(e => e.TimeGenerated >= since && e.Message.Contains(serviceName, StringComparison.OrdinalIgnoreCase))
                    .Select(e => new EventLogDto
                    {
                        Level = e.EntryType.ToString(),
                        Message = e.Message,
                        Time = e.TimeGenerated
                    }));
            }
            catch { }
            return events;
        }

        #endregion

        #region Linux Implementation

        /// <summary>
        /// Retrieves services on Linux systems using the <c>systemctl</c> command.
        /// </summary>
        /// <param name="namePattern">Optional filter applied to service names.</param>
        /// <returns>A <see cref="Result{T}"/> with the discovered services.</returns>
        private static Result<List<DetailedServiceInfoDto>> GetLinuxServices(string? namePattern)
            => Result<List<DetailedServiceInfoDto>>.SafeExecute(() =>
            {
                List<DetailedServiceInfoDto> services = new();
                try
                {
                    ProcessStartInfo psi = new("systemctl", "list-units --type=service --no-legend")
                    {
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    };
                    using Process? proc = Process.Start(psi);
                    if (proc == null)
                        return new Result<List<DetailedServiceInfoDto>>(services);
                    string output = proc.StandardOutput.ReadToEnd();
                    proc.WaitForExit();
                    foreach (string line in output.Split('\n', StringSplitOptions.RemoveEmptyEntries))
                    {
                        string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length < 4) continue;
                        string name = parts[0];
                        if (!string.IsNullOrWhiteSpace(namePattern) && !name.StartsWith(namePattern!, StringComparison.OrdinalIgnoreCase))
                            continue;
                        services.Add(new DetailedServiceInfoDto
                        {
                            ServiceName = name,
                            DisplayName = name,
                            Status = parts[3],
                            StartType = parts[1]
                        });
                    }
                }
                catch { }
                return new Result<List<DetailedServiceInfoDto>>(services);
            });

        #endregion
    }
}
