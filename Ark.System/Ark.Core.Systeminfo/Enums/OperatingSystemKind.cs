namespace Ark.Infrastructure.Info
{
    /// <summary>
    /// Defines host operating system categories.
    /// + Enables platform-specific providers to adapt behavior.
    /// - Collapses detailed distributions into generic values.
    /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.operatingsystem"/>
    /// </summary>
    public enum OperatingSystemKind
    {
        /// <summary>
        /// Microsoft Windows family.
        /// </summary>
        Windows,

        /// <summary>
        /// Linux distributions including Ubuntu.
        /// </summary>
        Linux,

        /// <summary>
        /// Apple macOS.
        /// </summary>
        MacOs,

        /// <summary>
        /// Google Android.
        /// </summary>
        Android,

        /// <summary>
        /// Apple iOS.
        /// </summary>
        Ios,

        /// <summary>
        /// Unknown or unsupported OS.
        /// </summary>
        Unknown
    }
}
