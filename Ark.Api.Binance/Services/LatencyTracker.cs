using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Ark;

namespace Ark.Api.Binance.Services
{
    /// <summary>
    /// Tracks latency for a single request.
    /// + Records success or failure measurements
    /// - Does not guard against concurrent reuse
    /// </summary>
    public class LatencyTracker : IDisposable
    {
        private readonly LatencyManagementService _service;
        private readonly Stopwatch _stopwatch;
        private readonly LatencyMeasurement _measurement;

        /// <summary>
        /// Initializes a new instance of the <see cref="LatencyTracker"/> class.
        /// </summary>
        /// <param name="service">Management service persisting measurements.</param>
        /// <param name="endpoint">Endpoint being measured.</param>
        /// <param name="requestType">Protocol identifier such as <c>REST</c> or <c>WS</c>.</param>
        public LatencyTracker(LatencyManagementService service, string endpoint, string requestType)
        {
            _service = service;
            _stopwatch = Stopwatch.StartNew();
            _measurement = new LatencyMeasurement
            {
                Endpoint = endpoint,
                RequestType = requestType,
                RequestStartTime = DateTime.UtcNow,
                MeasuredAt = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Completes the measurement successfully.
        /// + Persists latency details
        /// + Returns storage result for optional inspection
        /// - Assumes single completion per tracker
        /// Ref: <see href="https://learn.microsoft.com/dotnet/core/diagnostics/"/>
        /// </summary>
        /// <param name="binanceTimestamp">Timestamp reported by Binance.</param>
        /// <param name="additionalData">Optional JSON payload for diagnostics.</param>
        /// <returns>A <see cref="Result"/> describing the persistence outcome.</returns>
        public async ValueTask<Result> CompleteAsync(DateTime? binanceTimestamp = null, string? additionalData = null)
        {
            _stopwatch.Stop();
            _measurement.ResponseReceivedTime = DateTime.UtcNow;
            _measurement.BinanceTimestamp = binanceTimestamp;
            _measurement.TotalLatencyMs = (decimal)_stopwatch.ElapsedMilliseconds;
            _measurement.IsSuccessful = true;
            _measurement.AdditionalDataJson = additionalData ?? "{}";

            return await _service.RecordLatencyMeasurement(_measurement);
        }

        /// <summary>
        /// Completes the tracker with an error code.
        /// + Triggers async persistence of failed measurement
        /// - Does not await completion of the underlying storage
        /// </summary>
        /// <param name="errorCode">Machine-readable error identifier.</param>
        public void CompleteWithError(string errorCode)
        {
            _ = CompleteWithErrorAsync(errorCode);
        }

        /// <summary>
        /// Disposes the tracker, recording an error if measurement was not completed.
        /// </summary>
        public void Dispose()
        {
            if (_measurement.ResponseReceivedTime == default)
            {
                _ = CompleteWithErrorAsync("INCOMPLETE_MEASUREMENT");
            }
        }

        private async ValueTask<Result> CompleteWithErrorAsync(string errorCode)
        {
            _stopwatch.Stop();
            _measurement.ResponseReceivedTime = DateTime.UtcNow;
            _measurement.TotalLatencyMs = (decimal)_stopwatch.ElapsedMilliseconds;
            _measurement.IsSuccessful = false;
            _measurement.ErrorCode = errorCode;

            return await _service.RecordLatencyMeasurement(_measurement);
        }
    }
}
