using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

#nullable enable

namespace Ark.Api.Binance.Resilience
{
    /// <summary>
    /// Advanced retry policy with exponential backoff and jitter
    /// + Handles transient errors transparently
    /// + Use for operations requiring resilience
    /// - Avoid for non-idempotent operations that must not repeat
    /// </summary>
    public class RetryPolicy
    {
        private readonly int _maxRetries;
        private readonly TimeSpan _baseDelay;
        private readonly TimeSpan _maxDelay;
        private readonly double _backoffMultiplier;
        private readonly Random _random = new();
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the retry policy.
        /// </summary>
        /// <param name="maxRetries">Maximum number of retry attempts.</param>
        /// <param name="baseDelay">Initial delay before retrying.</param>
        /// <param name="maxDelay">Maximum delay between retries.</param>
        /// <param name="backoffMultiplier">Multiplier applied for exponential backoff.</param>
        /// <param name="logger">Optional logger for diagnostics.</param>
        public RetryPolicy(
            int maxRetries = 3,
            TimeSpan? baseDelay = null,
            TimeSpan? maxDelay = null,
            double backoffMultiplier = 2.0,
            ILogger? logger = null)
        {
            _maxRetries = maxRetries;
            _baseDelay = baseDelay ?? TimeSpan.FromMilliseconds(100);
            _maxDelay = maxDelay ?? TimeSpan.FromSeconds(30);
            _backoffMultiplier = backoffMultiplier;
            _logger = logger ?? NullLogger.Instance;
        }

        /// <summary>
        /// Executes the specified operation applying the retry policy.
        /// </summary>
        /// <typeparam name="TResult">Type of the operation result.</typeparam>
        /// <param name="operation">Delegate representing the operation.</param>
        /// <param name="cancellationToken">Token to cancel the execution.</param>
        /// <returns>The result of the operation.</returns>
        public async Task<TResult> ExecuteAsync<TResult>(
            Func<CancellationToken, Task<TResult>> operation,
            CancellationToken cancellationToken = default)
        {
            Exception? lastException = null;

            for (int attempt = 0; attempt <= _maxRetries; attempt++)
            {
                try
                {
                    var result = await operation(cancellationToken);

                    if (attempt > 0)
                    {
                        _logger.LogInformation("✅ Operation succeeded after {Attempts} attempts", attempt + 1);
                    }

                    return result;
                }
                catch (Exception ex) when (ShouldRetry(ex, attempt))
                {
                    lastException = ex;

                    if (attempt < _maxRetries)
                    {
                        var delay = CalculateDelay(attempt);
                        _logger.LogWarning("⚠️ Operation failed (attempt {Attempt}/{MaxAttempts}), retrying in {Delay}ms: {Error}",
                            attempt + 1, _maxRetries + 1, delay.TotalMilliseconds, ex.Message);

                        await Task.Delay(delay, cancellationToken);
                    }
                }
            }

            _logger.LogError("❌ Operation failed after {MaxAttempts} attempts", _maxRetries + 1);
            throw lastException ?? new InvalidOperationException("Retry policy failed without capturing exception");
        }

        private bool ShouldRetry(Exception exception, int attempt)
        {
            if (attempt >= _maxRetries) return false;

            // Don't retry certain types of exceptions
            return exception is not (
                ArgumentException or
                ArgumentNullException or
                InvalidOperationException or
                UnauthorizedAccessException
            );
        }

        private TimeSpan CalculateDelay(int attempt)
        {
            // Exponential backoff with jitter
            var exponentialDelay = TimeSpan.FromMilliseconds(
                _baseDelay.TotalMilliseconds * Math.Pow(_backoffMultiplier, attempt));

            // Add jitter (±25%)
            var jitterMultiplier = 0.75 + (_random.NextDouble() * 0.5);
            var delayWithJitter = TimeSpan.FromMilliseconds(exponentialDelay.TotalMilliseconds * jitterMultiplier);

            // Ensure we don't exceed max delay
            return delayWithJitter > _maxDelay ? _maxDelay : delayWithJitter;
        }
    }
}
