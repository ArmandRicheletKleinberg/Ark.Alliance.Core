using Ark;
using System;
using System.Threading.Tasks;
using System.Runtime.Versioning;

namespace Ark.Infrastructure.Info
{
    /// <summary>
    /// Entry point to retrieve network information across platforms.
    /// + Dispatches to platform-specific providers via <see cref="PlatformProvider"/>.
    /// - Returns minimal information on unsupported operating systems.
    /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.net.networkinformation"/>
    /// </summary>
    public static class NetworkInfoProvider
    {
        #region Fields

        /// <summary>
        /// Platform-specific implementation selected at runtime.
        /// </summary>
        #pragma warning disable CA1416 // Platform validation
        private static readonly INetworkInfoProvider _provider = PlatformProvider.Create<INetworkInfoProvider>(
            CreateWindowsNetworkInfoProvider,
            () => new LinuxNetworkInfoProvider(),
            () => new AndroidNetworkInfoProvider(),
            () => new IosNetworkInfoProvider(),
            () => new DefaultNetworkInfoProvider());
        #pragma warning restore CA1416

        #endregion Fields

        #region Methods (Public)

        /// <summary>
        /// Retrieves aggregated network information for the host.
        /// + Useful for diagnostics and monitoring dashboards.
        /// - May incur network interface enumeration overhead.
        /// </summary>
        /// <returns>
        /// A <see cref="Result{HostNetworkInfoDto}"/> whose <see cref="Result{T}.Data"/> resembles:
        /// <code>
        /// {
        ///   "ipAddress": "192.168.1.10",
        ///   "occupiedPorts": []
        /// }
        /// </code>
        /// </returns>
        public static Result<HostNetworkInfoDto> GetNetworkInfo() => _provider.GetNetworkInfo();

        /// <summary>
        /// Asynchronously retrieves aggregated network information for the host.
        /// + Avoids blocking the caller during network enumeration.
        /// - Still executed sequentially on the provider side.
        /// </summary>
        /// <returns>
        /// A task producing the same payload as <see cref="GetNetworkInfo"/> in JSON form.
        /// </returns>
        public static Task<Result<HostNetworkInfoDto>> GetNetworkInfoAsync() => _provider.GetNetworkInfoAsync();

        #endregion Methods (Public)

        private class DefaultNetworkInfoProvider : INetworkInfoProvider
        {
            public Result<HostNetworkInfoDto> GetNetworkInfo() => Result<HostNetworkInfoDto>.Success.WithData(new HostNetworkInfoDto());
            public Task<Result<HostNetworkInfoDto>> GetNetworkInfoAsync() => Task.FromResult(GetNetworkInfo());
        }

        [SupportedOSPlatform("windows")]
        private static INetworkInfoProvider CreateWindowsNetworkInfoProvider() => new WindowsNetworkInfoProvider();
    }
}
