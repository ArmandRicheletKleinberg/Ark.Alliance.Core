using Microsoft.AspNetCore.Mvc;
using Ark.Api.Binance.Services;


namespace Ark.Api.Binance.Controllers;

/// <summary>
/// Exposes rate limit simulation utilities.
/// + Predicts consumption before issuing real requests.
/// - Works from provided assumptions; live limits may differ.
/// </summary>
[ApiController]
[Route("api/ratelimit/simulate")]
public class RateLimitSimulationController : ControllerBase
{
    private readonly RateLimitSimulationService _service;

    /// <summary>
    /// Initializes the controller.
    /// </summary>
    /// <param name="service">Simulation service instance.</param>
    public RateLimitSimulationController(RateLimitSimulationService service)
    {
        _service = service;
    }

    /// <summary>
    /// Simulates weight and order rate usage for planned calls.
    /// + Helps plan batch operations without hitting limits.
    /// - Does not execute any remote requests or validate authentication.
    /// </summary>
    [HttpPost]
    public IActionResult Simulate([FromBody] RateLimitSimulationRequestDto request)
    {
        var result = _service.Simulate(request);
        return Ok(result);
    }
}
