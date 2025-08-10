namespace Ark.App
{
    /// <summary>
    /// Represents the state transitions of a <see cref="HostedService"/>. The
    /// values are mainly used for diagnostics and to expose status information
    /// via monitoring endpoints.
    ///
    /// <para>Performance impact is negligible.</para>
    /// </summary>
    public enum HostedServiceLifecycleStatusEnum
    {
        /// <summary>
        /// The service has not yet started or has been stopped.
        /// </summary>
        Stopped = 0,

        /// <summary>
        /// The service is starting, initial execution if any is being done.
        /// </summary>
        Starting = 1,

        /// <summary>
        /// The service is started and is running, initial execution if any has being done.
        /// </summary>
        Running = 2,

        /// <summary>
        /// The service is currently stopping, the current execution are cancelled.
        /// </summary>
        Stopping = 3
    }
}