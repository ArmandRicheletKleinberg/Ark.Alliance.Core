using Ark;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace Ark.Infrastructure.Info
{
    /// <summary>
    /// Provides network diagnostics for Linux-based systems.
    /// + Enumerates occupied ports and adapter statistics.
    /// - Uses 500 ms sampling which blocks the calling thread.
    /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.net.networkinformation.networkinterface"/>
    /// </summary>
    internal class LinuxNetworkInfoProvider : INetworkInfoProvider
    {
        /// <summary>
        /// Gets network diagnostics for the host.
        /// + Includes port usage and adapter throughput.
        /// - Requires synchronous sampling for bandwidth calculation.
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

                IPGlobalProperties props = IPGlobalProperties.GetIPGlobalProperties();
                info.OpenPorts.AddRange(props.GetActiveTcpListeners().Select(e => e.Port));
                info.OpenPorts.AddRange(props.GetActiveUdpListeners().Select(e => e.Port));

                foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
                {
                    NetworkAdapterInfoDto adapter = new()
                    {
                        Name = ni.Name,
                        Description = ni.Description,
                        Id = ni.Id,
                        IsDhcpEnabled = false,
                        Addresses = ni.GetIPProperties().UnicastAddresses.Select(a => a.Address.ToString()).ToList(),
                        Speed = ni.Speed
                    };

                    IPv4InterfaceStatistics start = ni.GetIPv4Statistics();
                    long begin = start.BytesReceived + start.BytesSent;
                    Thread.Sleep(500);
                    IPv4InterfaceStatistics end = ni.GetIPv4Statistics();
                    long finish = end.BytesReceived + end.BytesSent;
                    long delta = finish - begin;
                    double capacity = ni.Speed / 8d;
                    adapter.UsagePercentage = capacity > 0 ? delta * 100d / capacity : 0;

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
        /// + Offloads synchronous work to the thread pool via <see cref="Task.Run(System.Action)"/>.
        /// - Still blocks a thread during sampling.
        /// </summary>
        /// <returns>Task producing a <see cref="Result{T}"/> with <see cref="HostNetworkInfoDto"/>.</returns>
        public Task<Result<HostNetworkInfoDto>> GetNetworkInfoAsync() => Task.Run(GetNetworkInfo);
    }
}
