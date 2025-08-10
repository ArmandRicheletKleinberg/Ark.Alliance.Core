using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Ark.Api.Binance.Services;
using Ark.Api.Binance.Dto;

namespace Ark.Api.Binance.Controllers;

/// <summary>
/// Provides endpoints for Binance margin requirement projections.
/// + Allows estimating required margin before placing an order.
/// - Does not validate against account state or execute trades.
/// </summary>
/// <remarks>
/// Reference: https://binance-docs.github.io/apidocs/futures/en/#position-margin
/// </remarks>
[ApiController]
[Route("api/binance/[controller]")]
public class MarginController : ControllerBase
{
    private readonly FeeCalculationService _feeService;

    /// <summary>
    /// Creates a new instance of the controller.
    /// </summary>
    /// <param name="feeService">Service used for fee calculations.</param>
    public MarginController(FeeCalculationService feeService)
    {
        _feeService = feeService;
    }

    /// <summary>
    /// Projects the margin requirements for a potential position.
    /// + Helps preview initial and maintenance margin usage.
    /// - Assumes isolated margin; cross‑position exposure is ignored.
    /// </summary>
    /// <param name="request">Projection inputs including symbol, quantity, price and leverage.</param>
    /// <returns>Estimated margin requirements.</returns>
    [HttpPost("project")]
    public async Task<ActionResult<MarginRequirementDto>> Project([FromBody] MarginRequirementRequest request)
    {
        var result = await _feeService.ProjectMarginAsync(request.Symbol, request.Quantity, request.Price, request.Leverage);
        return Ok(result);
    }
}
