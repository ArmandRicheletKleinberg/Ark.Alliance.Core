using Ark.Data.EFCore.SqlServer;
using Microsoft.EntityFrameworkCore;

namespace Ark.Api.Binance
{
    /// <summary>
    /// Entity Framework context for Binance related entities.
    /// </summary>
    public class BinanceDbContext : SqlServerDbContext
    {
        /// <summary>Database table for fee rules.</summary>
        public DbSet<FeeRulesDbEntity> FeeRules => Set<FeeRulesDbEntity>();

        /// <summary>Database table for rate limit rules.</summary>
        public DbSet<RateLimitRulesDbEntity> RateLimitRules => Set<RateLimitRulesDbEntity>();

        /// <summary>Database table for latency measurements.</summary>
        public DbSet<LatencyMeasurementDbEntity> LatencyMeasurements => Set<LatencyMeasurementDbEntity>();
    }
}
