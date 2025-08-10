
namespace Ark.Api.Binance
{
    /// <summary>
    /// Helper methods to map position information.
    /// + Facilitates translation between persistence models and DTOs.
    /// - Does not enforce leverage or quantity constraints.
    /// </summary>
    /// <example>
    /// <code>
    /// var dto = PositionMapper.ToDto(entity);
    /// </code>
    /// </example>
    public static class PositionMapper
    {
        #region Methods (Public)
        /// <summary>
        /// Converts a database entity to a DTO.
        /// + Copies position metrics for transport across layers.
        /// - Skips properties such as database keys or concurrency tokens.
        /// </summary>
        /// <param name="entity">Database entity.</param>
        /// <returns>Position DTO.</returns>
        public static PositionDto ToDto(PositionDbEntity entity)
            => new()
            {
                Symbol = entity.Symbol,
                Side = entity.Side,
                Quantity = entity.Quantity,
                EntryPrice = entity.EntryPrice,
                MarkPrice = entity.MarkPrice,
                UnrealizedPnl = entity.UnrealizedPnl,
                Leverage = entity.Leverage,
                Timestamp = entity.Timestamp
            };

        /// <summary>
        /// Converts a DTO to a database entity.
        /// + Assigns the owning session identifier during conversion.
        /// - Assumes the DTO has been validated elsewhere.
        /// </summary>
        /// <param name="dto">Position DTO.</param>
        /// <param name="sessionId">Owning session id.</param>
        /// <returns>Database entity.</returns>
        public static PositionDbEntity ToEntity(PositionDto dto, System.Guid sessionId)
            => new()
            {
                SessionId = sessionId,
                Symbol = dto.Symbol,
                Side = dto.Side,
                Quantity = dto.Quantity,
                EntryPrice = dto.EntryPrice,
                MarkPrice = dto.MarkPrice,
                UnrealizedPnl = dto.UnrealizedPnl,
                Leverage = dto.Leverage,
                Timestamp = dto.Timestamp
            };

        #endregion Methods (Public)
    }
}
