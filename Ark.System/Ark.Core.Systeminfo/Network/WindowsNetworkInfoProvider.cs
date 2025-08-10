using Ark;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.Versioning;

namespace Ark.Infrastructure.Info
{
    /// <summary>
    /// Provides network adapter and port diagnostics for Windows hosts.
    /// + Aggregates occupied and open ports with usage percentages.
    /// - Samples throughput by sleeping, which blocks the calling thread.
    /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.net.networkinformation.networkinterface"/>
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal class WindowsNetworkInfoProvider : INetworkInfoProvider
    {
        /// <summary>
        /// Gets network diagnostics for the host.
        /// + Includes adapter statistics and port lists.
        /// - Requires synchronous sampling for 500 ms per adapter.
        /// </summary>
        /// <returns>A <see cref="Result{T}"/> containing <see cref="HostNetworkInfoDto"/>.</returns>
        public Result<HostNetworkInfoDto> GetNetworkInfo()
        {
            try
            {
                HostNetworkInfoDto info = new HostNetworkInfoDto
                {
                    OccupiedPorts = NetworkHelper.GetOccupiedPorts().Data
                };

                info.OpenPorts.AddRange(IPGlobalProperties.GetIPGlobalProperties()
                    .GetActiveTcpListeners()
                    .Select(e => e.Port));
                info.OpenPorts.AddRange(IPGlobalProperties.GetIPGlobalProperties()
                    .GetActiveUdpListeners()
                    .Select(e => e.Port));

                foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
                {
                    NetworkAdapterInfoDto adapter = new()
                    {
                        Name = ni.Name,
                        Description = ni.Description,
                        Id = ni.Id,
                        IsDhcpEnabled = ni.GetIPProperties().GetIPv4Properties()?.IsDhcpEnabled ?? false,
                        Addresses = ni.GetIPProperties().UnicastAddresses.Select(a => a.Address.ToString()).ToList(),
                        Speed = ni.Speed
                    };

                    IPv4InterfaceStatistics startStats = ni.GetIPv4Statistics();
                    long start = startStats.BytesReceived + startStats.BytesSent;
                    Thread.Sleep(500);
                    IPv4InterfaceStatistics endStats = ni.GetIPv4Statistics();
                    long end = endStats.BytesReceived + endStats.BytesSent;
                    long delta = end - start;
                    double capacityPerSec = ni.Speed / 8d;
                    adapter.UsagePercentage = capacityPerSec > 0 ? delta * 100d / capacityPerSec : 0;

                    info.Adapters.Add(adapter);
                }

                return Result<HostNetworkInfoDto>.Success.WithData(info);
            }
            catch (Exception ex)
            {
                return Result<HostNetworkInfoDto>.Failure.WithException(ex);
            }
        }

        /// <summary>
        /// Asynchronously gets network diagnostics.
        /// + Offloads work to the thread pool via <see cref="Task.Run(System.Action)"/>.
        /// - Still performs blocking operations internally.
        /// </summary>
        /// <returns>Task producing a <see cref="Result{T}"/> with <see cref="HostNetworkInfoDto"/>.</returns>
        public Task<Result<HostNetworkInfoDto>> GetNetworkInfoAsync() => Task.Run(GetNetworkInfo);
    }
}
