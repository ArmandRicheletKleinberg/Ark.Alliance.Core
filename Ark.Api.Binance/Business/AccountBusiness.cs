using System;
using System.Threading.Tasks;


namespace Ark.Api.Binance
{
    /// <summary>
    /// Provides account level operations.
    /// + Adjusts trading parameters for a <see cref="BinanceSession"/>.
    /// - Does not create sessions automatically when missing.
    /// Ref: <see href="https://binance-docs.github.io/apidocs/futures/en/#change-initial-leverage-trade"/>
    /// </summary>
    public static class AccountBusiness
    {
        #region Methods

        /// <summary>
        /// Changes the leverage for a symbol on an existing session via
        /// <see cref="AccountHelper.ChangeLeverageAsync(BinanceSession, string, int, System.Threading.CancellationToken)"/>.
        /// + Allows dynamic risk adjustment per symbol.
        /// - Returns <see cref="Result.NotFound"/> if the session is absent in
        ///   <see cref="BinanceSessionManagerCache"/>.
        /// Ref: <see href="https://binance-docs.github.io/apidocs/futures/en/#change-initial-leverage-trade"/>
        /// </summary>
        /// <param name="sessionId">Identifier of the session using standard GUID format.</param>
        /// <param name="symbol">Trading pair symbol, e.g. "BTCUSDT".</param>
        /// <param name="leverage">Desired leverage level in integer form.</param>
        /// <returns>
        /// A <see cref="Result"/> indicating success or failure.
        /// Example JSON:
        /// {
        ///   "Status": "Success"
        /// }
        /// </returns>
        public static Task<Result> ChangeLeverageAsync(System.Guid sessionId, string symbol, int leverage)
        {
            if (!BinanceSessionManagerCache.TryGetSession(sessionId, out var session) || session is null)
                return Task.FromResult(Result.NotFound);

            return session!.ChangeLeverageAsync(symbol, leverage);
        }

        #endregion Methods
    }
}
