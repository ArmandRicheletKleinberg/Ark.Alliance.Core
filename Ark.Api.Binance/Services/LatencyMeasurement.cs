using System;

namespace Ark.Api.Binance.Services
{
    /// <summary>
    /// Represents a single latency data point.
    /// + Captures timing and success flags.
    /// - Lacks network/processing separation by default.
    /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.datetime"/>
    /// </summary>
    public class LatencyMeasurement
    {
        #region Properties

        /// <summary>
        /// Name of the endpoint being measured.
        /// </summary>
        public string Endpoint { get; set; } = string.Empty;

        /// <summary>
        /// HTTP method or request type.
        /// </summary>
        public string RequestType { get; set; } = string.Empty;

        /// <summary>
        /// UTC timestamp when the request was sent.
        /// </summary>
        public DateTime RequestStartTime { get; set; }

        /// <summary>
        /// UTC timestamp when the response was received.
        /// </summary>
        public DateTime ResponseReceivedTime { get; set; }

        /// <summary>
        /// Binance-provided timestamp, if available.
        /// </summary>
        public DateTime? BinanceTimestamp { get; set; }

        /// <summary>
        /// Total round-trip latency in milliseconds.
        /// </summary>
        public decimal TotalLatencyMs { get; set; }

        /// <summary>
        /// Estimated network latency in milliseconds.
        /// </summary>
        public decimal NetworkLatencyMs { get; set; }

        /// <summary>
        /// Estimated processing latency in milliseconds.
        /// </summary>
        public decimal ProcessingLatencyMs { get; set; }

        /// <summary>
        /// Indicates whether the request succeeded.
        /// + Useful for reliability tracking.
        /// - Does not capture partial successes.
        /// </summary>
        public bool IsSuccessful { get; set; }

        /// <summary>
        /// Binance error code if the request failed.
        /// </summary>
        public string? ErrorCode { get; set; }

        /// <summary>
        /// Time when the measurement was recorded.
        /// </summary>
        public DateTime MeasuredAt { get; set; }

        /// <summary>
        /// Additional metadata in JSON format.
        /// + Allows attaching custom diagnostic data.
        /// </summary>
        /// <example>
        /// {
        ///   "symbol": "BTCUSDT",
        ///   "recvWindow": 5000
        /// }
        /// </example>
        public string? AdditionalDataJson { get; set; }

        #endregion Properties
    }
}
