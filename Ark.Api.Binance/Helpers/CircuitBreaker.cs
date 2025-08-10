using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

#nullable enable

namespace Ark.Api.Binance.Helpers
{
    /// <summary>
    /// Standalone circuit breaker for protecting critical operations
    /// + Prevents cascading failures
    /// + Configurable failure thresholds and recovery timeouts
    /// + Thread-safe operation
    /// TODO: Add metrics export for monitoring
    /// TODO: Implement adaptive thresholds based on error patterns
    /// </summary>
    public class CircuitBreaker : IDisposable
    {
        #region Fields
        private readonly int _failureThreshold;
        private readonly TimeSpan _recoveryTimeout;
        private readonly ILogger _logger;
        private readonly ReaderWriterLockSlim _lock = new();

        private volatile CircuitState _state = CircuitState.Closed;
        private volatile int _failureCount = 0;
        private long _lastFailureTicks = DateTime.MinValue.Ticks;
        private long _nextAttemptTicks = DateTime.MinValue.Ticks;
        #endregion Fields

        #region Events
        /// <summary>
        /// Fired when the circuit changes state.
        /// + Allows observers to react to open or close transitions.
        /// - Handlers execute on the calling thread.
        /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.eventhandler"/>
        /// </summary>
        public event EventHandler<CircuitStateEventArgs>? StateChanged;
        #endregion Events

        #region Constructors
        /// <summary>
        /// Creates a new circuit breaker instance.
        /// </summary>
        /// <param name="failureThreshold">Number of consecutive failures before opening the circuit.</param>
        /// <param name="recoveryTimeout">Time to wait before allowing a half-open test.</param>
        /// <param name="logger">Optional logger for diagnostics.</param>
        public CircuitBreaker(
            int failureThreshold = 5,
            TimeSpan? recoveryTimeout = null,
            ILogger? logger = null)
        {
            _failureThreshold = failureThreshold;
            _recoveryTimeout = recoveryTimeout ?? TimeSpan.FromMinutes(1);
            _logger = logger ?? NullLogger.Instance;
        }
        #endregion Constructors

        #region Properties
        /// <summary>
        /// Current state of the circuit.
        /// + Determines whether operations are permitted.
        /// - May change between calls without notification.
        /// Ref: <see cref="CircuitState"/>
        /// </summary>
        public CircuitState State => _state;

        /// <summary>
        /// Number of consecutive failures recorded.
        /// + Resets after a successful call.
        /// - High counts will open the circuit.
        /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.threading.interlocked"/>
        /// </summary>
        public int FailureCount => _failureCount;

        /// <summary>
        /// Timestamp of the last recorded failure.
        /// + Useful for monitoring repeated issues.
        /// - Accuracy depends on the system clock.
        /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.datetime"/>
        /// </summary>
        public DateTime LastFailureTime => new DateTime(Volatile.Read(ref _lastFailureTicks), DateTimeKind.Utc);

        /// <summary>
        /// Next permitted UTC time to retry after an open circuit.
        /// + Governs transition from <see cref="CircuitState.Open"/> to <see cref="CircuitState.HalfOpen"/>.
        /// - Requires synchronized clocks across nodes.
        /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.datetime"/>
        /// </summary>
        public DateTime NextAttemptTime => new DateTime(Volatile.Read(ref _nextAttemptTicks), DateTimeKind.Utc);

        /// <summary>
        /// Duration to wait before attempting recovery after the circuit opens.
        /// + Configurable per instance.
        /// - Excessive values delay availability.
        /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.timespan"/>
        /// </summary>
        public TimeSpan RecoveryTimeout => _recoveryTimeout;
        #endregion Properties

        #region Methods
        /// <summary>
        /// Check if the circuit breaker allows execution
        /// </summary>
        public bool CanExecute()
        {
            switch (_state)
            {
                case CircuitState.Closed:
                    return true;

                case CircuitState.Open:
                    if (DateTime.UtcNow >= NextAttemptTime)
                    {
                        TransitionTo(CircuitState.HalfOpen);
                        return true;
                    }
                    return false;

                case CircuitState.HalfOpen:
                    return true;

                default:
                    return false;
            }
        }
        
        /// <summary>
        /// Execute an operation through the circuit breaker
        /// </summary>
        public async Task<T> ExecuteAsync<T>(Func<Task<T>> operation)
        {
            if (!CanExecute())
            {
                var timeUntilRetry = NextAttemptTime - DateTime.UtcNow;
                throw new CircuitBreakerException(
                    $"Circuit breaker is open. Retry in {timeUntilRetry.TotalSeconds:F1} seconds.");
            }
            
            try
            {
                var result = await operation();
                RecordSuccess();
                return result;
            }
            catch (Exception ex)
            {
                RecordFailure(ex);
                throw;
            }
        }
        
        /// <summary>
        /// Execute a synchronous operation through the circuit breaker
        /// </summary>
        public T Execute<T>(Func<T> operation)
        {
            if (!CanExecute())
            {
                var timeUntilRetry = NextAttemptTime - DateTime.UtcNow;
                throw new CircuitBreakerException(
                    $"Circuit breaker is open. Retry in {timeUntilRetry.TotalSeconds:F1} seconds.");
            }
            
            try
            {
                var result = operation();
                RecordSuccess();
                return result;
            }
            catch (Exception ex)
            {
                RecordFailure(ex);
                throw;
            }
        }
        
        /// <summary>
        /// Record a successful operation
        /// </summary>
        public void RecordSuccess()
        {
            _lock.EnterWriteLock();
            try
            {
                var wasHalfOpen = _state == CircuitState.HalfOpen;
                
                Interlocked.Exchange(ref _failureCount, 0);
                
                if (wasHalfOpen)
                {
                    TransitionTo(CircuitState.Closed);
                }
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }
        
        /// <summary>
        /// Record a failed operation
        /// </summary>
        public void RecordFailure(Exception? exception = null)
        {
            _lock.EnterWriteLock();
            try
            {
                var newFailureCount = Interlocked.Increment(ref _failureCount);
                Interlocked.Exchange(ref _lastFailureTicks, DateTime.UtcNow.Ticks);

                _logger.LogWarning("Circuit breaker failure #{Count}: {Error}",
                    newFailureCount, exception?.Message ?? "Unknown error");

                if (newFailureCount >= _failureThreshold || _state == CircuitState.HalfOpen)
                {
                    Interlocked.Exchange(ref _nextAttemptTicks, DateTime.UtcNow.Add(_recoveryTimeout).Ticks);
                    TransitionTo(CircuitState.Open);
                }
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }
        
        /// <summary>
        /// Manually reset the circuit breaker to closed state
        /// </summary>
        public void Reset()
        {
            _lock.EnterWriteLock();
            try
            {
                Interlocked.Exchange(ref _failureCount, 0);
                Interlocked.Exchange(ref _lastFailureTicks, DateTime.MinValue.Ticks);
                Interlocked.Exchange(ref _nextAttemptTicks, DateTime.MinValue.Ticks);
                TransitionTo(CircuitState.Closed);
                
                _logger.LogInformation("Circuit breaker manually reset");
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }
        
        /// <summary>
        /// Get current circuit breaker statistics
        /// </summary>
        public CircuitBreakerStats GetStats()
        {
            _lock.EnterReadLock();
            try
            {
                return new CircuitBreakerStats
                {
                    State = _state,
                    FailureCount = _failureCount,
                    FailureThreshold = _failureThreshold,
                    LastFailureTime = new DateTime(Volatile.Read(ref _lastFailureTicks), DateTimeKind.Utc),
                    NextAttemptTime = new DateTime(Volatile.Read(ref _nextAttemptTicks), DateTimeKind.Utc),
                    RecoveryTimeout = _recoveryTimeout
                };
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }
        
        private void TransitionTo(CircuitState newState)
        {
            var oldState = _state;
            _state = newState;
            
            if (oldState != newState)
            {
                _logger.LogInformation("🔄 Circuit breaker state: {OldState} → {NewState}",
                    oldState, newState);
                
                StateChanged?.Invoke(this, new CircuitStateEventArgs(oldState, newState));
            }
        }
        
        /// <summary>
        /// Releases managed resources.
        /// </summary>
        public void Dispose()
        {
            _lock?.Dispose();
        }
        #endregion Methods
    }
    
    #region Supporting Types
    /// <summary>
    /// Possible states for the <see cref="CircuitBreaker"/>.
    /// + Indicates whether execution is currently allowed.
    /// - Does not track failure counts or timings.
    /// Ref: <see href="https://learn.microsoft.com/dotnet/architecture/"/>
    /// </summary>
    public enum CircuitState
    {
        /// <summary>Normal operation – requests allowed</summary>
        Closed,

        /// <summary>Failure threshold exceeded – requests blocked</summary>
        Open,

        /// <summary>Testing recovery – limited requests allowed</summary>
        HalfOpen
    }

    /// <summary>
    /// Exception thrown when the circuit breaker denies execution.
    /// + Contains a descriptive failure message.
    /// - Does not suggest recovery actions.
    /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.exception"/>
    /// </summary>
    public class CircuitBreakerException : Exception
    {
        /// <summary>
        /// Create a new instance with a message.
        /// </summary>
        public CircuitBreakerException(string message) : base(message) { }

        /// <summary>
        /// Create a new instance with a message and inner exception.
        /// </summary>
        public CircuitBreakerException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// Event arguments describing a circuit breaker state change.
    /// + Exposes previous and new states for observers.
    /// - Timestamp uses UTC and depends on system clock.
    /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.eventargs"/>
    /// </summary>
    public class CircuitStateEventArgs : EventArgs
    {
        /// <summary>
        /// State before the transition.
        /// + Useful for auditing changes.
        /// - Irrelevant if state did not change.
        /// </summary>
        public CircuitState OldState { get; }

        /// <summary>
        /// State after the transition.
        /// + Indicates current circuit permission.
        /// - Might change again immediately after.
        /// </summary>
        public CircuitState NewState { get; }

        /// <summary>
        /// UTC timestamp of the transition.
        /// + Helps correlate with logs.
        /// - Accuracy depends on system clock.
        /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.datetime"/>
        /// </summary>
        public DateTime Timestamp { get; }

        /// <summary>
        /// Initializes event arguments with previous and new circuit states.
        /// </summary>
        /// <param name="oldState">State before transition.</param>
        /// <param name="newState">State after transition.</param>
        public CircuitStateEventArgs(CircuitState oldState, CircuitState newState)
        {
            OldState = oldState;
            NewState = newState;
            Timestamp = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Snapshot of circuit breaker metrics for monitoring.
    /// + Provides insight into failure counts and timings.
    /// - Values may be stale if queried infrequently.
    /// </summary>
    public class CircuitBreakerStats
    {
        /// <summary>
        /// Current circuit state.
        /// + Indicates whether execution is permitted.
        /// - Does not show history of previous states.
        /// </summary>
        public CircuitState State { get; set; }

        /// <summary>
        /// Number of recent consecutive failures.
        /// + Helps evaluate stability.
        /// - Reset after success or manual reset.
        /// </summary>
        public int FailureCount { get; set; }

        /// <summary>
        /// Configured failure threshold before opening.
        /// + Guides tuning of the breaker.
        /// - Static during runtime.
        /// </summary>
        public int FailureThreshold { get; set; }

        /// <summary>
        /// UTC time of the most recent failure.
        /// + Enables monitoring of repeated issues.
        /// - May be DateTime.MinValue if none occurred.
        /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.datetime"/>
        /// </summary>
        public DateTime LastFailureTime { get; set; }

        /// <summary>
        /// Next UTC time when execution may be attempted again.
        /// + Controls transition from open to half-open.
        /// - Requires synchronised clocks across nodes.
        /// </summary>
        public DateTime NextAttemptTime { get; set; }

        /// <summary>
        /// Configured recovery timeout duration.
        /// + Determines delay before retry.
        /// - Excessive values can reduce availability.
        /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.timespan"/>
        /// </summary>
        public TimeSpan RecoveryTimeout { get; set; }

        /// <summary>
        /// Time remaining before a retry is permitted.
        /// + Zero when the circuit can execute immediately.
        /// - Calculated at access time; may drift.
        /// </summary>
        public TimeSpan TimeUntilRetry => NextAttemptTime > DateTime.UtcNow
            ? NextAttemptTime - DateTime.UtcNow
            : TimeSpan.Zero;
    }
    #endregion Supporting Types
}