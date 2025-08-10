using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace Ark.Api.Binance
{
    /// <summary>
    /// Repository used to persist Binance ticker information.
    /// </summary>
    public class TickerDbServices : BinanceEntityDbServices<TickerDbEntity>
    {
        #region Constructors

        /// <summary>
        /// Creates a <see cref="TickerDbServices"/> instance.
        /// </summary>
        /// <param name="connectionString">The SQL Server connection string to connect the database.</param>
        public TickerDbServices(string connectionString) : base(connectionString) { }

        #endregion Constructors

        /// <summary>
        /// Inserts ticker entries.
        /// </summary>
        /// <param name="tickers">The ticker entries to persist.</param>
        /// <returns>
        /// Success : The tickers have been persisted.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        #region Methods (Public)

        public Task<Result> InsertAsync(IEnumerable<TickerDbEntity> tickers)
            => Create(tickers.ToArray());

        /// <summary>
        /// Retrieves tickers matching the criteria.
        /// </summary>
        public Task<Result<TickerDbEntity[]>> GetAsync(System.Guid sessionId, TimeWindow window)
            => Get(query => query
                .Where(t => t.SessionId == sessionId &&
                            t.Timestamp >= window.StartUtc &&
                            t.Timestamp <= window.EndUtc));

        /// <summary>
        /// Deletes tickers matching the criteria.
        /// </summary>
        public Task<Result> DeleteAsync(System.Guid sessionId, TimeWindow window)
            => RemoveWhere(t => t.SessionId == sessionId &&
                                 t.Timestamp >= window.StartUtc &&
                                 t.Timestamp <= window.EndUtc);

        #endregion Methods (Public)
    }
}
