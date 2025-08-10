using System.Collections.Generic;


namespace Ark.Api.Binance;

/// <summary>
/// Request payload for simulating Binance rate limit consumption.
/// + Provides timed API calls and order batch parameters.
/// - Assumes limits are constant and does not verify authentication.
/// </summary>
public class RateLimitSimulationRequestDto
{
    /// <summary>Timed API calls to evaluate.</summary>
    public List<TimedApiCallDto> TimedCalls { get; set; } = new();

    /// <summary>Number of orders to place in the batch.</summary>
    public int OrderCount { get; set; }

    /// <summary>Total duration in seconds for sending the orders.</summary>
    public double BatchDurationSec { get; set; }
}
