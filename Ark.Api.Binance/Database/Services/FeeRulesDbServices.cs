using System.Threading.Tasks;

namespace Ark.Api.Binance
{
    /// <summary>
    /// Repository used to manage persisted fee rules.
    /// + Provides simple upsert and lookup helpers.
    /// - Assumes symbols are unique per record.
    /// </summary>
    public class FeeRulesDbServices : BinanceEntityDbServices<FeeRulesDbEntity>
    {
        #region Constructors

        /// <summary>
        /// Creates a <see cref="FeeRulesDbServices"/> instance.
        /// + Connection string is passed directly.
        /// - Caller must ensure the string targets a valid SQL Server.
        /// </summary>
        /// <param name="connectionString">SQL Server connection string.</param>
        public FeeRulesDbServices(string connectionString) : base(connectionString) { }

        #endregion Constructors

        #region Methods (Public)

        /// <summary>
        /// Inserts or updates a fee rule using an UPSERT operation.
        /// + Avoids duplicates when the symbol already exists.
        /// - Last write wins without concurrency checks.
        /// </summary>
        /// <param name="entity">The fee rule to persist.</param>
        public Task<Result> UpsertAsync(FeeRulesDbEntity entity)
            => CreateOrUpdate(entity);

        /// <summary>
        /// Finds a fee rule by symbol.
        /// + Returns <see cref="ResultStatus.NotFound"/> when no entry exists.
        /// - Symbol comparison is case-sensitive.
        /// </summary>
        /// <param name="symbol">The futures symbol to search for.</param>
        public Task<Result<FeeRulesDbEntity>> FindAsync(string symbol)
            => FindWhere(f => f.Symbol == symbol);

        /// <summary>
        /// Deletes the fee rule for a given symbol.
        /// + Removes stale configuration from the database.
        /// - Operation is irreversible.
        /// </summary>
        /// <param name="symbol">The symbol whose fee rule should be removed.</param>
        public Task<Result> DeleteAsync(string symbol)
            => RemoveWhere(f => f.Symbol == symbol);

        #endregion Methods (Public)
    }
}
