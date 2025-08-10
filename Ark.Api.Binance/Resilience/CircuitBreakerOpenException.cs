using System;

namespace Ark.Api.Binance.Resilience
{
    /// <summary>
    /// Exception raised when an operation is invoked while the circuit breaker is open.
    /// + Provides immediate feedback to the caller about circuit state.
    /// - Does not include the triggering exception details.
    /// Ref: <see href="https://learn.microsoft.com/dotnet/core/resilience/circuit-breaker"/>
    /// </summary>
    public class CircuitBreakerOpenException : Exception
    {
        /// <summary>
        /// Initializes a new instance with a descriptive message.
        /// + Allows custom error descriptions for diagnostics.
        /// - Message should not expose sensitive information.
        /// </summary>
        /// <param name="message">Explanation of the open circuit condition.</param>
        public CircuitBreakerOpenException(string message) : base(message) { }
    }
}
