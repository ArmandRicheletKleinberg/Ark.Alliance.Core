using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace Ark.Api.Binance
{
    /// <summary>
    /// Repository used to persist Binance sessions.
    /// </summary>
    public class BinanceSessionDbServices : BinanceEntityDbServices<BinanceSessionDbEntity>
    {
        #region Constructors

        /// <summary>
        /// Creates a <see cref="BinanceSessionDbServices"/> instance.
        /// </summary>
        /// <param name="connectionString">The SQL Server connection string to connect the database.</param>
        public BinanceSessionDbServices(string connectionString) : base(connectionString) { }

        #endregion Constructors

        /// <summary>
        /// Inserts a Binance session.
        /// </summary>
        /// <param name="session">The session to persist.</param>
        /// <returns>
        /// Success : The session has been persisted.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        #region Methods (Public)

        public Task<Result> InsertAsync(BinanceSessionDbEntity session)
            => Create(session);

        /// <summary>
        /// Retrieves sessions for an owner within a time window.
        /// </summary>
        public Task<Result<BinanceSessionDbEntity[]>> GetAsync(string ownerId, TimeWindow window)
            => Get(query => query
                .Where(s => s.OwnerId == ownerId &&
                            s.Created >= window.StartUtc &&
                            s.Created <= window.EndUtc));

        /// <summary>
        /// Deletes sessions for an owner within a time window.
        /// </summary>
        public Task<Result> DeleteAsync(string ownerId, TimeWindow window)
            => RemoveWhere(s => s.OwnerId == ownerId &&
                                 s.Created >= window.StartUtc &&
                                 s.Created <= window.EndUtc);

        /// <summary>
        /// Retrieves a session with its related data.
        /// </summary>
        public Task<Result<BinanceSessionDbEntity>> GetByIdWithDetailsAsync(System.Guid sessionId)
            => Find(query => query
                .Where(s => s.Id == sessionId)
                .Include(s => s.Orders)
                .Include(s => s.Positions)
                .Include(s => s.Tickers)
                .Include(s => s.Trades)
                .Include(s => s.Incomes));

        #endregion Methods (Public)
    }
}
