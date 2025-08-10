using System.Threading.Tasks;

namespace Ark.Api.Binance
{
    /// <summary>
    /// Repository used to manage rate limit rules.
    /// + Centralizes endpoint quota information.
    /// - Does not enforce limits by itself.
    /// </summary>
    public class RateLimitRulesDbServices : BinanceEntityDbServices<RateLimitRulesDbEntity>
    {
        #region Constructors

        /// <summary>
        /// Creates a <see cref="RateLimitRulesDbServices"/> instance.
        /// + Connection string is provided directly.
        /// - Caller must validate the connection string.
        /// </summary>
        /// <param name="connectionString">SQL Server connection string.</param>
        public RateLimitRulesDbServices(string connectionString) : base(connectionString) { }

        #endregion Constructors

        #region Methods (Public)

        /// <summary>
        /// Inserts or updates a rate limit rule.
        /// + Uses UPSERT semantics to avoid duplicates.
        /// - Last write wins without conflict detection.
        /// </summary>
        /// <param name="entity">The rate limit rule to persist.</param>
        public Task<Result> UpsertAsync(RateLimitRulesDbEntity entity)
            => CreateOrUpdate(entity);

        /// <summary>
        /// Finds a rate limit rule by endpoint category.
        /// + Returns <see cref="ResultStatus.NotFound"/> when the rule is missing.
        /// - Endpoint category comparison is case-sensitive.
        /// </summary>
        /// <param name="endpointCategory">The endpoint category to search for.</param>
        public Task<Result<RateLimitRulesDbEntity>> FindAsync(string endpointCategory)
            => FindWhere(r => r.EndpointCategory == endpointCategory);

        /// <summary>
        /// Deletes a rate limit rule by endpoint category.
        /// + Cleans obsolete quota information.
        /// - Deletion cannot be undone.
        /// </summary>
        /// <param name="endpointCategory">The endpoint category whose rule should be removed.</param>
        public Task<Result> DeleteAsync(string endpointCategory)
            => RemoveWhere(r => r.EndpointCategory == endpointCategory);

        #endregion Methods (Public)
    }
}
