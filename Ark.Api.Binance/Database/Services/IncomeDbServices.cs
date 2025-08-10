using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace Ark.Api.Binance
{
    /// <summary>
    /// Repository used to persist Binance income history entries.
    /// </summary>
    public class IncomeDbServices : BinanceEntityDbServices<IncomeDbEntity>
    {
        #region Constructors

        /// <summary>
        /// Creates a <see cref="IncomeDbServices"/> instance.
        /// </summary>
        /// <param name="connectionString">The SQL Server connection string to connect the database.</param>
        public IncomeDbServices(string connectionString) : base(connectionString) { }

        #endregion Constructors

        /// <summary>
        /// Inserts income history entries.
        /// </summary>
        /// <param name="incomes">The income history entries to persist.</param>
        /// <returns>
        /// Success : The incomes have been persisted.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        #region Methods (Public)

        public Task<Result> InsertAsync(IEnumerable<IncomeDbEntity> incomes)
            => Create(incomes.ToArray());

        /// <summary>
        /// Retrieves incomes matching the criteria.
        /// </summary>
        public Task<Result<IncomeDbEntity[]>> GetAsync(System.Guid sessionId, TimeWindow window)
            => Get(query => query
                .Where(i => i.SessionId == sessionId &&
                            i.Time >= window.StartUtc &&
                            i.Time <= window.EndUtc));

        /// <summary>
        /// Deletes incomes matching the criteria.
        /// </summary>
        public Task<Result> DeleteAsync(System.Guid sessionId, TimeWindow window)
            => RemoveWhere(i => i.SessionId == sessionId &&
                                 i.Time >= window.StartUtc &&
                                 i.Time <= window.EndUtc);

        #endregion Methods (Public)
    }
}
