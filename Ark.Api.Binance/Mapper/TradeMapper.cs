using System;


namespace Ark.Api.Binance
{
    /// <summary>
    /// Mapping helpers for trade history.
    /// + Translates between database entities and transferable DTOs.
    /// - Does not perform currency conversions or side calculations.
    /// </summary>
    /// <example>
    /// <code>
    /// var dto = TradeMapper.ToDto(entity);
    /// </code>
    /// </example>
    public static class TradeMapper
    {
        #region Methods (Public)
        /// <summary>
        /// Converts a database entity to a DTO.
        /// + Copies trade details for external transmission.
        /// - Excludes navigation properties that may exist on the entity.
        /// </summary>
        /// <param name="entity">Trade entity.</param>
        /// <returns>Trade DTO.</returns>
        public static TradeHistoryDto ToDto(TradeDbEntity entity)
            => new()
            {
                Id = entity.Id,
                Symbol = entity.Symbol,
                Side = entity.Side,
                Quantity = entity.Quantity,
                Price = entity.Price,
                Fee = entity.Fee,
                RealizedPnl = entity.RealizedPnl,
                Leverage = entity.Leverage,
                CloseType = entity.CloseType,
                Timestamp = entity.Timestamp,
                Status = entity.Status
            };

        /// <summary>
        /// Converts a DTO to a database entity.
        /// + Associates the trade with its owning session identifier.
        /// - Assumes the DTO values are already validated.
        /// </summary>
        /// <param name="dto">Trade DTO.</param>
        /// <param name="sessionId">Owning session id.</param>
        /// <returns>Trade entity.</returns>
        public static TradeDbEntity ToEntity(TradeHistoryDto dto, System.Guid sessionId)
            => new()
            {
                SessionId = sessionId,
                Id = dto.Id,
                Symbol = dto.Symbol,
                Side = dto.Side,
                Quantity = dto.Quantity,
                Price = dto.Price,
                Fee = dto.Fee,
                RealizedPnl = dto.RealizedPnl,
                Leverage = dto.Leverage,
                CloseType = dto.CloseType,
                Timestamp = dto.Timestamp,
                Status = dto.Status
            };

        #endregion Methods (Public)
    }
}
