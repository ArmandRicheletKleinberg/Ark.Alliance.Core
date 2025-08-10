using Ark;
using System.Threading.Tasks;

namespace Ark.Infrastructure.Info
{
    /// <summary>
    /// Abstraction for retrieving network information on the current platform.
    /// + Allows platform-specific implementations (Windows, Linux, etc.).
    /// - Exposes only a minimal set of diagnostics.
    /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.net.networkinformation"/>
    /// </summary>
    internal interface INetworkInfoProvider
    {
        /// <summary>
        /// Retrieves detailed network information for the host.
        /// + Synchronous for use in simple scripts.
        /// - May block while probing system interfaces.
        /// </summary>
        /// <returns>
        /// A <see cref="Result{HostNetworkInfoDto}"/> containing values like IP addresses and occupied ports.
        /// </returns>
        Result<HostNetworkInfoDto> GetNetworkInfo();

        /// <summary>
        /// Asynchronously retrieves detailed network information for the host.
        /// + Suitable for UI threads or high concurrency scenarios.
        /// - Still subject to underlying platform API latency.
        /// </summary>
        /// <returns>
        /// A task yielding the same <see cref="HostNetworkInfoDto"/> payload as <see cref="GetNetworkInfo"/>.
        /// </returns>
        Task<Result<HostNetworkInfoDto>> GetNetworkInfoAsync();
    }
}
