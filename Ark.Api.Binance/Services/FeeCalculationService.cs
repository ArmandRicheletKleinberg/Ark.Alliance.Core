using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Ark.Api.Binance.Models;
using Ark.Api.Binance.Dto;


namespace Ark.Api.Binance.Services;

/// <summary>
/// Service de calcul précis des frais pour optimiser les profits
/// + Calcul en temps réel des frais de trading, funding et liquidation
/// + Prise en compte des tiers VIP et des réductions BNB
/// + Optimisation des montants de click pour garantir PnL positif
/// - Les estimations ne prennent pas encore en compte la volatilité historique
/// TODO: Ajouter support pour les contrats COIN-M
/// TODO: Intégrer l'historique des taux de funding
/// </summary>
    public class FeeCalculationService
    {
    private readonly ILogger<FeeCalculationService> _logger;
    private readonly IOptionsMonitor<BinanceOptions> _options;
        private readonly FeeRulesService _feeRules;

    // Taux de base pour chaque niveau VIP (maker/taker)
    private static readonly (decimal maker, decimal taker)[] VipRates = {
        (0.0002m, 0.0005m), // VIP 0
        (0.00018m, 0.00045m), // VIP 1
        (0.00016m, 0.0004m), // VIP 2
        (0.00014m, 0.00036m), // VIP 3
        (0.00012m, 0.00032m), // VIP 4
        (0.0001m, 0.0003m), // VIP 5
        (0.00008m, 0.00028m), // VIP 6
        (0.00006m, 0.00026m), // VIP 7
        (0.00004m, 0.00024m), // VIP 8
        (0.0m, 0.00017m) // VIP 9
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="FeeCalculationService"/> class.
    /// </summary>
    /// <param name="logger">Logger for diagnostic messages.</param>
    /// <param name="options">Monitor providing Binance configuration.</param>
    /// <param name="feeRules">Service supplying fee and funding rules.</param>
    public FeeCalculationService(
        ILogger<FeeCalculationService> logger,
        IOptionsMonitor<BinanceOptions> options,
        FeeRulesService feeRules)
    {
        _logger = logger;
        _options = options;
        _feeRules = feeRules;
    }

    /// <summary>
    /// Calcule le montant optimal de click pour garantir un PnL positif
    /// + Prend en compte tous les frais: trading, funding, slippage
    /// - Basé sur des hypothèses simplifiées de volatilité
    /// </summary>
    public async Task<ClickOptimizationResultDto> CalculateOptimalClickAmount(
        string symbol,
        decimal currentPrice,
        decimal targetProfitPct,
        int leverage,
        bool isLong,
        int vipLevel = 0,
        bool useBnbDiscount = false)
    {
        try
        {
            // Récupération des règles actuelles
            var rules = await _feeRules.GetFeeRulesAsync(symbol);
            var fundingRate = await _feeRules.GetCurrentFundingRateAsync(symbol);

            // Calcul des frais de trading
            var (makerFee, takerFee) = GetTradingFees(vipLevel, useBnbDiscount);

            // Frais d'entrée (généralement taker pour ordre initial)
            decimal entryFeeRate = takerFee;

            // Frais de sortie (optimisé pour maker si possible)
            decimal exitFeeRate = makerFee;

            // Frais de funding estimé (pour 8h de position)
            decimal fundingFeeRate = Math.Abs(fundingRate) * 1; // 1 période de 8h

            // Frais de liquidation potentiel (pour calcul de sécurité)
            decimal liquidationFeeRate = rules.LiquidationFeeRate;

            // Calcul du montant minimal nécessaire pour couvrir tous les frais
            decimal totalFeeRate = entryFeeRate + exitFeeRate + fundingFeeRate;

            // Marge de sécurité supplémentaire
            decimal safetyMargin = 0.0005m; // 0.05% supplémentaire

            // Profit minimum requis
            decimal minProfitRate = targetProfitPct / 100m;

            // Montant total nécessaire pour le profit + frais
            decimal requiredMoveRate = minProfitRate + totalFeeRate + safetyMargin;

            // Calcul du montant optimal avec effet de levier
            decimal baseAmount = CalculateBaseAmount(currentPrice, requiredMoveRate, leverage);

            // Optimisation finale basée sur la volatilité récente
            var optimizedAmount = await OptimizeForVolatility(symbol, baseAmount, currentPrice);

            var result = new ClickOptimizationResultDto
            {
                OptimalAmount = optimizedAmount,
                EstimatedTotalFees = totalFeeRate * optimizedAmount * currentPrice,
                EstimatedProfit = minProfitRate * optimizedAmount * currentPrice,
                RequiredPriceMove = requiredMoveRate * currentPrice,
                SafetyMarginUsed = safetyMargin,
                FeeBreakdown = new FeeBreakdownDto
                {
                    EntryFee = entryFeeRate * optimizedAmount * currentPrice,
                    ExitFee = exitFeeRate * optimizedAmount * currentPrice,
                    FundingFee = fundingFeeRate * optimizedAmount * currentPrice,
                    SlippageEstimate = CalculateSlippageEstimate(symbol, optimizedAmount, currentPrice)
                }
            };

            _logger.LogInformation(
                "Calcul optimal pour {Symbol}: Montant={Amount}, Frais totaux={Fees}%, Profit cible={Profit}%",
                symbol, optimizedAmount, totalFeeRate * 100, minProfitRate * 100);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors du calcul du montant optimal pour {Symbol}", symbol);
            throw;
        }
    }

    /// <summary>
    /// Valide qu'un ordre générera un profit positif après tous les frais
    /// + Compare les frais totaux au profit brut prévu
    /// - Ne considère pas le glissement ou les variations de funding après l'entrée
    /// </summary>
    public async Task<bool> ValidateProfitabilityAsync(
        string symbol,
        decimal quantity,
        decimal entryPrice,
        decimal exitPrice,
        bool entryIsMaker,
        bool exitIsMaker,
        int vipLevel = 0)
    {
        var fees = await CalculateCompleteTradeFees(
            symbol, quantity, entryPrice, exitPrice, entryIsMaker, exitIsMaker, vipLevel);

        decimal grossProfit = Math.Abs(exitPrice - entryPrice) * quantity;
        decimal netProfit = grossProfit - fees.TotalFees;

        return netProfit > 0;
    }

    private (decimal maker, decimal taker) GetTradingFees(int vipLevel, bool useBnbDiscount)
    {
        vipLevel = Math.Max(0, Math.Min(vipLevel, 9));
        var (maker, taker) = VipRates[vipLevel];

        if (useBnbDiscount)
        {
            maker *= 0.9m; // 10% de réduction
            taker *= 0.9m;
        }

        return (maker, taker);
    }

    private decimal CalculateBaseAmount(decimal price, decimal requiredMoveRate, int leverage)
    {
        // Formule simplifiée - peut être complexifiée selon les besoins
        decimal leverageAdjustment = 1.0m / leverage;
        return requiredMoveRate * leverageAdjustment * 1000; // Base de 1000 USDT
    }

    private Task<decimal> OptimizeForVolatility(string symbol, decimal baseAmount, decimal currentPrice)
    {
        // TODO: Intégrer l'analyse de volatilité historique
        // Pour l'instant, retourne le montant de base
        return Task.FromResult(baseAmount);
    }

    private decimal CalculateSlippageEstimate(string symbol, decimal quantity, decimal price)
    {
        // Estimation simple du slippage basée sur la quantité
        decimal notional = quantity * price;

        if (notional < 1000) return 0;
        if (notional < 10000) return notional * 0.0001m; // 0.01%
        if (notional < 100000) return notional * 0.0002m; // 0.02%

        return notional * 0.0005m; // 0.05% pour les grosses positions
    }

    private async Task<CompleteTradeFees> CalculateCompleteTradeFees(
        string symbol, decimal quantity, decimal entryPrice, decimal exitPrice,
        bool entryIsMaker, bool exitIsMaker, int vipLevel)
    {
        var (makerFee, takerFee) = GetTradingFees(vipLevel, false);

        decimal entryNotional = quantity * entryPrice;
        decimal exitNotional = quantity * exitPrice;

        decimal entryFee = entryNotional * (entryIsMaker ? makerFee : takerFee);
        decimal exitFee = exitNotional * (exitIsMaker ? makerFee : takerFee);

        // Funding fee estimate (si position tenue pendant 8h)
        var fundingRate = await _feeRules.GetCurrentFundingRateAsync(symbol);
        decimal fundingFee = Math.Abs(fundingRate) * (entryNotional + exitNotional) / 2;

        return new CompleteTradeFees
        {
            EntryFee = entryFee,
            ExitFee = exitFee,
            FundingFee = fundingFee,
            TotalFees = entryFee + exitFee + fundingFee
        };
    }

    /// <summary>
    /// Projects initial and maintenance margin requirements for a position.
    /// + Uses notional value and leverage to estimate margin consumption.
    /// - Uses constant maintenance rate pending symbol-specific integration.
    /// </summary>
    /// <param name="symbol">Futures contract symbol.</param>
    /// <param name="quantity">Number of contracts.</param>
    /// <param name="price">Entry price in quote asset.</param>
    /// <param name="leverage">Applied leverage multiplier.</param>
    /// <returns>Estimated margin requirements.</returns>
    public Task<MarginRequirementDto> ProjectMarginAsync(string symbol, decimal quantity, decimal price, int leverage)
    {
        var notional = quantity * price;
        var initial = notional / leverage;
        // Placeholder maintenance margin at 50% of initial until full rule set is implemented
        var maintenance = initial * 0.5m;

        var dto = new MarginRequirementDto
        {
            InitialMargin = initial,
            MaintenanceMargin = maintenance
        };

        return Task.FromResult(dto);
    }
}
