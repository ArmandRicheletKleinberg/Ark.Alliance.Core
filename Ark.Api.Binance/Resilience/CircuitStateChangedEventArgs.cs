using System;

namespace Ark.Api.Binance.Resilience
{
    /// <summary>
    /// State transition event arguments for <see cref="CircuitBreakerPolicy"/>
    /// </summary>
    public class CircuitStateChangedEventArgs : EventArgs
    {
        /// <summary>
        /// State before the transition.
        /// </summary>
        public CircuitState OldState { get; }

        /// <summary>
        /// State after the transition.
        /// </summary>
        public CircuitState NewState { get; }

        /// <summary>
        /// Initializes a new instance of the event args with the old and new circuit states.
        /// </summary>
        /// <param name="oldState">State prior to the change.</param>
        /// <param name="newState">State after the change.</param>
        public CircuitStateChangedEventArgs(CircuitState oldState, CircuitState newState)
        {
            OldState = oldState;
            NewState = newState;
        }
    }
}
