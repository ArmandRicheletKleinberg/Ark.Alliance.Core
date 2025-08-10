using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Ark.Api.Binance.Helpers;

#nullable enable

namespace Ark.Api.Binance.Resilience
{
    /// <summary>
    /// Production-ready resilience pipeline with circuit breaker, retry, and bulkhead patterns
    /// + Implements sophisticated failure handling
    /// + Provides metrics and observability
    /// + Configurable policies per operation type
    /// + Cache-aside for idempotent GET operations
    /// </summary>
    public class RealResiliencePipeline<TResult> : IDisposable
    {
        private readonly RetryPolicy _retryPolicy;
        private readonly CircuitBreakerPolicy _circuitBreakerPolicy;
        private readonly BulkheadPolicy _bulkheadPolicy;
        private readonly ILogger _logger;
        private readonly EnhancedRateLimiter? _rateLimiter;
        private readonly ConcurrentDictionary<string, CacheEntry> _cache = new();
        private volatile bool _disposed;

        private readonly record struct CacheEntry(TResult Value, DateTime Expiry);

        /// <summary>
        /// Initializes a new instance of the pipeline with the specified resilience policies.
        /// </summary>
        /// <param name="retryPolicy">Policy handling transient failures.</param>
        /// <param name="circuitBreakerPolicy">Policy controlling circuit state transitions.</param>
        /// <param name="bulkheadPolicy">Policy limiting concurrent executions.</param>
        /// <param name="logger">Optional logger for diagnostics.</param>
        /// <param name="rateLimiter">Optional rate limiter for throttling.</param>
        public RealResiliencePipeline(
            RetryPolicy retryPolicy,
            CircuitBreakerPolicy circuitBreakerPolicy,
            BulkheadPolicy bulkheadPolicy,
            ILogger? logger = null,
            EnhancedRateLimiter? rateLimiter = null)
        {
            _retryPolicy = retryPolicy ?? throw new ArgumentNullException(nameof(retryPolicy));
            _circuitBreakerPolicy = circuitBreakerPolicy ?? throw new ArgumentNullException(nameof(circuitBreakerPolicy));
            _bulkheadPolicy = bulkheadPolicy ?? throw new ArgumentNullException(nameof(bulkheadPolicy));
            _logger = logger ?? NullLogger.Instance;
            _rateLimiter = rateLimiter;
        }

        /// <summary>
        /// Executes the provided operation with the configured resilience policies.
        /// </summary>
        /// <param name="operation">Delegate representing the work to execute.</param>
        /// <param name="cacheKey">Optional cache key for result caching.</param>
        /// <param name="cacheDuration">Optional time-to-live for cached results.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>The result of the operation.</returns>
        public async Task<TResult> ExecuteAsync(
            Func<CancellationToken, Task<TResult>> operation,
            string? cacheKey = null,
            TimeSpan? cacheDuration = null,
            CancellationToken cancellationToken = default)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(RealResiliencePipeline<TResult>));

            if (cacheKey is not null &&
                _cache.TryGetValue(cacheKey, out var entry) &&
                entry.Expiry > DateTime.UtcNow)
            {
                return entry.Value;
            }

            if (_rateLimiter is not null)
                await _rateLimiter.WaitAsync(cancellationToken);

            try
            {
                var result = await _bulkheadPolicy.ExecuteAsync(async () =>
                {
                    return await _circuitBreakerPolicy.ExecuteAsync(async () =>
                    {
                        return await _retryPolicy.ExecuteAsync(operation, cancellationToken);
                    });
                });

                _rateLimiter?.RecordSuccess();

                if (cacheKey is not null)
                {
                    var ttl = cacheDuration ?? TimeSpan.FromSeconds(5);
                    _cache[cacheKey] = new CacheEntry(result, DateTime.UtcNow.Add(ttl));
                }

                return result;
            }
            catch
            {
                _rateLimiter?.RecordFailure();
                throw;
            }
        }

        /// <summary>
        /// Disposes underlying policies and releases resources.
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            _bulkheadPolicy.Dispose();
            _circuitBreakerPolicy.Dispose();
        }
    }
}
