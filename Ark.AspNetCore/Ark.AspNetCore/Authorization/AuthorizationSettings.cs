using System;

namespace Ark.AspNetCore
{
    /// <summary>
    /// Configuration options controlling how authorization is performed in the API.
    /// + Supports multiple strategies via <see cref="AuthorizationTypeEnum"/>.
    /// - Short timeouts or wrong types can lock users out unexpectedly.
    /// Ref: <see href="https://learn.microsoft.com/aspnet/core/security/authorization/introduction"/>
    /// </summary>
    public class AuthorizationSettings
    {
        #region Properties (Public)

        /// <summary>
        /// Gets or sets the authorization provider.
        /// + Defaults to <see cref="AuthorizationTypeEnum.CrossCutting"/> for centralized security.
        /// - Setting to <see cref="AuthorizationTypeEnum.None"/> exposes endpoints publicly.
        /// </summary>
        public AuthorizationTypeEnum Type { get; set; } = AuthorizationTypeEnum.CrossCutting;

        /// <summary>
        /// Gets or sets the duration a user session remains valid before re-authentication is required.
        /// + Enhances security by expiring stale sessions.
        /// - Very small values may degrade user experience.
        /// Example: <c>00:30:00</c> for 30 minutes.
        /// </summary>
        public TimeSpan UserSessionTimeout { get; set; } = TimeSpan.FromMinutes(30);

        #endregion Properties (Public)
    }
}