using System;


namespace Ark.Api.Binance
{
    /// <summary>
    /// Helper methods to map ticker information.
    /// + Provides lightweight conversions between persistence and transport models.
    /// - Omits validation such as symbol formatting or timestamp sanity checks.
    /// </summary>
    /// <example>
    /// <code>
    /// var dto = TickerMapper.ToDto(entity);
    /// </code>
    /// </example>
    public static class TickerMapper
    {
        #region Methods (Public)
        /// <summary>
        /// Converts a database entity to a DTO.
        /// + Allocates a new <see cref="TickerDto"/> populated from the entity.
        /// - Ignores additional columns not represented in the DTO.
        /// </summary>
        /// <param name="entity">Ticker entity.</param>
        /// <returns>Ticker DTO.</returns>
        public static TickerDto ToDto(TickerDbEntity entity)
            => new()
            {
                Symbol = entity.Symbol,
                Price = entity.Price,
                Timestamp = entity.Timestamp
            };

        /// <summary>
        /// Converts a DTO to a database entity.
        /// + Includes the owning session identifier for persistence.
        /// - Does not set database-generated values like identifiers.
        /// </summary>
        /// <param name="dto">Ticker DTO.</param>
        /// <param name="sessionId">Owning session id.</param>
        /// <returns>Ticker database entity.</returns>
        public static TickerDbEntity ToEntity(TickerDto dto, System.Guid sessionId)
            => new()
            {
                SessionId = sessionId,
                Symbol = dto.Symbol,
                Price = dto.Price,
                Timestamp = dto.Timestamp
            };

        #endregion Methods (Public)
    }
}
