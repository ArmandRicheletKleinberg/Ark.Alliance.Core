using System.Threading;
using System.Threading.Tasks;
using Ark;

namespace Ark.Api.Binance
{
    /// <summary>
    /// Utility operations targeting account settings.
    /// + Exposes helpers for leverage and margin adjustments.
    /// - Relies on a configured <see cref="BinanceSession"/> instance.
    /// Ref: <see href="https://binance-docs.github.io/apidocs/futures/en/#change-initial-leverage-trade"/>
    /// </summary>
    public static class AccountHelper
    {
        #region Methods

        /// <summary>
        /// Changes the initial leverage for a symbol.
        /// + Calls Binance Futures API via <see cref="BinanceApiClient.ChangeInitialLeverageAsync"/>.
        /// - Does not validate symbol format before sending.
        /// Ref: <see href="https://binance-docs.github.io/apidocs/futures/en/#change-initial-leverage-trade"/>
        /// </summary>
        /// <param name="session">Active trading session.</param>
        /// <param name="symbol">Trading pair symbol, e.g. "ETHUSDT".</param>
        /// <param name="leverage">Desired leverage level in integer form.</param>
        /// <param name="token">Cancellation token for the operation.</param>
        /// <returns>
        /// A <see cref="Result"/> describing the API outcome.
        /// Example JSON:
        /// {
        ///   "Status": "Success"
        /// }
        /// </returns>
        public static async Task<Result> ChangeLeverageAsync(this BinanceSession session, string symbol, int leverage, CancellationToken token = default)
        {
            var result = await session.Client.ChangeInitialLeverageAsync(symbol, leverage, token);
            return result;
        }

        #endregion Methods
    }
}
