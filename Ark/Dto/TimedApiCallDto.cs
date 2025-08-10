namespace Ark;

/// <summary>
/// Describes a planned API call with its weight and time offset.
/// + Enables rate limit simulations across timed batches.
/// </summary>
public class TimedApiCallDto
{
    /// <summary>Endpoint name or identifier.</summary>
    public string Endpoint { get; set; } = string.Empty;

    /// <summary>Weight consumed by the call.</summary>
    public int Weight { get; set; }

    /// <summary>True if this call places or cancels orders.</summary>
    public bool IsOrder { get; set; }

    /// <summary>Time offset in seconds relative to batch start.</summary>
    public double TimeSec { get; set; }
}
