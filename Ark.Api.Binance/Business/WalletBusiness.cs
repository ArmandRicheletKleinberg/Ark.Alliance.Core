using System;
using System.Threading.Tasks;


namespace Ark.Api.Binance
{
    /// <summary>
    /// Handles asset transfers between Binance futures and funding wallets.
    /// + Simplifies rebalancing profit and collateral.
    /// - Requires an existing session; none are created automatically.
    /// Ref: <see href="https://binance-docs.github.io/apidocs/futures/en/#asset-transfer-futures-funding-wallet-trade"/>
    /// </summary>
    public static class WalletBusiness
    {
        #region Methods (Public)

        /// <summary>
        /// Transfers funds from futures to funding wallet.
        /// + Moves realized profit to funding account.
        /// - Returns <see cref="Result.NotFound"/> if the session is missing.
        /// Ref: <see href="https://binance-docs.github.io/apidocs/futures/en/#asset-transfer-futures-funding-wallet-trade"/>
        /// </summary>
        /// <param name="sessionId">Identifier of the session using standard GUID format.</param>
        /// <param name="asset">Asset symbol, e.g. "USDT".</param>
        /// <param name="quantity">Quantity to transfer.</param>
        /// <returns>
        /// A <see cref="Result"/> indicating success or failure.
        /// Example JSON:
        /// {
        ///   "Status": "Success"
        /// }
        /// </returns>
        public static Task<Result> TransferFuturesToFundingAsync(System.Guid sessionId, string asset, decimal quantity)
        {
            if (!BinanceSessionManagerCache.TryGetSession(sessionId, out var session) || session is null)
                return Task.FromResult(Result.NotFound);

            return session!.TransferFuturesToFundingAsync(asset, quantity);
        }

        /// <summary>
        /// Transfers funds from funding wallet to futures.
        /// + Provides collateral for new positions.
        /// - Returns <see cref="Result.NotFound"/> if the session is missing.
        /// Ref: <see href="https://binance-docs.github.io/apidocs/futures/en/#asset-transfer-futures-funding-wallet-trade"/>
        /// </summary>
        /// <param name="sessionId">Identifier of the session using standard GUID format.</param>
        /// <param name="asset">Asset symbol, e.g. "USDT".</param>
        /// <param name="quantity">Quantity to transfer.</param>
        /// <returns>
        /// A <see cref="Result"/> indicating success or failure.
        /// Example JSON:
        /// {
        ///   "Status": "Success"
        /// }
        /// </returns>
        public static Task<Result> TransferFundingToFuturesAsync(System.Guid sessionId, string asset, decimal quantity)
        {
            if (!BinanceSessionManagerCache.TryGetSession(sessionId, out var session) || session is null)
                return Task.FromResult(Result.NotFound);

            return session!.TransferFundingToFuturesAsync(asset, quantity);
        }

        #endregion Methods (Public)
    }
}
