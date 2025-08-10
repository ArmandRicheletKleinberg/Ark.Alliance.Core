using System;

namespace Ark.AspNetCore
{
    /// <summary>
    /// Provides server-wide configuration for user session management.
    /// + Centralizes the <see cref="UserDataProfileType"/> used across the application.
    /// - Misconfiguration may prevent profile data serialization.
    /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.type"/>
    /// </summary>
    public class ServerConfig
    {
        #region Properties (Public)

        /// <summary>
        /// Gets or sets the CLR <see cref="Type"/> describing the user-specific profile payload.
        /// + Enables strongly typed profile data when constructing <see cref="UserSession{TProfileData}"/>.
        /// - A mismatched type results in an empty profile being created.
        /// </summary>
        public Type UserDataProfileType { get; set; }

        #endregion Properties (Public)
    }
}