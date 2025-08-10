using System.Linq;

namespace Ark.Api.Binance.Services;

/// <summary>
/// Provides rate limit simulation using <see cref="RateLimitAnalyzer"/>.
/// + Evaluates planned calls to predict weight and order rate usage.
/// - Uses static assumptions; live limits may differ.
/// </summary>
public class RateLimitSimulationService
{
    private readonly RateLimitAnalyzer _analyzer;

    /// <summary>
    /// Initializes a new instance of the <see cref="RateLimitSimulationService"/> class.
    /// </summary>
    /// <param name="analyzer">Analyzer used to evaluate rate limit usage.</param>
    public RateLimitSimulationService(RateLimitAnalyzer analyzer)
    {
        _analyzer = analyzer;
    }

    /// <summary>
    /// Simulates weight usage and order rates for a batch of API calls.
    /// + Aggregates call weights and detects burst violations.
    /// - Ignores network failures or API key restrictions.
    /// </summary>
    public RateLimitSimulationResponseDto Simulate(RateLimitSimulationRequestDto request)
    {
        var calls = request.TimedCalls
            .Select(tc => new RateLimitAnalyzer.ApiCall
            {
                Endpoint = tc.Endpoint,
                Weight = tc.Weight,
                IsOrder = tc.IsOrder
            }).ToList();

        var (totalWeight, exceeds) = _analyzer.CalculateWeightUsage(calls);

        var timed = request.TimedCalls
            .Select(tc => (tc.TimeSec, new RateLimitAnalyzer.ApiCall
            {
                Endpoint = tc.Endpoint,
                Weight = tc.Weight,
                IsOrder = tc.IsOrder
            }));

        bool burst = _analyzer.DetectWeightBurstViolation(timed);
        string orderRate = _analyzer.CheckOrderRate(request.OrderCount, request.BatchDurationSec);
        double delay = _analyzer.SuggestOrderStaggerDelay(request.OrderCount);

        return new RateLimitSimulationResponseDto
        {
            TotalWeight = totalWeight,
            WeightLimitExceeded = exceeds,
            BurstViolation = burst,
            OrderRateMessage = orderRate,
            SuggestedDelayMs = delay
        };
    }
}
