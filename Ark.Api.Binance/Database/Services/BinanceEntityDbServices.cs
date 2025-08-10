using System;
using Ark;
using Ark.App.Diagnostics;
using Ark.Data.EFCore;
using Ark.Data.EFCore.SqlServer;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Ark.Api.Binance
{
    /// <summary>
    /// Base services used to persist Binance related entities.
    /// </summary>
    /// <typeparam name="TEntity">The entity type managed by these services.</typeparam>
    public abstract class BinanceEntityDbServices<TEntity> : DbServices<BinanceDbContext, TEntity>
        where TEntity : DbEntity<BinanceDbContext>, new()
    {
        /// <summary>Logger used when database operations fail.</summary>
        protected ILogger Logger { get; } = Diag.Logs?.Database ?? NullLogger.Instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="BinanceEntityDbServices{TEntity}"/> class.
        /// </summary>
        /// <param name="connectionString">Connection string to the SQL Server database.</param>
        protected BinanceEntityDbServices(string connectionString)
        {
            DbContextEx.OptionsByType.AddOrUpdate(
                typeof(BinanceDbContext),
                new DatabaseOptions { ConnectionString = connectionString, GlobalDateTimeGlobalKind = DateTimeKind.Utc });
        }
    }
}
