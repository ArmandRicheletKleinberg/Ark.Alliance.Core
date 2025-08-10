using System;
using Ark.Data;
using Microsoft.Extensions.Configuration;

namespace Ark.AspNetCore
{
    /// <inheritdoc />
    /// <summary>
    /// Ce repository de cache garde les informations de l'utilisateur en mémoire.
    /// La durée de validité de ce cache est la durée maximale de session utilisateur.
    /// </summary>
    public class UserSessionCacheRepository : MemoryRepositoryBase<string, UserSession>
    {
        #region Static

        /// <summary>
        /// Lazy loaded and kept in static for performance purpose.
        /// </summary>
        private static TimeSpan? _validityTimeSpan;

        #endregion Static

        #region Methods (Override)

        /// <inheritdoc />
        protected override TimeSpan? ValidityTimeSpan =>
            _validityTimeSpan ??= AppSettingsRepository.Configuration.GetSection("Authorization")?.GetValue<TimeSpan>("UserSessionTimeout");

        #endregion Methods (Override)
    }
}