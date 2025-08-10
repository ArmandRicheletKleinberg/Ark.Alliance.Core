using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Ark.Api.Binance.Services;
using Ark.Api.Binance;

namespace Ark.Api.Binance.Controllers;

/// <summary>
/// Provides fee rule endpoints for Binance symbols.
/// + Exposes helper operations to inspect and simulate fees before trading.
/// - Does not persist user-specific overrides; configuration remains in memory.
/// Ref: <see href="https://binance-docs.github.io/apidocs/futures/en/#commission-rate-data"/>
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class FeeRulesController : ControllerBase
{
    private readonly FeeRulesService _feeRulesService;
    private readonly FeeCalculationService _feeCalculationService;

    /// <summary>
    /// Initializes a new instance of the controller.
    /// </summary>
    /// <param name="feeRulesService">Service accessing fee rules.</param>
    /// <param name="feeCalculationService">Service calculating fees.</param>
    public FeeRulesController(
        FeeRulesService feeRulesService,
        FeeCalculationService feeCalculationService)
    {
        _feeRulesService = feeRulesService;
        _feeCalculationService = feeCalculationService;
    }

    /// <summary>
    /// Gets fee rules for a symbol.
    /// + Returns maker, taker and funding rates.
    /// - Values may be outdated if Binance updates their schedule.
    /// </summary>
    /// <param name="symbol">Trading symbol such as "BTCUSDT".</param>
    /// <returns>Fee rule details in JSON format.</returns>
    [HttpGet("{symbol}")]
    public async Task<IActionResult> GetFeeRules(string symbol)
    {
        var entity = await _feeRulesService.GetFeeRulesAsync(symbol);
        var dto = new FeeRulesDto
        {
            MakerFee = entity.MakerFeeVip0,
            TakerFee = entity.TakerFeeVip0,
            FundingRate = entity.CurrentFundingRate
        };

        return Ok(dto);
    }

    /// <summary>
    /// Calculates optimal click amount for a symbol.
    /// + Evaluates trade size needed to reach a target profit percentage.
    /// - Ignores slippage and funding rate variance.
    /// </summary>
    /// <param name="symbol">Trading symbol.</param>
    /// <param name="request">Optimization parameters.</param>
    /// <returns>Optimal click result.</returns>
    [HttpPost("{symbol}/optimize-click")]
    public async Task<IActionResult> OptimizeClick(
        string symbol,
        [FromBody] OptimizeClickRequest request)
    {
        var result = await _feeCalculationService.CalculateOptimalClickAmount(
            symbol,
            request.CurrentPrice,
            request.TargetProfitPct,
            request.Leverage,
            request.IsLong,
            request.VipLevel,
            request.UseBnbDiscount);

        return Ok(result);
    }

    /// <summary>
    /// Validates profitability for trade parameters.
    /// + Confirms that projected profit covers exchange fees.
    /// - Assumes fee rules remain constant during trade lifetime.
    /// </summary>
    /// <param name="symbol">Trading symbol.</param>
    /// <param name="request">Profitability validation request.</param>
    /// <returns>Whether the trade is profitable.</returns>
    [HttpPost("{symbol}/validate-profitability")]
    public async Task<IActionResult> ValidateProfitability(
        string symbol,
        [FromBody] ValidateProfitabilityRequest request)
    {
        var isProfitable = await _feeCalculationService.ValidateProfitabilityAsync(
            symbol,
            request.Quantity,
            request.EntryPrice,
            request.ExitPrice,
            request.EntryIsMaker,
            request.ExitIsMaker,
            request.VipLevel);

        return Ok(new { IsProfitable = isProfitable });
    }
}
