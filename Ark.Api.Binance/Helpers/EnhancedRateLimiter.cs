using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

#nullable enable

namespace Ark.Api.Binance.Helpers
{
    /// <summary>
    /// Enhanced rate limiter with exponential backoff and jitter.
    /// + Implements sophisticated throttling strategies
    /// + Reduces thundering herd problems with jitter
    /// - More complex than simple rate limiting
    /// TODO: Add adaptive rate limiting based on server responses
    /// </summary>
    public class EnhancedRateLimiter
    {
        private readonly int _limit;
        private readonly TimeSpan _interval;
        private readonly double _alertThreshold;
        private readonly double _recoveryThreshold;
        private readonly ConcurrentQueue<DateTime> _timestamps = new();
        private readonly ILogger _logger;
        private readonly Random _random = new();
        private readonly SemaphoreSlim _semaphore;

        /// <summary>
        /// Gets the fraction of the limit at which warnings are emitted.
        /// </summary>
        public double AlertThreshold => _alertThreshold;

        /// <summary>
        /// Gets the fraction of the limit below which normal operation resumes.
        /// </summary>
        public double RecoveryThreshold => _recoveryThreshold;
        
        // Exponential backoff parameters
        private readonly TimeSpan _baseDelay = TimeSpan.FromMilliseconds(100);
        private readonly TimeSpan _maxDelay = TimeSpan.FromSeconds(30);
        private int _consecutiveFailures = 0;
        private DateTime _lastFailure = DateTime.MinValue;
        
        /// <summary>
        /// Initializes a new rate limiter with the provided configuration.
        /// </summary>
        /// <param name="info">Limit settings including count and thresholds.</param>
        /// <param name="logger">Optional logger for diagnostics.</param>
        public EnhancedRateLimiter(LimitInfo info, ILogger? logger = null)
        {
            _limit = info.Limit;
            _alertThreshold = info.AlertThreshold;
            _recoveryThreshold = info.RecoveryThreshold;
            _interval = TimeSpan.TryParse(info.Interval, out var ts) ? ts : TimeSpan.FromMinutes(1);
            _logger = logger ?? NullLogger.Instance;
            _semaphore = new SemaphoreSlim(_limit, _limit);
        }
        
        /// <summary>
        /// Wait with exponential backoff and jitter
        /// </summary>
        public async Task WaitAsync(CancellationToken token = default)
        {
            await _semaphore.WaitAsync(token);
            
            try
            {
                while (true)
                {
                    lock (_timestamps)
                    {
                        CleanOldTimestamps();
                        
                        if (_timestamps.Count < _limit)
                        {
                            _timestamps.Enqueue(DateTime.UtcNow);
                            
                            // Alert when approaching limit
                            if (_timestamps.Count > _limit * _alertThreshold)
                            {
                                _logger.LogWarning("Rate limit approaching: {Count}/{Limit} ({Percentage:P0})", 
                                    _timestamps.Count, _limit, (double)_timestamps.Count / _limit);
                            }
                            
                            return;
                        }
                    }
                    
                    // Calculate backoff delay
                    var baseWait = CalculateBaseWait();
                    var backoffMultiplier = CalculateBackoffMultiplier();
                    var jitter = CalculateJitter();
                    
                    var totalDelay = TimeSpan.FromMilliseconds(
                        Math.Min(_maxDelay.TotalMilliseconds, 
                                baseWait.TotalMilliseconds * backoffMultiplier + jitter));
                    
                    _logger.LogDebug("Rate limited, waiting {Delay}ms", totalDelay.TotalMilliseconds);
                    
                    await Task.Delay(totalDelay, token);
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }
        
        /// <summary>
        /// Record a successful request
        /// </summary>
        public void RecordSuccess()
        {
            Interlocked.Exchange(ref _consecutiveFailures, 0);
        }
        
        /// <summary>
        /// Record a failed request for backoff calculation
        /// </summary>
        public void RecordFailure()
        {
            Interlocked.Increment(ref _consecutiveFailures);
            _lastFailure = DateTime.UtcNow;
            _logger.LogWarning("Rate limit failure recorded. Consecutive failures: {Count}", _consecutiveFailures);
        }
        
        /// <summary>
        /// Gets the current usage ratio relative to the limit.
        /// </summary>
        public double Usage
        {
            get
            {
                lock (_timestamps)
                {
                    CleanOldTimestamps();
                    return (double)_timestamps.Count / _limit;
                }
            }
        }
        
        private void CleanOldTimestamps()
        {
            var cutoff = DateTime.UtcNow - _interval;
            while (_timestamps.TryPeek(out var timestamp) && timestamp < cutoff)
            {
                _timestamps.TryDequeue(out _);
            }
        }
        
        private TimeSpan CalculateBaseWait()
        {
            lock (_timestamps)
            {
                if (_timestamps.TryPeek(out var oldest))
                {
                    var waitTime = _interval - (DateTime.UtcNow - oldest);
                    return waitTime > TimeSpan.Zero ? waitTime : TimeSpan.Zero;
                }
            }
            return TimeSpan.Zero;
        }
        
        private double CalculateBackoffMultiplier()
        {
            var failures = _consecutiveFailures;
            if (failures == 0) return 1.0;
            
            // Exponential backoff: 2^failures, capped at reasonable maximum
            return Math.Min(Math.Pow(2, Math.Min(failures, 10)), 64.0);
        }
        
        private double CalculateJitter()
        {
            // Add up to 50% jitter to prevent thundering herd
            return _random.NextDouble() * _baseDelay.TotalMilliseconds * 0.5;
        }

        /// <summary>
        /// Indicates whether the current usage exceeds the alert threshold.
        /// </summary>
        public bool IsApproachingLimit()
        {
            lock (_timestamps)
            {
                CleanOldTimestamps();
                return _timestamps.Count > _limit * _alertThreshold;
            }
        }

        /// <summary>
        /// Determines if usage has fallen below the recovery threshold.
        /// </summary>
        public bool IsBelowRecoveryThreshold()
        {
            lock (_timestamps)
            {
                CleanOldTimestamps();
                return _timestamps.Count < _limit * _recoveryThreshold;
            }
        }

        /// <summary>
        /// Releases underlying resources.
        /// </summary>
        public void Dispose()
        {
            _semaphore?.Dispose();
        }
    }
}