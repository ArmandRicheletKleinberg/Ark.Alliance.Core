using System;


namespace Ark.Api.Binance
{
    /// <summary>
    /// Mapping helpers for income history.
    /// + Converts between database entities and DTOs used by services.
    /// - Does not compute derived values like net totals or percentages.
    /// </summary>
    /// <example>
    /// <code>
    /// var dto = IncomeMapper.ToDto(entity);
    /// </code>
    /// </example>
    public static class IncomeMapper
    {
        #region Methods (Public)
        /// <summary>
        /// Converts a database entity to a DTO.
        /// + Preserves amount, fee and status fields for reporting.
        /// - Ignores fields such as internal identifiers.
        /// </summary>
        /// <param name="entity">Database entity.</param>
        /// <returns>Equivalent DTO.</returns>
        public static IncomeSummaryDto ToDto(IncomeDbEntity entity)
            => new()
            {
                Symbol = entity.Symbol,
                Time = entity.Time,
                IncomeType = entity.IncomeType,
                Amount = entity.Amount,
                Fee = entity.Fee,
                NetIncome = entity.NetIncome,
                Status = entity.Status
            };

        /// <summary>
        /// Converts a DTO to a database entity.
        /// + Associates the income entry with a session for persistence.
        /// - Assumes timestamps and amounts are already sanitized.
        /// </summary>
        /// <param name="dto">Income summary DTO.</param>
        /// <param name="sessionId">Identifier of the owning session.</param>
        /// <returns>The database entity.</returns>
        public static IncomeDbEntity ToEntity(IncomeSummaryDto dto, System.Guid sessionId)
            => new()
            {
                SessionId = sessionId,
                Symbol = dto.Symbol,
                Time = dto.Time,
                IncomeType = dto.IncomeType,
                Amount = dto.Amount,
                Fee = dto.Fee,
                NetIncome = dto.NetIncome,
                Status = dto.Status
            };

        #endregion Methods (Public)
    }
}
