namespace Ark.Api.Binance.Services
{
    /// <summary>
    /// Configuration for latency management thresholds.
    /// + Provides control over warning and critical levels.
    /// - Values are applied globally across endpoints.
    /// Ref: <see href="https://learn.microsoft.com/dotnet/core/extensions/resilience"/>
    /// <example>
    /// {
    ///   "WarningLatencyThresholdMs": 1000,
    ///   "CriticalLatencyThresholdMs": 3000,
    ///   "AverageLatencyThresholdMs": 500,
    ///   "EnableEmergencyLiquidation": true,
    ///   "EnableOrderCancellation": true
    /// }
    /// </example>
    /// </summary>
    public class LatencyOptions
    {
        #region Properties

        /// <summary>
        /// Warning threshold in milliseconds beyond which latency is considered degraded.
        /// </summary>
        public decimal WarningLatencyThresholdMs { get; set; } = 1000;

        /// <summary>
        /// Critical threshold in milliseconds that triggers emergency handling.
        /// </summary>
        public decimal CriticalLatencyThresholdMs { get; set; } = 3000;

        /// <summary>
        /// Average latency limit in milliseconds used for trend monitoring.
        /// </summary>
        public decimal AverageLatencyThresholdMs { get; set; } = 500;

        /// <summary>
        /// Enables forced position liquidation when critical latency persists.
        /// + Adds safety during major outages.
        /// - May exit profitable trades prematurely.
        /// </summary>
        public bool EnableEmergencyLiquidation { get; set; } = true;

        /// <summary>
        /// Cancels outstanding orders when critical latency is observed.
        /// </summary>
        public bool EnableOrderCancellation { get; set; } = true;

        #endregion Properties
    }
}
