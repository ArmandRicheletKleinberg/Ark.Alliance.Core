
using System;
using System.Threading.Tasks;
using Ark.Api.Binance;
using Ark.Net.Models;
using Microsoft.AspNetCore.Mvc;

namespace Ark.AspNetCore
{
    /// <summary>
    /// Controller exposing operations to manage Binance futures orders.
    /// </summary>
    [ApiController]
    [ApiExplorerSettings(GroupName = "ο Binance")]
    public class BinanceOrderController<TUserProfileData> : ControllerBase<TUserProfileData>
            where TUserProfileData : new()
    {
        #region Methods (Public)

        /// <summary>
        /// Places a new futures order for the specified session.
        /// </summary>
        /// <param name="sessionId">Identifier of the session.</param>
        /// <param name="order">Order parameters.</param>
        /// <returns>The created order result.</returns>
        /// <remarks>
        /// ## Example ##
        /// ```
        /// POST api/binance/sessions/{sessionId}/orders
        /// ```
        /// </remarks>
        /// <response code="200">Success - order created.</response>
        /// <response code="404">Session not found.</response>
        /// <response code="500">Unexpected error.</response>
        [HttpPost("binance/sessions/{sessionId}/orders")]
        public Task<ResultDto<OrderResultDto>> PlaceOrder(Guid sessionId, [FromBody] FuturesOrder order)
            => ExecuteBlAsync(() => OrderBusiness.PlaceOrderAsync(sessionId, order));

        /// <summary>
        /// Replaces an existing order by cancelling it and creating a new one.
        /// </summary>
        /// <param name="sessionId">Identifier of the session.</param>
        /// <param name="orderId">Identifier of the order to replace.</param>
        /// <param name="order">New order details.</param>
        /// <returns>The result of the newly placed order.</returns>
        /// <remarks>
        /// ## Example ##
        /// ```
        /// PUT api/binance/sessions/{sessionId}/orders/{orderId}
        /// ```
        /// </remarks>
        /// <response code="200">Success - order replaced.</response>
        /// <response code="404">Session not found.</response>
        /// <response code="500">Unexpected error.</response>
        [HttpPut("binance/sessions/{sessionId}/orders/{orderId}")]
        public Task<ResultDto<OrderResultDto>> ModifyOrder(Guid sessionId, long orderId, [FromBody] FuturesOrder order)
            => ExecuteBlAsync(() => OrderBusiness.ModifyOrderAsync(sessionId, orderId, order));

        /// <summary>
        /// Cancels an existing order.
        /// </summary>
        /// <param name="sessionId">Identifier of the session.</param>
        /// <param name="symbol">Trading symbol.</param>
        /// <param name="orderId">Identifier of the order to cancel.</param>
        /// <returns>A <see cref="Result"/> describing the cancellation.</returns>
        /// <remarks>
        /// ## Example ##
        /// ```
        /// DELETE api/binance/sessions/{sessionId}/orders/{orderId}
        /// ```
        /// </remarks>
        /// <response code="200">Success - order cancelled.</response>
        /// <response code="404">Session not found.</response>
        /// <response code="500">Unexpected error.</response>
        [HttpDelete("binance/sessions/{sessionId}/orders/{orderId}")]
        public Task<ResultDto> CancelOrder(Guid sessionId, string symbol, long orderId)
            => ExecuteBlAsync(() => OrderBusiness.CancelOrderAsync(sessionId, symbol, orderId));

        /// <summary>
        /// Transfers funds from the futures wallet to the funding wallet.
        /// </summary>
        [HttpPost("binance/sessions/{sessionId}/wallet/futures-to-funding")]
        public Task<ResultDto> TransferFuturesToFunding(Guid sessionId, string asset, decimal quantity)
            => ExecuteBlAsync(() => WalletBusiness.TransferFuturesToFundingAsync(sessionId, asset, quantity));

        /// <summary>
        /// Transfers funds from the funding wallet to the futures wallet.
        /// </summary>
        [HttpPost("binance/sessions/{sessionId}/wallet/funding-to-futures")]
        public Task<ResultDto> TransferFundingToFutures(Guid sessionId, string asset, decimal quantity)
            => ExecuteBlAsync(() => WalletBusiness.TransferFundingToFuturesAsync(sessionId, asset, quantity));

        #endregion Methods (Public)
    }
}
