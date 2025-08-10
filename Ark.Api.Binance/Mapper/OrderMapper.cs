using System;
using Ark.Api.Binance;

namespace Ark.Api.Binance
{
    /// <summary>
    /// Helpers to map order entities and DTOs.
    /// + Ensures consistent transformations between database and API layers.
    /// - Performs no validation of business rules or symbol filters.
    /// </summary>
    /// <example>
    /// <code>
    /// var entity = OrderMapper.ToEntity(dto, sessionId);
    /// </code>
    /// </example>
    public static class OrderMapper
    {
        #region Methods (Public)
        /// <summary>
        /// Maps a database entity to a DTO.
        /// + Copies matching fields without allocations.
        /// - Returns default values when source fields are unset.
        /// </summary>
        /// <param name="entity">Database order entity.</param>
        /// <returns>Order result DTO.</returns>
        public static OrderResultDto ToDto(OrderDbEntity entity)
            => new()
            {
                OrderId = entity.OrderId,
                Symbol = entity.Symbol,
                Side = entity.Side,
                Type = entity.Type,
                Quantity = entity.Quantity,
                Price = entity.Price,
                StopPrice = entity.StopPrice,
                TimeInForce = entity.TimeInForce,
                ReduceOnly = entity.ReduceOnly,
                PositionSide = entity.PositionSide,
                ClientOrderId = entity.ClientOrderId,
                Status = entity.Status,
                Timestamp = entity.Timestamp
            };

        /// <summary>
        /// Maps a DTO to a database entity.
        /// + Prepares data for persistence with the owning session id.
        /// - Ignores invariants such as unique client order identifiers.
        /// </summary>
        /// <param name="dto">Order DTO.</param>
        /// <param name="sessionId">Owning session id.</param>
        /// <returns>Database entity.</returns>
        public static OrderDbEntity ToEntity(OrderResultDto dto, System.Guid sessionId)
            => new()
            {
                SessionId = sessionId,
                OrderId = dto.OrderId,
                Symbol = dto.Symbol,
                Side = dto.Side,
                Type = dto.Type,
                Quantity = dto.Quantity,
                Price = dto.Price,
                StopPrice = dto.StopPrice,
                TimeInForce = dto.TimeInForce,
                ReduceOnly = dto.ReduceOnly,
                PositionSide = dto.PositionSide,
                ClientOrderId = dto.ClientOrderId,
                Status = dto.Status,
                Timestamp = dto.Timestamp
            };

        #endregion Methods (Public)
    }
}
