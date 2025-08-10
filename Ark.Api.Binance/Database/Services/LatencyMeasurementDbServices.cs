using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ark.Api.Binance
{
    /// <summary>
    /// Repository used to persist latency measurements.
    /// + Supports batch inserts and time-window queries.
    /// - Large datasets may require external pruning.
    /// </summary>
    public class LatencyMeasurementDbServices : BinanceEntityDbServices<LatencyMeasurementDbEntity>
    {
        #region Constructors

        /// <summary>
        /// Creates a <see cref="LatencyMeasurementDbServices"/> instance.
        /// + Connection string passed directly for flexibility.
        /// - Caller must ensure database availability.
        /// </summary>
        /// <param name="connectionString">SQL Server connection string.</param>
        public LatencyMeasurementDbServices(string connectionString) : base(connectionString) { }

        #endregion Constructors

        #region Methods (Public)

        /// <summary>
        /// Inserts latency measurements in batch.
        /// + Minimizes round-trips using a single transaction.
        /// - Does not deduplicate entries.
        /// </summary>
        /// <param name="measurements">The measurements to persist.</param>
        public Task<Result> InsertAsync(IEnumerable<LatencyMeasurementDbEntity> measurements)
            => Create(measurements.ToArray());

        /// <summary>
        /// Retrieves measurements for an endpoint within a time window.
        /// + Useful for analytics dashboards.
        /// - Endpoint comparison is case-sensitive.
        /// </summary>
        /// <param name="endpoint">The endpoint to filter on.</param>
        /// <param name="window">The time window bounding the search.</param>
        public Task<Result<LatencyMeasurementDbEntity[]>> GetAsync(string endpoint, TimeWindow window)
            => Get(query => query.Where(m => m.Endpoint == endpoint &&
                                            m.MeasuredAt >= window.StartUtc &&
                                            m.MeasuredAt <= window.EndUtc));

        /// <summary>
        /// Deletes measurements matching the criteria.
        /// + Keeps the dataset size manageable.
        /// - Operation is irreversible.
        /// </summary>
        /// <param name="endpoint">The endpoint to filter on.</param>
        /// <param name="window">The time window of measurements to delete.</param>
        public Task<Result> DeleteAsync(string endpoint, TimeWindow window)
            => RemoveWhere(m => m.Endpoint == endpoint &&
                                 m.MeasuredAt >= window.StartUtc &&
                                 m.MeasuredAt <= window.EndUtc);

        #endregion Methods (Public)
    }
}
