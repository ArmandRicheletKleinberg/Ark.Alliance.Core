using System;
using System.Collections.Generic;

namespace Ark.Api.Binance.Services;

/// <summary>
/// Provides utility methods to estimate API weight and order rate usage.
/// + Helps plan request batches without violating Binance limits.
/// - Does not query remote limits automatically.
/// </summary>
public class RateLimitAnalyzer
{
    /// <summary>
    /// Default weight limit per minute.
    /// + Based on Binance REST API rules.
    /// </summary>
    public int WeightLimitPerMin { get; set; } = 2400;

    /// <summary>
    /// Default order limit per minute.
    /// + All order endpoints share the same cap.
    /// </summary>
    public int OrderLimitPerMin { get; set; } = 1200;

    /// <summary>
    /// Default order burst limit for 10 seconds.
    /// + Prevents short spikes that would trigger bans.
    /// </summary>
    public int OrderLimitPer10s { get; set; } = 300;

    /// <summary>
    /// Describes an API call with its weight and whether it counts as an order.
    /// </summary>
    public class ApiCall
    {
        /// <summary>Endpoint name.</summary>
        public string Endpoint { get; init; } = string.Empty;

        /// <summary>Weight used for the call.</summary>
        public int Weight { get; init; } = 1;

        /// <summary>True if call places or cancels orders.</summary>
        public bool IsOrder { get; init; }
    }

    /// <summary>
    /// Calculates total request weight for a batch and checks against the minute limit.
    /// </summary>
    /// <param name="calls">Batch of API calls.</param>
    /// <returns>Tuple of total weight and whether the limit is exceeded.</returns>
    public (int totalWeight, bool exceedsLimit) CalculateWeightUsage(IEnumerable<ApiCall> calls)
    {
        int total = 0;
        foreach (var call in calls)
        {
            total += call.Weight;
        }
        return (total, total > WeightLimitPerMin);
    }

    /// <summary>
    /// Detects if any rolling one minute window exceeds the weight limit.
    /// + Assumes calls are ordered by time in seconds.
    /// </summary>
    public bool DetectWeightBurstViolation(IEnumerable<(double timeSec, ApiCall call)> timedCalls)
    {
        var list = new List<(double t, ApiCall c)>(timedCalls);
        for (int i = 0; i < list.Count; i++)
        {
            double start = list[i].t;
            double end = start + 60.0;
            int sum = 0;
            for (int j = i; j < list.Count && list[j].t < end; j++)
            {
                sum += list[j].c.Weight;
                if (sum > WeightLimitPerMin)
                    return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Checks if a batch of order placements violates order rate limits.
    /// </summary>
    /// <param name="orderCount">Number of orders to place.</param>
    /// <param name="batchDurationSec">Duration for sending the orders.</param>
    /// <returns>Message indicating whether limits are safe or violated.</returns>
    public string CheckOrderRate(int orderCount, double batchDurationSec)
    {
        double perMin = orderCount / (batchDurationSec / 60.0);
        if (perMin > OrderLimitPerMin)
            return $"Order rate exceeds {OrderLimitPerMin}/min limit (rate ~{perMin:F1} orders/min)";

        double per10s = orderCount / (batchDurationSec / 10.0);
        if (per10s > OrderLimitPer10s)
            return $"Order burst exceeds {OrderLimitPer10s}/10s limit (rate ~{per10s:F1} orders/10s)";

        return "Order rate within limits.";
    }

    /// <summary>
    /// Suggests a minimal delay between orders to respect the 300/10s burst cap.
    /// </summary>
    /// <param name="ordersToPlace">Total number of orders.</param>
    /// <returns>Recommended delay in milliseconds.</returns>
    public double SuggestOrderStaggerDelay(int ordersToPlace)
    {
        if (ordersToPlace <= OrderLimitPer10s)
            return 0;

        double batches = Math.Ceiling(ordersToPlace / (double)OrderLimitPer10s);
        double requiredTime = batches * 10.0;
        return (requiredTime / ordersToPlace) * 1000.0;
    }
}
