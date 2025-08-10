
using System;
using Ark;
using Ark.App.Diagnostics;
using Binance.Net.Clients;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Resilience;
using Ark.Api.Binance.Helpers;
using Ark.Api.Binance.Services;

namespace Ark.Api.Binance
{
    /// <summary>
    /// Provides a singleton instance of <see cref="BinanceApiClient"/> with
    /// automatic recovery and rate limiting.
    /// + Enforces concurrency limits and retries for robustness.
    /// - Global singleton may become a bottleneck under extreme load.
    /// </summary>
    /// <remarks>
    /// The manager should be configured once at application startup using
    /// <see cref="Configure"/>.
    /// </remarks>
    /// <example>
    /// <code>
    /// BinanceApiClientManager.Configure(options);
    /// var result = await BinanceApiClientManager.ExecuteAsync((c,t) => c.GetTickerAsync("BTCUSDT", t), "default", CancellationToken.None);
    /// </code>
    /// </example>
    public static class BinanceApiClientManager
    {
        #region Fields

        private static BinanceApiClient? _client;
        private static IOptions<BinanceOptions>? _options;
        private static ILogger? _logger;
        private static readonly object _lock = new();
        private static SemaphoreSlim? _concurrency;
        private static Dictionary<string, EnhancedRateLimiter>? _rateLimits;
        private static ResiliencePipeline<Result>? _retryPipeline;
        private static RateLimitAnalyzer? _rateLimitAnalyzer;

        static BinanceApiClientManager()
        {
            _logger = Diag.Logs?.BinanceApiClientManager ?? NullLogger.Instance;
        }

        #endregion Fields

        /// <summary>
        /// Configures the manager with options and logger.
        /// </summary>
        /// <param name="options">The Binance options.</param>
        /// <remarks>
        /// Must be called before any execution methods.
        /// </remarks>
        /// <example>
        /// <code>
        /// BinanceApiClientManager.Configure(myOptions);
        /// </code>
        /// </example>
        #region Methods (Public)

        public static void Configure(IOptions<BinanceOptions> options)
        {
            lock (_lock)
            {
                _options ??= options;
                _logger ??= Diag.Logs?.BinanceApiClientManager ?? NullLogger.Instance;
                _concurrency ??= new SemaphoreSlim(options.Value.MaxConcurrentRequests);
                _rateLimits ??= options.Value.Limits.ToDictionary(k => k.Key, v => new EnhancedRateLimiter(v.Value));
                _client ??= new BinanceApiClient(options);
                Diag.ApplyLogLevel(options.Value.LogLevel);
                _rateLimitAnalyzer ??= new RateLimitAnalyzer();
                if (_retryPipeline == null)
                {
                    _retryPipeline = new ResiliencePipelineBuilder<Result>()
                        .AddRetry(o => o.MaxRetryAttempts = options.Value.RetryCount)
                        .Build();
                }
            }
        }

        /// <summary>
        /// Executes an operation on the <see cref="BinanceApiClient"/> ensuring
        /// the client is alive and rate limits are respected.
        /// </summary>
        /// <param name="action">Delegate using the managed client.</param>
        /// <param name="limitKey">Identifier of the rate limiter to use.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>The result returned by the delegate.</returns>
        public static async Task<Result> ExecuteAsync(Func<BinanceApiClient, CancellationToken, Task<Result>> action, string limitKey, CancellationToken token)
        {
            if (_options == null || _logger == null || _concurrency == null || _rateLimits == null)
                return Result.Unexpected.WithReason("BinanceApiClientManager not configured");

            await _concurrency.WaitAsync(token);
            try
            {
                await EnsureAliveAsync(token);
                EnhancedRateLimiter? limiter = null;
                if (_rateLimits.TryGetValue(limitKey, out limiter))
                    await limiter.WaitAsync(token);

                var result = await _retryPipeline!.ExecuteAsync(ct => action(_client!, ct), token);
                limiter?.RecordSuccess();
                return result;
            }
            catch (Exception ex)
            {
                if (_rateLimits.TryGetValue(limitKey, out var limiter))
                    limiter.RecordFailure();
                _logger!.LogError(ex, "Error during Binance call");
                RestartClient();
                return Result.Failure.WithException(ex);
            }
            finally
            {
                _concurrency.Release();
            }
        }

        /// <summary>
        /// Executes an operation returning <see cref="Result{T}"/> on the <see cref="BinanceApiClient"/>.
        /// </summary>
        /// <param name="action">Delegate using the managed client.</param>
        /// <param name="limitKey">Identifier of the rate limiter.</param>
        /// <param name="token">Cancellation token.</param>
        /// <typeparam name="T">Type returned in the result.</typeparam>
        /// <returns>A <see cref="Result{T}"/> from the delegate.</returns>
        public static async Task<Result<T>> ExecuteAsync<T>(Func<BinanceApiClient, CancellationToken, Task<Result<T>>> action, string limitKey, CancellationToken token)
        {
            if (_options == null || _logger == null || _concurrency == null || _rateLimits == null)
                return Result<T>.Unexpected.WithReason("BinanceApiClientManager not configured");

            await _concurrency.WaitAsync(token);
            try
            {
                await EnsureAliveAsync(token);
                EnhancedRateLimiter? limiter = null;
                if (_rateLimits.TryGetValue(limitKey, out limiter))
                    await limiter.WaitAsync(token);

                var pipeline = new ResiliencePipelineBuilder<Result<T>>()
                    .AddRetry(o => o.MaxRetryAttempts = _options.Value.RetryCount)
                    .Build();
                var result = await pipeline.ExecuteAsync(ct => action(_client!, ct), token);
                limiter?.RecordSuccess();
                return result;
            }
            catch (Exception ex)
            {
                if (_rateLimits.TryGetValue(limitKey, out var limiter))
                    limiter.RecordFailure();
                _logger!.LogError(ex, "Error during Binance call");
                RestartClient();
                return Result<T>.Failure.WithException(ex);
            }
            finally
            {
                _concurrency.Release();
            }
        }

        /// <summary>
        /// Gets the current usage ratio of the specified rate limiter.
        /// </summary>
        /// <param name="limitKey">Identifier of the rate limiter.</param>
        /// <returns>A value between 0 and 1 representing the usage.</returns>
        public static double GetRateLimitUsage(string limitKey)
        {
            if (_rateLimits != null && _rateLimits.TryGetValue(limitKey, out var limiter))
                return limiter.Usage;
            return 0d;
        }

        /// <summary>
        /// Determines if the specified limiter has crossed its alert threshold.
        /// </summary>
        /// <param name="limitKey">Identifier of the rate limiter.</param>
        /// <returns><c>true</c> when usage exceeds the alert threshold.</returns>
        public static bool IsApproachingLimit(string limitKey)
        {
            if (_rateLimits != null && _rateLimits.TryGetValue(limitKey, out var limiter))
                return limiter.IsApproachingLimit();
            return false;
        }

        /// <summary>
        /// Determines if the specified limiter has recovered below its threshold.
        /// </summary>
        /// <param name="limitKey">Identifier of the rate limiter.</param>
        /// <returns><c>true</c> when usage is below the recovery threshold.</returns>
        public static bool IsBelowRecoveryLimit(string limitKey)
        {
            if (_rateLimits != null && _rateLimits.TryGetValue(limitKey, out var limiter))
                return limiter.IsBelowRecoveryThreshold();
            return true;
        }

        /// <summary>
        /// Evaluates a batch of API calls against the configured limits.
        /// + Assists planning to avoid weight violations.
        /// - Uses static rules; server-side changes are not reflected automatically.
        /// </summary>
        /// <param name="calls">Planned API calls.</param>
        /// <returns>Total weight and whether the limit would be exceeded.</returns>
        public static (int totalWeight, bool exceedsLimit) AnalyzeBatch(IEnumerable<RateLimitAnalyzer.ApiCall> calls)
        {
            _rateLimitAnalyzer ??= new RateLimitAnalyzer();
            return _rateLimitAnalyzer.CalculateWeightUsage(calls);
        }

        /// <summary>
        /// Pings the Binance API and recreates the client if unreachable.
        /// </summary>
        /// <param name="token">Cancellation token.</param>
        #endregion Methods (Public)

        #region Methods (Private)

        private static async Task EnsureAliveAsync(CancellationToken token)
        {
            try
            {
                var ping = await _client!.GetExchangeInfoAsync(token);
                if (ping.IsNotSuccess)
                    throw new Exception(ping.Reason ?? "Ping failed");
            }
            catch (Exception ex)
            {
                _logger!.LogWarning(ex, "Binance client unreachable - recreating instance");
                RestartClient();
            }
        }

        /// <summary>
        /// Recreates the internal <see cref="BinanceApiClient"/> instance.
        /// </summary>
        private static void RestartClient()
        {
            lock (_lock)
            {
                _client = new BinanceApiClient(_options!);
            }
        }

        #endregion Methods (Private)
    }
}
