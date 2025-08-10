using System;
using System.Runtime.InteropServices;

namespace Ark.Infrastructure.Info
{
    /// <summary>
    /// Provides helper methods to select platform specific implementations.
    /// + Centralizes runtime OS checks in one place.
    /// - Requires supplying factories for all supported platforms.
    /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.runtime.interopservices.runtimeinformation"/>
    /// </summary>
    public static class PlatformProvider
    {
        #region Methods

        /// <summary>
        /// Selects a platform-specific implementation using runtime OS checks.
        /// + Executes the function corresponding to the detected platform.
        /// - Falls back to <paramref name="default"/> when the OS is unsupported.
        /// </summary>
        /// <typeparam name="T">Return type of the factory methods.</typeparam>
        /// <param name="windows">Factory for Windows.</param>
        /// <param name="linux">Factory for Linux.</param>
        /// <param name="android">Factory for Android.</param>
        /// <param name="ios">Factory for iOS/macOS.</param>
        /// <param name="default">Fallback factory when platform is unknown.</param>
        /// <returns>The value produced by the matching factory.</returns>
        public static T Create<T>(Func<T> windows, Func<T> linux, Func<T> android, Func<T> ios, Func<T> @default)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return windows();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return linux();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Create("ANDROID")))
                return android();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return ios();
            return @default();
        }

        #endregion Methods
    }
}
