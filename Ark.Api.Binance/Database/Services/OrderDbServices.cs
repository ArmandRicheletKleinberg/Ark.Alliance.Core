using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace Ark.Api.Binance
{
    /// <summary>
    /// Repository used to persist Binance orders.
    /// </summary>
    public class OrderDbServices : BinanceEntityDbServices<OrderDbEntity>
    {
        #region Constructors

        /// <summary>
        /// Creates a <see cref="OrderDbServices"/> instance.
        /// </summary>
        /// <param name="connectionString">The SQL Server connection string to connect the database.</param>
        public OrderDbServices(string connectionString) : base(connectionString) { }

        #endregion Constructors

        /// <summary>
        /// Inserts orders.
        /// </summary>
        /// <param name="orders">The orders to persist.</param>
        /// <returns>
        /// Success : The orders have been persisted.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        #region Methods (Public)

        public Task<Result> InsertAsync(IEnumerable<OrderDbEntity> orders)
            => Create(orders.ToArray());

        /// <summary>
        /// Retrieves orders matching the criteria.
        /// </summary>
        public Task<Result<OrderDbEntity[]>> GetAsync(System.Guid sessionId, TimeWindow window)
            => Get(query => query
                .Where(o => o.SessionId == sessionId &&
                            o.Timestamp >= window.StartUtc &&
                            o.Timestamp <= window.EndUtc));

        /// <summary>
        /// Deletes orders matching the criteria.
        /// </summary>
        public Task<Result> DeleteAsync(System.Guid sessionId, TimeWindow window)
            => RemoveWhere(o => o.SessionId == sessionId &&
                                 o.Timestamp >= window.StartUtc &&
                                 o.Timestamp <= window.EndUtc);

        #endregion Methods (Public)
    }
}
