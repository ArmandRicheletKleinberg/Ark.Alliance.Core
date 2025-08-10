using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ark;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Ark.Api.Binance.Services
{
    /// <summary>
    /// Coordinates collection and evaluation of latency metrics.
    /// + Persists <see cref="LatencyMeasurement"/> records for diagnostics.
    /// + Triggers threshold alerts from <see cref="LatencyOptions"/>.
    /// - Maintains in-memory history which grows with high traffic.
    /// Ref: <see href="https://learn.microsoft.com/dotnet/core/diagnostics/"/>
    /// </summary>
    public class LatencyManagementService
    {
        #region Fields

        private readonly BinanceDbContext _context;
        private readonly ILogger<LatencyManagementService> _logger;
        private readonly IOptionsMonitor<LatencyOptions> _options;

        private readonly Dictionary<string, List<LatencyMeasurement>> _recentMeasurements = new();
        private readonly object _measurementsLock = new();

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Creates a new management service for latency tracking.
        /// </summary>
        /// <param name="context">Database context storing measurements.</param>
        /// <param name="logger">Logger for diagnostics output.</param>
        /// <param name="options">Monitored latency thresholds.</param>
        public LatencyManagementService(
            BinanceDbContext context,
            ILogger<LatencyManagementService> logger,
            IOptionsMonitor<LatencyOptions> options)
        {
            _context = context;
            _logger = logger;
            _options = options;
        }

        #endregion Constructors

        #region Methods (Public)

        /// <summary>
        /// Begins measuring latency for a specific endpoint.
        /// + Returns a tracker that records start/stop timestamps.
        /// - Caller must dispose the tracker to persist data.
        /// </summary>
        /// <param name="endpoint">Binance endpoint name.</param>
        /// <param name="requestType">Communication type such as <c>REST</c> or <c>WS</c>.</param>
        /// <returns>Disposable tracker that reports latency on completion.</returns>
        public LatencyTracker StartLatencyMeasurement(string endpoint, string requestType = "REST")
        {
            return new LatencyTracker(this, endpoint, requestType);
        }

        /// <summary>
        /// Persists and evaluates a completed latency measurement.
        /// + Stores metrics to <see cref="BinanceDbContext"/>.
        /// + Returns operation outcome for caller diagnostics.
        /// - Database write may impact throughput under load.
        /// Ref: <see href="https://learn.microsoft.com/dotnet/core/diagnostics/"/>
        /// </summary>
        /// <param name="measurement">Measured latency information.</param>
        /// <returns>A <see cref="Result"/> indicating success or unexpected failure.</returns>
        public async ValueTask<Result> RecordLatencyMeasurement(LatencyMeasurement measurement)
        {
            try
            {
                var entity = new LatencyMeasurementDbEntity
                {
                    Endpoint = measurement.Endpoint,
                    RequestType = measurement.RequestType,
                    RequestStartTime = measurement.RequestStartTime,
                    ResponseReceivedTime = measurement.ResponseReceivedTime,
                    BinanceTimestamp = measurement.BinanceTimestamp,
                    TotalLatencyMs = measurement.TotalLatencyMs,
                    NetworkLatencyMs = measurement.NetworkLatencyMs,
                    ProcessingLatencyMs = measurement.ProcessingLatencyMs,
                    IsSuccessful = measurement.IsSuccessful,
                    ErrorCode = measurement.ErrorCode,
                    AdditionalDataJson = measurement.AdditionalDataJson ?? "{}",
                };

                _context.LatencyMeasurements.Add(entity);
                await _context.SaveChangesAsync();

                UpdateRecentMeasurements(measurement);
                await CheckLatencyThresholds(measurement);
                return Result.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'enregistrement de latence pour {Endpoint}",
                    measurement.Endpoint);
                return Result.Unexpected;
            }
        }

        #endregion Methods (Public)

        #region Methods (Private)

        private async Task CheckLatencyThresholds(LatencyMeasurement measurement)
        {
            var options = _options.CurrentValue;

            if (measurement.TotalLatencyMs > options.CriticalLatencyThresholdMs)
            {
                _logger.LogWarning(
                    "🚨 LATENCE CRITIQUE détectée sur {Endpoint}: {Latency}ms",
                    measurement.Endpoint, measurement.TotalLatencyMs);

                await TriggerEmergencyActions(measurement);
            }
        }

        private Task TriggerEmergencyActions(LatencyMeasurement measurement)
        {
            var options = _options.CurrentValue;

            if (options.EnableEmergencyLiquidation)
            {
                _logger.LogCritical(
                    "🆘 LIQUIDATION D'URGENCE déclenchée due à la latence critique sur {Endpoint}",
                    measurement.Endpoint);
            }

            return Task.CompletedTask;
        }

        private void UpdateRecentMeasurements(LatencyMeasurement measurement)
        {
            lock (_measurementsLock)
            {
                var measurements = _recentMeasurements.GetValue(measurement.Endpoint, new List<LatencyMeasurement>());
                measurements.Add(measurement);

                if (measurements.Count > 100)
                    measurements.RemoveAt(0);

                _recentMeasurements.AddOrUpdate(measurement.Endpoint, measurements);
            }
        }

        #endregion Methods (Private)
    }
}

