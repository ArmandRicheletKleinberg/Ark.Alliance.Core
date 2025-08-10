using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ark.Api.Binance;

#nullable enable

namespace Ark.Api.Binance
{
    /// <summary>
    /// Helper methods to manage session positions.
    /// </summary>
    public static class PositionHelper
    {
        #region Methods (Public)
        /// <summary>
        /// Updates the session position dictionary and optionally records
        /// the entries to the database asynchronously.
        /// </summary>
        /// <param name="session">Active Binance session.</param>
        /// <param name="positions">The positions retrieved from Binance.</param>
        /// <param name="connectionString">Optional database connection string.</param>
        public static void UpdatePositions(this BinanceSession session, IEnumerable<PositionDto> positions, string? connectionString = null)
        {
            var list = positions.ToList();
            foreach (var pos in list)
            {
                if (pos.Quantity == 0)
                    session.Positions.TryRemove(pos.Symbol, out _);
                else
                    session.Positions[pos.Symbol] = pos;
            }

            if (connectionString != null && list.Count > 0)
            {
                var entities = list.Select(p => PositionMapper.ToEntity(p, session.Id)).ToList();
                _ = Task.Run(async () =>
                {
                    var repo = new PositionDbServices(connectionString);
                    await repo.InsertAsync(entities);
                });
            }
        }

        #endregion Methods (Public)
    }
}
