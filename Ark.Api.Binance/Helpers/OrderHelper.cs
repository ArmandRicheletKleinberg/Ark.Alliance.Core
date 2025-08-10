

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ark;
using Ark.Api.Binance;
using Binance.Net.Enums;

#nullable enable

namespace Ark.Api.Binance
{
    /// <summary>
    /// Helper methods to place, modify and cancel orders.
    /// </summary>
    /// <remarks>
    /// These extension methods store order results inside the related <see cref="BinanceSession"/>.
    /// </remarks>
    /// <example>
    /// <code>
    /// await session.PlaceOrderAsync(order);
    /// </code>
    /// </example>
    public static class OrderHelper
    {
        #region Methods (Public)
        /// <summary>
        /// Places an order using the provided session.
        /// </summary>
        /// <param name="session">Active Binance session.</param>
        /// <param name="order">Order parameters.</param>
        /// <param name="token">Cancellation token.</param>
        /// <param name="connectionString">Optional connection string for persisting the order.</param>

        /// <returns>The created order result.</returns>
        public static async Task<Result<OrderResultDto>> PlaceOrderAsync(this BinanceSession session, FuturesOrder order, CancellationToken token = default, string? connectionString = null)
        {
            var result = await session.Client.PlaceOrderAsync(order, token);
            if (result.IsSuccess && result.Data != null)
            {
                session.Orders[result.Data.OrderId] = result;
                if (connectionString != null)
                {
                    var entity = OrderMapper.ToEntity(result.Data, session.Id);
                    _ = Task.Run(async () =>
                    {
                        var repo = new OrderDbServices(connectionString);
                        await repo.InsertAsync(new[] { entity });
                    });
                }
            }
            return result;
        }

        /// <summary>
        /// Modifies an existing order.
        /// </summary>
        /// <param name="session">Active Binance session.</param>
        /// <param name="orderId">Identifier of the order to replace.</param>
        /// <param name="newOrder">New order details.</param>
        /// <param name="token">Cancellation token.</param>
        /// <param name="connectionString">Optional connection string for persisting the order.</param>
        /// <returns>The result of the new order.</returns>
        public static async Task<Result<OrderResultDto>> ModifyOrderAsync(this BinanceSession session, long orderId, FuturesOrder newOrder, CancellationToken token = default, string? connectionString = null)
        {
            var result = await session.Client.ModifyOrderAsync(orderId, newOrder, token);
            if (result.IsSuccess && result.Data != null)
            {
                session.Orders[result.Data.OrderId] = result;
                if (connectionString != null)
                {
                    var entity = OrderMapper.ToEntity(result.Data, session.Id);
                    _ = Task.Run(async () =>
                    {
                        var repo = new OrderDbServices(connectionString);
                        await repo.InsertAsync(new[] { entity });
                    });
                }
            }
            return result;
        }

        /// <summary>
        /// Cancels an order.
        /// </summary>
        /// <param name="session">Active Binance session.</param>
        /// <param name="symbol">Trading symbol.</param>
        /// <param name="orderId">Identifier of the order to cancel.</param>
        /// <param name="token">Cancellation token.</param>
        /// <param name="connectionString">Optional connection string for persisting the cancellation.</param>
        /// <returns>A <see cref="Result"/> describing the cancellation.</returns>
        public static async Task<Result> CancelOrderAsync(this BinanceSession session, string symbol, long orderId, CancellationToken token = default, string? connectionString = null)
        {
            var result = await session.Client.CancelOrderAsync(symbol, orderId, token);
            var dto = new OrderResultDto
            {
                OrderId = orderId,
                Timestamp = DateTime.UtcNow,
                Status = result.IsSuccess ? OrderStatus.Canceled : OrderStatus.Rejected
            };
            session.Orders[orderId] = Result<OrderResultDto>.Success.WithData(dto);

            if (connectionString != null)
            {
                var entity = OrderMapper.ToEntity(dto, session.Id);
                _ = Task.Run(async () =>
                {
                    var repo = new OrderDbServices(connectionString);
                    await repo.InsertAsync(new[] { entity });
                });
            }
            return result;
        }

        #endregion Methods (Public)
    }
}
