using System;
using System.Threading.Tasks;

using Ark;

namespace Ark.Api.Binance
{
    /// <summary>
    /// Provides order management operations for Binance futures.
    /// + Wraps order endpoints with session cache lookups.
    /// - Requires an existing session; none are created on demand.
    /// Ref: <see href="https://binance-docs.github.io/apidocs/futures/en/#new-order-trade"/>
    /// </summary>
    public static class OrderBusiness
    {
        #region Methods (Public)

        /// <summary>
        /// Places a futures order on an existing session.
        /// + Applies session-level rate limit checks before forwarding to Binance.
        /// - Returns <see cref="ResultStatus.NotFound"/> if the session is missing.
        /// Ref: <see href="https://binance-docs.github.io/apidocs/futures/en/#new-order-trade"/>
        /// </summary>
        /// <param name="sessionId">Identifier of the session using standard GUID format.</param>
        /// <param name="order">Order parameters.</param>
        /// <returns>
        /// The created order result or <see cref="ResultStatus.NotFound"/>.
        /// Example JSON:
        /// {
        ///   "OrderId": 123456,
        ///   "Status": "New"
        /// }
        /// </returns>
        public static Task<Result<OrderResultDto>> PlaceOrderAsync(System.Guid sessionId, FuturesOrder order)
        {
            if (!BinanceSessionManagerCache.TryGetSession(sessionId, out var session) || session is null)
                return Task.FromResult(Result<OrderResultDto>.NotFound);

            return session!.PlaceOrderAsync(order);
        }

        /// <summary>
        /// Replaces an order by cancelling it and creating a new one.
        /// + Maintains atomicity by using a cancel/replace pattern.
        /// - Fails with <see cref="ResultStatus.NotFound"/> if the session is missing.
        /// Ref: <see href="https://binance-docs.github.io/apidocs/futures/en/#cancel-replace-order-trade"/>
        /// </summary>
        /// <param name="sessionId">Identifier of the session using standard GUID format.</param>
        /// <param name="orderId">Identifier of the order to replace.</param>
        /// <param name="order">New order parameters.</param>
        /// <returns>
        /// The newly placed order result or <see cref="ResultStatus.NotFound"/>.
        /// Example JSON:
        /// {
        ///   "OrderId": 123457,
        ///   "Status": "Replaced"
        /// }
        /// </returns>
        public static Task<Result<OrderResultDto>> ModifyOrderAsync(System.Guid sessionId, long orderId, FuturesOrder order)
        {
            if (!BinanceSessionManagerCache.TryGetSession(sessionId, out var session) || session is null)
                return Task.FromResult(Result<OrderResultDto>.NotFound);

            return session!.ModifyOrderAsync(orderId, order);
        }

        /// <summary>
        /// Cancels an existing order.
        /// + Releases margin by removing pending orders.
        /// - Returns <see cref="ResultStatus.NotFound"/> when the session is absent.
        /// Ref: <see href="https://binance-docs.github.io/apidocs/futures/en/#cancel-order-trade"/>
        /// </summary>
        /// <param name="sessionId">Identifier of the session using standard GUID format.</param>
        /// <param name="symbol">Trading symbol such as "BTCUSDT".</param>
        /// <param name="orderId">Order identifier from Binance.</param>
        /// <returns>
        /// A result describing the cancellation.
        /// Example JSON:
        /// {
        ///   "Status": "Canceled"
        /// }
        /// </returns>
        public static Task<Result> CancelOrderAsync(System.Guid sessionId, string symbol, long orderId)
        {
            if (!BinanceSessionManagerCache.TryGetSession(sessionId, out var session) || session is null)
                return Task.FromResult(Result.NotFound);

            return session!.CancelOrderAsync(symbol, orderId);
        }

        #endregion Methods (Public)
    }
}
