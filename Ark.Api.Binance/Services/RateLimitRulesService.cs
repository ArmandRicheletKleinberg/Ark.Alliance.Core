using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;


namespace Ark.Api.Binance.Services;

/// <summary>
/// Service managing persistence of rate limit rules.
/// + Caches rules for fast retrieval and stores updates in the database.
/// - In-memory cache may deliver stale values until refreshed.
/// </summary>
public class RateLimitRulesService
{
    private readonly BinanceDbContext _context;
    private readonly ILogger<RateLimitRulesService> _logger;
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _cacheExpiry = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Initializes a new instance of the <see cref="RateLimitRulesService"/> class.
    /// </summary>
    /// <param name="context">Database context providing persisted rules.</param>
    /// <param name="logger">Logger for diagnostics.</param>
    /// <param name="cache">In-memory cache for quick rule lookups.</param>
    public RateLimitRulesService(
        BinanceDbContext context,
        ILogger<RateLimitRulesService> logger,
        IMemoryCache cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    /// <summary>
    /// Retrieves configured rate limit rules for an endpoint category.
    /// + Returns cached values when available to minimise database hits.
    /// - Creates default entries when none exist in storage.
    /// </summary>
    /// <param name="category">Endpoint group name.</param>
    /// <returns>The stored rules for the category.</returns>
    public async Task<RateLimitRulesDto> GetRulesAsync(string category = "default")
    {
        var cacheKey = $"rate_limit_rules_{category}";
        if (_cache.TryGetValue(cacheKey, out RateLimitRulesDto? dto) && dto is not null)
            return dto;

        var entity = await _context.RateLimitRules
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.EndpointCategory == category && r.IsActive);

        if (entity == null)
        {
            entity = new RateLimitRulesDbEntity { EndpointCategory = category };
            _context.RateLimitRules.Add(entity);
            await _context.SaveChangesAsync();
        }

        dto = new RateLimitRulesDto
        {
            WeightLimit = entity.WeightLimitPerMinute,
            OrderLimit = entity.OrderLimitPerMinute,
            AlertThreshold = entity.AlertThreshold,
            RecoveryThreshold = entity.RecoveryThreshold
        };

        _cache.Set(cacheKey, dto, _cacheExpiry);
        return dto;
    }

    /// <summary>
    /// Updates and persists rate limit rules for a category.
    /// + Applies changes immediately and clears relevant cache entries.
    /// - Does not validate values against Binance hard limits.
    /// </summary>
    /// <param name="request">New rule values.</param>
    /// <param name="category">Endpoint group name.</param>
    public async Task UpdateRulesAsync(UpdateRateLimitsRequest request, string category = "default")
    {
        var entity = await _context.RateLimitRules
            .FirstOrDefaultAsync(r => r.EndpointCategory == category && r.IsActive)
            ?? new RateLimitRulesDbEntity { EndpointCategory = category, IsActive = true };

        entity.WeightLimitPerMinute = request.WeightLimit;
        entity.OrderLimitPerMinute = request.OrderLimit;
        entity.AlertThreshold = request.AlertThreshold;
        entity.RecoveryThreshold = request.RecoveryThreshold;
        entity.UpdatedAt = DateTime.UtcNow;

        if (entity.Id == 0)
            _context.RateLimitRules.Add(entity);

        await _context.SaveChangesAsync();
        _cache.Remove($"rate_limit_rules_{category}");
        _logger.LogInformation("Rate limit rules updated for {Category}", category);
    }
}
