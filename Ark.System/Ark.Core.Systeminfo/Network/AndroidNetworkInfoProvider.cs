namespace Ark.Infrastructure.Info
{
    /// <summary>
    /// Android implementation of <see cref="INetworkInfoProvider"/>.
    /// + Inherits Linux provider to cover Android's Linux kernel.
    /// - Omits mobile radio metrics and permissions checks.
    /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.net.networkinformation.networkinterface"/>
    /// </summary>
    internal class AndroidNetworkInfoProvider : LinuxNetworkInfoProvider { }
}
