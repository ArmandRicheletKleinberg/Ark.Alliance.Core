using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Ark.Api.Binance.Resilience
{
    /// <summary>
    /// Applies the bulkhead resilience pattern to isolate a resource pool.
    /// + Limits concurrent operations to avoid resource exhaustion.
    /// - Requests beyond the limit wait for an available slot.
    /// Ref: <see href="https://learn.microsoft.com/dotnet/core/resilience/bulkhead"/>
    /// </summary>
    public class BulkheadPolicy : IDisposable
    {
        #region Fields

        private readonly SemaphoreSlim _semaphore;
        private readonly int _maxConcurrency;
        private readonly ILogger _logger;
        private volatile bool _disposed = false;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes the policy with a maximum number of concurrent operations.
        /// + Uses <see cref="SemaphoreSlim"/> to guard the critical section.
        /// - A high value may still cause contention under heavy load.
        /// </summary>
        /// <param name="maxConcurrency">Maximum number of simultaneous operations.</param>
        /// <param name="logger">Optional <see cref="ILogger"/> for diagnostics.</param>
        public BulkheadPolicy(int maxConcurrency = 10, ILogger? logger = null)
        {
            _maxConcurrency = maxConcurrency;
            _semaphore = new SemaphoreSlim(maxConcurrency, maxConcurrency);
            _logger = logger ?? NullLogger.Instance;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Executes an asynchronous operation within the bulkhead, waiting until capacity is available.
        /// + Prevents resource saturation by queuing requests.
        /// - Adds latency when all slots are occupied.
        /// </summary>
        /// <typeparam name="TResult">Type returned by the operation.</typeparam>
        /// <param name="operation">Operation to execute when a slot is acquired.</param>
        /// <returns>The result produced by <paramref name="operation"/>.</returns>
        public async Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> operation)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(BulkheadPolicy));

            var stopwatch = Stopwatch.StartNew();

            await _semaphore.WaitAsync();

            var waitTime = stopwatch.Elapsed;
            if (waitTime > TimeSpan.FromSeconds(1))
            {
                _logger.LogWarning("⏳ Bulkhead wait time: {WaitTime}ms", waitTime.TotalMilliseconds);
            }

            try
            {
                return await operation();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        #endregion Methods

        #region Properties

        /// <summary>
        /// Remaining slots that can be acquired without blocking.
        /// + Useful for monitoring throughput.
        /// - May be stale immediately after retrieval under contention.
        /// </summary>
        public int AvailableCount => _semaphore.CurrentCount;

        /// <summary>
        /// Maximum concurrent operations allowed by this bulkhead.
        /// + Controls parallelism for protected resources.
        /// - Must be tuned to match downstream capacity.
        /// </summary>
        public int MaxConcurrency => _maxConcurrency;

        #endregion Properties

        #region IDisposable

        /// <summary>
        /// Releases resources used by the policy.
        /// + No unmanaged handles are held.
        /// - Future implementations may introduce disposable members.
        /// Ref: <see href="https://learn.microsoft.com/dotnet/standard/garbage-collection/implementing-dispose"/>
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            _semaphore.Dispose();
        }

        #endregion IDisposable
    }
}
