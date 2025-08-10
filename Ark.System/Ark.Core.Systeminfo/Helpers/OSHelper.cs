namespace Ark.Infrastructure.Info
{
    /// <summary>
    /// Helper methods related to operating system detection.
    /// + Wraps <see cref="OperatingSystem"/> helpers for concise platform checks.
    /// - Mirrors the <see cref="OperatingSystemKind"/> enumeration; new platforms require updates.
    /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.operatingsystem"/>
    /// </summary>
    public static class OSHelper
    {
        /// <summary>
        /// Determines the current operating system.
        /// + Uses built-in platform checks avoiding manual `RuntimeInformation` calls.
        /// - Adds branching for each supported platform.
        /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.operatingsystem.iswindows"/>
        /// </summary>
        /// <returns>The detected <see cref="OperatingSystemKind"/>.</returns>
        public static OperatingSystemKind GetCurrentPlatform()
        {
            if (OperatingSystem.IsWindows())
                return OperatingSystemKind.Windows;
            if (OperatingSystem.IsLinux())
                return OperatingSystemKind.Linux;
            if (OperatingSystem.IsMacOS())
                return OperatingSystemKind.MacOs;
            if (OperatingSystem.IsAndroid())
                return OperatingSystemKind.Android;
            if (OperatingSystem.IsIOS())
                return OperatingSystemKind.Ios;
            return OperatingSystemKind.Unknown;
        }
    }
}
