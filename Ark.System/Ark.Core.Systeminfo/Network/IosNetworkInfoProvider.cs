namespace Ark.Infrastructure.Info
{
    /// <summary>
    /// iOS implementation of <see cref="INetworkInfoProvider"/>.
    /// + Reuses <see cref="LinuxNetworkInfoProvider"/> for Darwin systems.
    /// - Lacks iOS-specific adapter metrics.
    /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.net.networkinformation.networkinterface"/>
    /// </summary>
    internal class IosNetworkInfoProvider : LinuxNetworkInfoProvider { }
}
