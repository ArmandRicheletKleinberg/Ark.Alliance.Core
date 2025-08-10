using System.Net.Http;
using System.Net.Http.Headers;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace Ark.Net.Http
{
    /// <summary>
    /// This class is used to extend <see cref="HttpClient"/> class.
    /// </summary>
    public static class HttpClientExtensions
    {
        #region Methods (Public)

        /// <summary>
        /// Whether this HTTP client accept GZIP compression in response.
        /// </summary>
        /// <param name="client">The HTTP client that needs to accept or not GZIP compression.</param>
        /// <param name="accept">Whether to accept or not GZIP compression in response.</param>
        public static void AcceptGzipCompressionInResponse(this HttpClient client, bool accept = true)
        {
            var header = new StringWithQualityHeaderValue("gzip");

            if (accept) client.DefaultRequestHeaders.AcceptEncoding.Add(header);
            else client.DefaultRequestHeaders.AcceptEncoding.Remove(header);
        }

        #endregion Methods (Public)
    }
}