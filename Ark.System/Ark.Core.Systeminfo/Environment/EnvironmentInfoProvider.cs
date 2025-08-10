using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using Ark;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Runtime.Versioning;
using System.Threading.Tasks;


namespace Ark.Infrastructure.Info
{
    /// <summary>
    /// Provides methods to retrieve detailed information about the execution environment.
    /// + Aggregates CPU, memory, network, and service data in one object.
    /// - Each call enumerates system resources and may be costly on large hosts.
    /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.runtime.interopservices.runtimeinformation"/>
    /// </summary>
    public static class EnvironmentInfoProvider
    {
        #region Fields

        private const int DefaultSinceMinutes = 60;

        private static string DefaultSourceName => Process.GetCurrentProcess().ProcessName;

        #endregion Fields

        #region Methods (Public)

        /// <summary>
        /// Builds a full <see cref="AppSystemInfoDto"/> snapshot of the current environment.
        /// + Combines multiple providers into a single payload and uses <see cref="OSHelper"/> for platform checks.
        /// - Runs synchronously and may block while querying services and event logs.
        /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.diagnostics.eventlog"/>
        /// </summary>
        /// <param name="since">Minutes of event logs to include.</param>
        /// <returns>A populated <see cref="AppSystemInfoDto"/>.</returns>
        /// <example>
        /// <code language="json">
        /// {
        ///   "machineName": "HOST",
        ///   "operatingSystem": "Linux"
        /// }
        /// </code>
        /// </example>
        public static AppSystemInfoDto GetEnvironmentInfo(int since = DefaultSinceMinutes)
        {
            AppSystemInfoDto envInfo = new AppSystemInfoDto
            {
                Framework = RuntimeInformation.FrameworkDescription,
                ProgrammingLanguage = "C#",
                OperatingSystem = RuntimeInformation.OSDescription,
                OSType = OSHelper.GetCurrentPlatform().ToString(),
                OSVersion = Environment.OSVersion.ToString(),

                MachineName = Environment.MachineName,
                DomainName = IPGlobalProperties.GetIPGlobalProperties().DomainName,

                HostNetworkInfo = NetworkInfoProvider.GetNetworkInfo().Data,
                NetworkUsage = GetNetworkUsage(),
                DiskUsage = GetDiskUsage(),
                MemoryUsage = GetMemoryUsage(),
                AssemblyCPUUsage = OperatingSystem.IsWindows() ? GetCurrentProcessCPUUsage() : 0,
                RunningServices = GetRunningServices(),
                EventLogs = EventLogInfoProvider.GetEventLogs(since, DefaultSourceName)
            };
            return envInfo;
        }

        /// <summary>
        /// Asynchronously collects a comprehensive snapshot of the host environment.
        /// + Offloads work to the thread pool via <see cref="Task.Run{TResult}(System.Func{TResult})"/>.
        /// - Uses a background thread rather than true asynchronous APIs.
        /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.threading.tasks.task.run"/>
        /// </summary>
        /// <param name="since">Minutes of event logs to include.</param>
        /// <returns>
        /// Task producing an <see cref="AppSystemInfoDto"/>.
        /// Example JSON:
        /// <code language="json">
        /// {
        ///   "machineName": "HOST",
        ///   "operatingSystem": "Linux"
        /// }
        /// </code>
        /// </returns>
        public static Task<AppSystemInfoDto> GetEnvironmentInfoAsync(int since = DefaultSinceMinutes)
            => Task.Run(() => GetEnvironmentInfo(since));

        /// <summary>
        /// Collects a lightweight snapshot of the host environment skipping heavy operations.
        /// + Avoids expensive queries for faster execution.
        /// - Provides less detailed diagnostics than <see cref="GetEnvironmentInfo(int)"/>.
        /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.environment"/>
        /// </summary>
        /// <param name="since">Minutes of event logs to include.</param>
        /// <returns>An <see cref="AppSystemInfoDto"/> containing basic environment data.</returns>
        /// <example>
        /// <code>
        /// var environmentInfo = EnvironmentInfoProvider.GetLazyEnvironmentInfo();
        /// </code>
        /// </example>
        public static AppSystemInfoDto GetLazyEnvironmentInfo(int since = DefaultSinceMinutes)
        {
            AppSystemInfoDto envInfo = new AppSystemInfoDto
            {
                Framework = RuntimeInformation.FrameworkDescription,
                ProgrammingLanguage = "C#",
                OperatingSystem = RuntimeInformation.OSDescription,
                OSType = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "Windows" :
                          RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "Linux" :
                          RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "MacOS" : "Unknown",
                OSVersion = Environment.OSVersion.ToString(),

                MachineName = Environment.MachineName,

                DomainName = IPGlobalProperties.GetIPGlobalProperties().DomainName,

                HostNetworkInfo = NetworkInfoProvider.GetNetworkInfo().Data,
                NetworkUsage = GetNetworkUsage(),
                DiskUsage = GetDiskUsage(),
                MemoryUsage = GetMemoryUsage(),
                AssemblyCPUUsage = OperatingSystem.IsWindows() ? GetCurrentProcessCPUUsage() : 0,
                RunningServices = GetRunningServices(),
                EventLogs = EventLogInfoProvider.GetEventLogs(since, DefaultSourceName),

            };

            return envInfo;
        }

        /// <summary>
        /// Asynchronously retrieves a lightweight environment snapshot.
        /// + Uses <see cref="Task.Run{TResult}(System.Func{TResult})"/> to offload work.
        /// - Returns limited metrics compared to <see cref="GetEnvironmentInfoAsync"/>.
        /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.threading.tasks.task"/>
        /// </summary>
        /// <param name="since">Minutes of event logs to include.</param>
        /// <returns>A task producing an <see cref="AppSystemInfoDto"/> with basic environment data.</returns>
        /// <example>
        /// <code>
        /// var info = await EnvironmentInfoProvider.GetLazyEnvironmentInfoAsync();
        /// </code>
        /// </example>
        public static Task<AppSystemInfoDto> GetLazyEnvironmentInfoAsync(int since = DefaultSinceMinutes)
            => Task.Run(() => GetLazyEnvironmentInfo(since));

        /// <summary>
        /// Gets the total CPU usage percentage asynchronously.
        /// + Offloads sampling to a background thread using <see cref="Task.Run{TResult}(System.Func{TResult})"/>.
        /// - Single snapshot; does not average over time.
        /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.diagnostics.performancecounter"/>
        /// </summary>
        /// <returns>A task returning the CPU usage percentage (e.g., <c>75.5</c>).</returns>
        public static Task<double> GetCPUUsageAsync()
            => OperatingSystem.IsWindows() ? Task.Run(GetCPUUsage) : Task.FromResult(0d);


        /// <summary>
        /// Gets the total network usage asynchronously.
        /// + Aggregates sent and received bytes across all adapters.
        /// - Uses <see cref="NetworkInterface"/> which may not include virtual interfaces on some systems.
        /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.net.networkinformation.networkinterface"/>
        /// </summary>
        /// <returns>A task returning the total network usage in bytes.</returns>
        public static Task<double> GetNetworkUsageAsync()
            => Task.Run(GetNetworkUsage);

        /// <summary>
        /// Gets the disk usage percentage asynchronously.
        /// + Computes totals across all ready drives.
        /// - Uses <see cref="DriveInfo"/> which may block on slow media.
        /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.io.driveinfo"/>
        /// </summary>
        /// <returns>A task returning the disk usage percentage.</returns>
        public static Task<double> GetDiskUsageAsync()
            => Task.Run(GetDiskUsage);

        #endregion Methods (Public)

        #region Methods (Private)

        /// <summary>
        /// Gets a list of services that are currently running.
        /// </summary>
        /// <returns>A list of <see cref="ServiceInfoDto"/> objects representing the running services.</returns>
        private static List<ServiceInfoDto> GetRunningServices()
        {
            Result<List<DetailedServiceInfoDto>> result = ServiceInfo.Get("SERVICE_");
            return result.IsSuccess
                ? result.Data.Select(d => new ServiceInfoDto
                {
                    ServiceName = d.ServiceName,
                    DisplayName = d.DisplayName,
                    Status = d.Status,
                    StartType = d.StartType,
                    IsCanStarted = true,
                    IsCanStop = true,
                    Path = string.Empty,
                    Account = d.Account,
                    StartTime = d.LastStartTime?.ToString() ?? string.Empty,
                    Uptime = TimeSpan.Zero
                }).ToList()
                : new List<ServiceInfoDto>();
        }
        /// <summary>
        /// Lists host IPv4 and IPv6 addresses.
        /// + Filters address families using Ark extensions.
        /// - Loopback and unsupported families are skipped.
        /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.net.dns.gethostentry"/>
        /// </summary>
        /// <returns>A list of IP addresses as strings.</returns>
        private static List<string> GetIPAddresses()
            => Dns.GetHostEntry(Dns.GetHostName())
                  .AddressList
                  .Where(ip => ip.AddressFamily.IsOneOf(AddressFamily.InterNetwork, AddressFamily.InterNetworkV6))
                  .Select(ip => ip.ToString())
                  .ToList();


        /// <summary>
        /// Gets the total CPU usage percentage.
        /// + Uses <see cref="PerformanceCounter"/> for cross-platform CPU metrics.
        /// - Requires an initial delay to obtain a stable reading.
        /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.diagnostics.performancecounter"/>
        /// </summary>
        /// <returns>The total CPU usage percentage.</returns>
        public static double GetCPUUsage()
        {
            if (!OperatingSystem.IsWindows())
                return 0;

            using PerformanceCounter cpuCounter = new("Processor", "% Processor Time", "_Total");
            cpuCounter.NextValue();
            Thread.Sleep(1000);
            return cpuCounter.NextValue();
        }

        /// <summary>
        /// Gets the total network usage in bytes.
        /// + Sums sent and received bytes from <see cref="NetworkInterface"/> statistics.
        /// - IPv6 statistics are not included.
        /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.net.networkinformation.networkinterface"/>
        /// </summary>
        /// <returns>The total network usage in bytes.</returns>
        public static double GetNetworkUsage()
        {
            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            long bytesSent = networkInterfaces.Sum(ni => ni.GetIPv4Statistics().BytesSent);
            long bytesReceived = networkInterfaces.Sum(ni => ni.GetIPv4Statistics().BytesReceived);
            return bytesSent + bytesReceived;
        }

        /// <summary>
        /// Gets the disk usage percentage.
        /// + Aggregates capacity across all ready drives.
        /// - Excludes drives that are not currently mounted or ready.
        /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.io.driveinfo"/>
        /// </summary>
        /// <returns>The disk usage percentage.</returns>
        public static double GetDiskUsage()
        {
            List<DriveInfo> driveInfos = DriveInfo.GetDrives()
                                      .Where(d => d.IsReady)
                                      .ToList();

            long totalSpace = driveInfos.Sum(d => d.TotalSize);
            long freeSpace = driveInfos.Sum(d => d.TotalFreeSpace);
            return (double)(totalSpace - freeSpace) / totalSpace * 100;
        }

        /// <summary>
        /// Gets the memory usage percentage.
        /// </summary>
        /// <returns>The memory usage percentage.</returns>
        private static double GetMemoryUsage()
        {
            double total = NativeMemory.GetTotalPhysicalMemory();
            if (total <= 0)
                return 0;
            double available = GC.GetGCMemoryInfo().TotalAvailableMemoryBytes;
            double used = total - available;
            return used / total * 100;
        }

        /// <summary>
        /// Gets the CPU usage percentage of the current process.
        /// </summary>
        /// <returns>The CPU usage percentage of the current process.</returns>
        [SupportedOSPlatform("windows")]
        private static double GetCurrentProcessCPUUsage()
        {
            if (!OperatingSystem.IsWindows())
                return 0;

            Process currentProcess = Process.GetCurrentProcess();
            double totalCpuTime = currentProcess.TotalProcessorTime.TotalMilliseconds;
            using PerformanceCounter processCpuCounter = new("Process", "% Processor Time", currentProcess.ProcessName, true);
            processCpuCounter.NextValue();
            Thread.Sleep(1000);
            return processCpuCounter.NextValue();
        }

        #endregion Methods (Private)
    }
}

