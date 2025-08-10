namespace Ark.Api.Binance.Resilience
{
    /// <summary>
    /// Circuit breaker states
    /// + Closed – normal operation
    /// + Open – failing and blocking requests
    /// + HalfOpen – testing for recovery
    /// </summary>
    public enum CircuitState
    {
        /// <summary>Closed – normal operation</summary>
        Closed,

        /// <summary>Open – failing and blocking requests</summary>
        Open,

        /// <summary>HalfOpen – testing for recovery</summary>
        HalfOpen
    }
}
