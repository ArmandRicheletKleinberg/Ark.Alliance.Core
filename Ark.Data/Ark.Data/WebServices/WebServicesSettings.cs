using System;
using System.Collections.Generic;
using System.Text;

namespace Ark.Data
{
    /// <summary>
    /// Configuration values for consuming a web service.
    /// + Centralizes endpoint and credential information.
    /// - Stores secrets in memory; prefer secure storage when possible.
    /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.net.http.httpclient"/>
    /// </summary>
    public class WebServicesSettings
    {
        #region Properties (Public)

        /// <summary>
        /// Endpoint URL of the web service.
        /// + Must be an absolute URI.
        /// - No validation is performed on assignment.
        /// Ref: <see cref="Uri"/>
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Username used for basic authentication.
        /// + Optional when the service is anonymous.
        /// - Stored in plain text within configuration files.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Password used for basic authentication.
        /// + Optional when the service is anonymous.
        /// - Should be replaced by secrets management in production.
        /// </summary>
        public string Password { get; set; }

        #endregion Properties (Public)
    }
}
