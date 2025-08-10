using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Ark.Api.Binance.Services;


namespace Ark.Api.Binance.Controllers;

/// <summary>
/// Exposes endpoints to manage rate limit rules.
/// + Allows runtime tuning of request weight and order limits.
/// - Changes are not persisted beyond process restart.
/// Ref: <see href="https://binance-docs.github.io/apidocs/futures/en/#limit-information"/>
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class RateLimitRulesController : ControllerBase
{
    private readonly RateLimitRulesService _service;

    /// <summary>
    /// Creates the controller.
    /// </summary>
    /// <param name="service">Rate limit rule service.</param>
    public RateLimitRulesController(RateLimitRulesService service)
    {
        _service = service;
    }

    /// <summary>
    /// Gets current rate limit rules.
    /// + Useful for monitoring consumption thresholds.
    /// - Provides raw values without usage statistics.
    /// </summary>
    /// <returns>Current configuration.</returns>
    [HttpGet]
    public async Task<IActionResult> GetRateLimitRules()
    {
        var rules = await _service.GetRulesAsync();
        return Ok(rules);
    }

    /// <summary>
    /// Updates rate limit rules.
    /// + Immediately applies new weight and order limits.
    /// - No validation beyond basic range checks in <see cref="RateLimitRulesService"/>.
    /// </summary>
    /// <param name="request">New rule values.</param>
    /// <returns>Confirmation of update.</returns>
    [HttpPost]
    public async Task<IActionResult> UpdateRateLimitRules([FromBody] UpdateRateLimitsRequest request)
    {
        await _service.UpdateRulesAsync(request);
        return Ok(new { Message = "Limites de taux mises à jour" });
    }
}
