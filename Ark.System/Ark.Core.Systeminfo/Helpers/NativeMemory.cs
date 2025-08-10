using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Ark.Infrastructure.Info
{
    /// <summary>
    /// Provides cross-platform helpers to query total physical memory.
    /// + Uses native APIs or <c>/proc/meminfo</c> to deliver accurate totals.
    /// - Returns <c>0</c> when platform details cannot be determined.
    /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.runtime.interopservices.runtimeinformation"/>
    /// </summary>
    internal static class NativeMemory
    {
        #region Methods (Public)

        /// <summary>
        /// Retrieves total physical memory in bytes.
        /// + Enables environment snapshots to compute usage percentages.
        /// - On unsupported platforms the value may be approximate or zero.
        /// </summary>
        /// <returns>Total physical memory in bytes.</returns>
        internal static double GetTotalPhysicalMemory()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                MEMORYSTATUSEX status = new();
                if (GlobalMemoryStatusEx(status))
                    return status.ullTotalPhys;
                return 0;
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                try
                {
                    foreach (string line in File.ReadLines("/proc/meminfo"))
                    {
                        if (line.StartsWith("MemTotal:"))
                        {
                            string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                            if (parts.Length >= 2 && ulong.TryParse(parts[1], out ulong kb))
                                return kb * 1024d;
                        }
                    }
                }
                catch { }
                return 0;
            }
            return GC.GetGCMemoryInfo().TotalAvailableMemoryBytes;
        }

        #endregion Methods (Public)

        #region ApiWindows

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private sealed class MEMORYSTATUSEX
        {
            public uint dwLength = (uint)Marshal.SizeOf<MEMORYSTATUSEX>();
            public uint dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        [SupportedOSPlatform("windows")]
        private static extern bool GlobalMemoryStatusEx([In, Out] MEMORYSTATUSEX lpBuffer);

        #endregion ApiWindows
    }
}
