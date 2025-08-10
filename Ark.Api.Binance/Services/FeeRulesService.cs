using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Ark.Api.Binance.Services
{
    /// <summary>
    /// Manages retrieval and caching of Binance fee rules.
    /// + Uses <see cref="IMemoryCache"/> to reduce database queries.
    /// - Cached rules may be stale for up to five minutes.
    /// Ref: <see href="https://learn.microsoft.com/dotnet/core/extensions/caching/memory"/>
    /// </summary>
    public class FeeRulesService
    {
        #region Fields

        private readonly BinanceDbContext _context;
        private readonly ILogger<FeeRulesService> _logger;
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _cacheExpiry = TimeSpan.FromMinutes(5);

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FeeRulesService"/> class.
        /// </summary>
        /// <param name="context">Entity Framework context used to access fee rules.</param>
        /// <param name="logger">Logger for diagnostic information.</param>
        /// <param name="cache">Memory cache for storing recent fee rules.</param>
        public FeeRulesService(
            BinanceDbContext context,
            ILogger<FeeRulesService> logger,
            IMemoryCache cache)
        {
            _context = context;
            _logger = logger;
            _cache = cache;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Retrieves active fee rules for a trading symbol.
        /// + Returns cached values when available.
        /// - Creates default rules if none exist.
        /// </summary>
        /// <param name="symbol">Trading pair identifier, e.g. <c>BTCUSDT</c>.</param>
        /// <returns>
        /// Active <see cref="FeeRulesDbEntity"/>. Example:
        /// <code>
        /// {
        ///   "symbol": "BTCUSDT",
        ///   "makerFeeVip0": 0.0002,
        ///   "takerFeeVip0": 0.0005
        /// }
        /// </code>
        /// </returns>
        /// <remarks>Ref: <see href="https://binance-docs.github.io/apidocs/futures/en/"/></remarks>
        public async Task<FeeRulesDbEntity> GetFeeRulesAsync(string symbol)
        {
            string cacheKey = $"fee_rules_{symbol}";

            if (_cache.TryGetValue(cacheKey, out FeeRulesDbEntity? cachedRules) && cachedRules is not null)
            {
                return cachedRules;
            }

            var rules = await _context.FeeRules
                .FirstOrDefaultAsync(f => f.Symbol == symbol && f.IsActive);

            if (rules == null)
            {
                rules = await CreateDefaultFeeRules(symbol);
            }

            _cache.Set(cacheKey, rules, _cacheExpiry);
            return rules;
        }

        /// <summary>
        /// Gets the current funding rate for a symbol.
        /// + Leverages cached rules for efficiency.
        /// - Returns 0 when rules are missing.
        /// </summary>
        /// <param name="symbol">Trading pair to inspect.</param>
        /// <returns>Funding rate expressed as a decimal fraction.</returns>
        public async Task<decimal> GetCurrentFundingRateAsync(string symbol)
        {
            var rules = await GetFeeRulesAsync(symbol);
            return rules.CurrentFundingRate;
        }

        /// <summary>
        /// Creates default fee rules when none exist for the symbol.
        /// + Guarantees callers receive a valid configuration.
        /// - Placeholder values may differ from exchange data.
        /// </summary>
        /// <param name="symbol">Trading symbol requiring default rules.</param>
        /// <returns>Newly persisted <see cref="FeeRulesDbEntity"/>.</returns>
        private async Task<FeeRulesDbEntity> CreateDefaultFeeRules(string symbol)
        {
            var defaultRules = new FeeRulesDbEntity
            {
                Symbol = symbol,
                MakerFeeVip0 = 0.0002m,
                TakerFeeVip0 = 0.0005m,
                LiquidationFeeRate = 0.0125m,
                CurrentFundingRate = 0.0001m,
                NextFundingTime = DateTime.UtcNow.AddHours(8),
                BnbDiscountEnabled = true,
                VipRatesJson = "{}",
                UpdatedBy = "System",
                IsActive = true
            };

            _context.FeeRules.Add(defaultRules);
            await _context.SaveChangesAsync();
            return defaultRules;
        }

        #endregion Methods
    }
}