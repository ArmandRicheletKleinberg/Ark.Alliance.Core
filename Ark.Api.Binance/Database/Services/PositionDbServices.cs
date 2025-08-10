using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace Ark.Api.Binance
{
    /// <summary>
    /// Repository used to persist Binance position entries.
    /// </summary>
    public class PositionDbServices : BinanceEntityDbServices<PositionDbEntity>
    {
        #region Constructors

        /// <summary>
        /// Creates a <see cref="PositionDbServices"/> instance.
        /// </summary>
        /// <param name="connectionString">The SQL Server connection string to connect the database.</param>
        public PositionDbServices(string connectionString) : base(connectionString) { }

        #endregion Constructors

        /// <summary>
        /// Inserts position entries.
        /// </summary>
        /// <param name="positions">The positions to persist.</param>
        /// <returns>
        /// Success : The positions have been persisted.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        #region Methods (Public)

        public Task<Result> InsertAsync(IEnumerable<PositionDbEntity> positions)
            => Create(positions.ToArray());

        /// <summary>
        /// Retrieves positions matching the criteria.
        /// </summary>
        public Task<Result<PositionDbEntity[]>> GetAsync(System.Guid sessionId, TimeWindow window)
            => Get(query => query
                .Where(p => p.SessionId == sessionId &&
                            p.Timestamp >= window.StartUtc &&
                            p.Timestamp <= window.EndUtc));

        /// <summary>
        /// Deletes positions matching the criteria.
        /// </summary>
        public Task<Result> DeleteAsync(System.Guid sessionId, TimeWindow window)
            => RemoveWhere(p => p.SessionId == sessionId &&
                                 p.Timestamp >= window.StartUtc &&
                                 p.Timestamp <= window.EndUtc);

        #endregion Methods (Public)
    }
}
