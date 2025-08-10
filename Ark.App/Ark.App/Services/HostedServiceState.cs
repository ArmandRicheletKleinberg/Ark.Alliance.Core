using System;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Ark.App
{
    /// <summary>
    /// Simple DTO that exposes runtime information for a
    /// <see cref="HostedService"/>. This can be serialised and returned from a
    /// monitoring endpoint to understand what the service is doing.
    /// Overhead for maintaining this structure is minimal.
    /// </summary>
    public class HostedServiceState
    {
        #region Properties (Public)

        /// <summary>
        /// The service settings.
        /// </summary>
        public HostedServiceSettings Settings { get; set; }

        /// <summary>
        /// The time when the service has started.
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// The last time the service code has executed.
        /// </summary>
        public DateTime? LastExecutionTime { get; set; }

        /// <summary>
        /// The lifecycle status of the hosted service.
        /// </summary>
        public HostedServiceLifecycleStatusEnum LifecycleStatus { get; set; }

        #endregion Properties (Public)
    }
}
