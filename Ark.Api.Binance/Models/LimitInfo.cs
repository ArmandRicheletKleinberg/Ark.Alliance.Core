namespace Ark.Api.Binance
{
    /// <summary>
    /// Information about an API rate limit.
    /// + Indicates thresholds for requests and orders.
    /// - Does not include current usage metrics.
    /// </summary>
    public class LimitInfo
    {
        /// <summary>
        /// Maximum number of requests allowed.
        /// </summary>
        public int Limit { get; set; }

        /// <summary>
        /// Interval for the limit.
        /// </summary>
        public string Interval { get; set; } = string.Empty;

        /// <summary>
        /// Usage ratio at which alerts are emitted.
        /// </summary>
        public double AlertThreshold { get; set; }

        /// <summary>
        /// Usage ratio under which the limiter is considered recovered.
        /// </summary>
        public double RecoveryThreshold { get; set; }
    }
}
