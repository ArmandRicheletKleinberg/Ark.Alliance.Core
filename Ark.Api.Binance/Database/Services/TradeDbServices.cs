using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace Ark.Api.Binance
{
    /// <summary>
    /// Repository used to persist Binance trade history entries.
    /// </summary>
    public class TradeDbServices : BinanceEntityDbServices<TradeDbEntity>
    {
        #region Constructors

        /// <summary>
        /// Creates a <see cref="TradeDbServices"/> instance.
        /// </summary>
        /// <param name="connectionString">The SQL Server connection string to connect the database.</param>
        public TradeDbServices(string connectionString) : base(connectionString) { }

        #endregion Constructors

        /// <summary>
        /// Inserts trade history entries.
        /// </summary>
        /// <param name="trades">The trades to persist.</param>
        /// <returns>
        /// Success : The trades have been persisted.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        #region Methods (Public)

        public Task<Result> InsertAsync(IEnumerable<TradeDbEntity> trades)
            => Create(trades.ToArray());

        /// <summary>
        /// Retrieves trades matching the criteria.
        /// </summary>
        public Task<Result<TradeDbEntity[]>> GetAsync(System.Guid sessionId, TimeWindow window)
            => Get(query => query
                .Where(t => t.SessionId == sessionId &&
                            t.Timestamp >= window.StartUtc &&
                            t.Timestamp <= window.EndUtc));

        /// <summary>
        /// Deletes trades matching the criteria.
        /// </summary>
        public Task<Result> DeleteAsync(System.Guid sessionId, TimeWindow window)
            => RemoveWhere(t => t.SessionId == sessionId &&
                                 t.Timestamp >= window.StartUtc &&
                                 t.Timestamp <= window.EndUtc);

        #endregion Methods (Public)
    }
}
