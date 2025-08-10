using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Ark.Api.Binance.Resilience
{
    /// <summary>
    /// Implements a circuit breaker to prevent cascading failures.
    /// + Opens after consecutive failures and recovers after a timeout.
    /// + Emits state change events for diagnostics.
    /// - May block requests even after external recovery until timeout elapses.
    /// Ref: <see href="https://learn.microsoft.com/dotnet/core/resilience/circuit-breaker"/>
    /// </summary>
    public class CircuitBreakerPolicy : IDisposable
    {
        #region Fields

        private readonly int _failureThreshold;
        private readonly TimeSpan _recoveryTimeout;
        private readonly ILogger _logger;
        private readonly object _lock = new();

        private CircuitState _state = CircuitState.Closed;
        private int _failureCount = 0;
        private DateTime _lastFailureTime = DateTime.MinValue;
        private DateTime _nextAttemptTime = DateTime.MinValue;

        #endregion Fields

        #region Events

        /// <summary>
        /// Occurs when the circuit changes state.
        /// + Allows subscribers to observe resilience behavior.
        /// - Handlers execute synchronously and should be lightweight.
        /// </summary>
        public event EventHandler<CircuitStateChangedEventArgs>? StateChanged;

        #endregion Events

        #region Constructors

        /// <summary>
        /// Creates a new circuit breaker policy.
        /// + Customizable failure threshold and recovery timeout.
        /// - Initial failures before tracking starts are not counted.
        /// </summary>
        /// <param name="failureThreshold">Number of failures before opening the circuit.</param>
        /// <param name="recoveryTimeout">Time to wait before allowing a test request.</param>
        /// <param name="logger">Optional <see cref="ILogger"/> for diagnostic messages.</param>
        public CircuitBreakerPolicy(
            int failureThreshold = 5,
            TimeSpan? recoveryTimeout = null,
            ILogger? logger = null)
        {
            _failureThreshold = failureThreshold;
            _recoveryTimeout = recoveryTimeout ?? TimeSpan.FromSeconds(30);
            _logger = logger ?? NullLogger.Instance;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Current state of the circuit breaker.
        /// + Enables callers to query the circuit status.
        /// - Exposes internal state that may change concurrently.
        /// </summary>
        public CircuitState State
        {
            get
            {
                lock (_lock)
                {
                    return _state;
                }
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Executes an operation, preventing calls when the circuit is open.
        /// + Protects downstream services from repeated failures.
        /// - Introduces locking overhead for each call.
        /// </summary>
        /// <typeparam name="TResult">Type returned by the operation.</typeparam>
        /// <param name="operation">Operation to invoke when permitted.</param>
        /// <returns>Result of the executed <paramref name="operation"/>.</returns>
        /// <exception cref="CircuitBreakerOpenException">Thrown when the circuit is open and not ready for retry.</exception>
        public async Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> operation)
        {
            lock (_lock)
            {
                switch (_state)
                {
                    case CircuitState.Open:
                        if (DateTime.UtcNow < _nextAttemptTime)
                        {
                            throw new CircuitBreakerOpenException($"Circuit breaker is open. Next attempt allowed at {_nextAttemptTime}");
                        }

                        // Move to half-open state
                        ChangeState(CircuitState.HalfOpen);
                        break;

                    case CircuitState.HalfOpen:
                        // Allow one test request
                        break;

                    case CircuitState.Closed:
                        // Normal operation
                        break;
                }
            }

            try
            {
                var result = await operation();
                OnSuccess();
                return result;
            }
            catch (Exception ex)
            {
                OnFailure(ex);
                throw;
            }
        }

        private void OnSuccess()
        {
            lock (_lock)
            {
                _failureCount = 0;

                if (_state == CircuitState.HalfOpen)
                {
                    ChangeState(CircuitState.Closed);
                }
            }
        }

        private void OnFailure(Exception exception)
        {
            lock (_lock)
            {
                _failureCount++;
                _lastFailureTime = DateTime.UtcNow;

                _logger.LogWarning("Circuit breaker recorded failure #{Count}: {Error}", _failureCount, exception.Message);

                if (_failureCount >= _failureThreshold || _state == CircuitState.HalfOpen)
                {
                    _nextAttemptTime = DateTime.UtcNow.Add(_recoveryTimeout);
                    ChangeState(CircuitState.Open);
                }
            }
        }

        private void ChangeState(CircuitState newState)
        {
            var oldState = _state;
            _state = newState;

            _logger.LogInformation("🔄 Circuit breaker state changed: {OldState} -> {NewState}", oldState, newState);

            StateChanged?.Invoke(this, new CircuitStateChangedEventArgs(oldState, newState));
        }

        #endregion Methods

        #region IDisposable

        /// <summary>
        /// Releases resources used by the policy.
        /// + No unmanaged handles are held.
        /// - Future implementations may introduce disposable members.
        /// Ref: <see href="https://learn.microsoft.com/dotnet/standard/garbage-collection/implementing-dispose"/>
        /// </summary>
        public void Dispose()
        {
            // Nothing to dispose currently
        }

        #endregion IDisposable
    }
}
