using Ark;
using System.Net;
using System.Net.Http;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace Ark.Net.Http
{
    /// <summary>
    /// This class is used to extend <see cref="HttpResponseMessage"/> class.
    /// </summary>
    public static class HttpStatusCodeExtensions
    {
        #region Methods (Public)

        /// <summary>
        /// Whether this status code is a success or not.
        /// An HTTP status is a success if between 200 and 299 included.
        /// </summary>
        /// <param name="httpStatusCode">The HTTP status to test.</param>
        /// <returns>True if this is a success status code.</returns>
        public static bool IsSuccessStatusCode(this HttpStatusCode httpStatusCode)
            => (int)httpStatusCode >= 200 && (int)httpStatusCode <= 299;

        /// <summary>
        /// Converts an HTTP status code to a result status.
        /// </summary>
        /// <param name="httpStatusCode">The HTTP status to convert.</param>
        /// <returns>The converted status code.</returns>
        public static ResultStatus ToResultStatus(this HttpStatusCode httpStatusCode)
        {
            if (httpStatusCode.IsSuccessStatusCode())
                return ResultStatus.Success;

            switch (httpStatusCode)
            {
                case HttpStatusCode.Conflict: return ResultStatus.Already;
                case HttpStatusCode.Unauthorized: return ResultStatus.Unauthorized;
                case HttpStatusCode.Forbidden: return ResultStatus.Unauthorized;
                case HttpStatusCode.ProxyAuthenticationRequired: return ResultStatus.Unauthorized;
                case HttpStatusCode.RequestTimeout: return ResultStatus.Timeout;
                case HttpStatusCode.GatewayTimeout: return ResultStatus.Timeout;
                case HttpStatusCode.NotFound: return ResultStatus.NotFound;
                case HttpStatusCode.Gone: return ResultStatus.NotFound;
                case HttpStatusCode.NotImplemented: return ResultStatus.NotImplemented;
                default: return ResultStatus.Failure;
            }
        }

        #endregion Methods (Public)
    }
}