#nullable enable

namespace Ark.Core.Api.TradingView.Models
{
    /// <summary>
    /// Optional credentials used when calling TradingView endpoints.
    /// + Enables access to authenticated features when supported.
    /// - Uses basic authentication which sends credentials in plain text over TLS.
    /// Ref: <see href="https://developer.mozilla.org/docs/Web/HTTP/Authentication#basic_authentication_scheme"/>
    /// </summary>
    public sealed class TradingViewCredentials
    {
        #region Properties

        /// <summary>Username for basic authentication.</summary>
        public string? Username { get; set; }

        /// <summary>Password for basic authentication.</summary>
        public string? Password { get; set; }

        #endregion Properties
    }
}
