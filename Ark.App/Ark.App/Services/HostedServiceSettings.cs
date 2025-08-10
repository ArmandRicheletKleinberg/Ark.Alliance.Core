// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Ark.App
{
    /// <summary>
    /// Base configuration section used by <see cref="HostedService"/> and its
    /// derivatives. Values are typically loaded from <c>appsettings.json</c>.
    /// Using these settings allows administrators to enable/disable services and
    /// control whether they execute immediately when the application starts.
    ///
    /// <para>Advantage:</para> settings-driven behaviour avoids recompilation to
    /// change scheduling. <para>Disadvantage:</para> service startup reads
    /// configuration which adds a small amount of I/O.
    /// </summary>
    public class HostedServiceSettings
    {
        #region Properties (Public)

        /// <summary>
        /// Whether the service is enabled or not.
        /// If not enabled, it does not start at all.
        /// Default to true.
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Whether to execute the code execution as soon as the service starts.
        /// Default to false.
        /// </summary>
        public bool ExecuteWhenStart { get; set; } = false;

        #endregion Properties (Public)
    }

    /// <inheritdoc />
    public class HostedServiceSettings<TData> : HostedServiceSettings
    {
        #region Properties (Public)

        /// <summary>
        /// Some custom data used by the service.
        /// Optional.
        /// </summary>
        public TData Data { get; set; }

        #endregion Properties (Public)
    }
}