using System;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Ark.App
{
    /// <summary>
    /// Settings dedicated to <see cref="ScheduledHostedService"/>. These values
    /// specify when and how often the service should run. They are usually read
    /// from configuration files.
    ///
    /// Changing the schedule does not require recompiling the service, which is
    /// the main advantage. The overhead of reading these values at startup is
    /// trivial.
    /// </summary>
    public class ScheduledHostedServiceSettings : HostedServiceSettings
    {
        #region Properties (Public)

        /// <summary>
        /// The scheduled laps to repeat the service code execution every timespan.
        /// </summary>
        public TimeSpan ScheduledLaps { get; set; }

        /// <summary>
        /// The scheduled local time to start firstly the code execution.
        /// Optional.
        /// </summary>
        public DateTime? ScheduledStartLocalTime { get; set; }

        /// <summary>
        /// The scheduled UTC time to start firstly the code execution.
        /// Optional.
        /// </summary>
        public DateTime? ScheduledStartUtcTime { get; set; }

        #endregion Properties (Public)
    }

    /// <inheritdoc />
    public class ScheduledHostedServiceSettings<TData> : ScheduledHostedServiceSettings
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
