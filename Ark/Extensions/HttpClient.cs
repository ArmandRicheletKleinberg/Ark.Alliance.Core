using System.Net.Http.Headers;

namespace Ark
{
    /// <summary>
    /// This class extends the HttpClient class.
    /// </summary>
    public static class HttpClientExtensibility
    {
        #region Methods

        /// <summary>
        /// Whether this HTTP client accept GZIP compression in response.
        /// </summary>
        /// <param name="client">The HTTP client that needs to accept or not GZIP compression.</param>
        /// <param name="accept">Whether to accept or not GZIP compression in response.</param>
        /// <param name="maxEstimatedDecompressedSizeBytes">
        ///     Optional limit used to set <see cref="HttpClient.MaxResponseContentBufferSize"/>.
        ///     This value represents the maximum expected size of the decompressed content. The
        ///     default value is 100 MB.
        /// </param>
        public static void AcceptGzipCompressionInResponse(this HttpClient client, bool accept = true,
            long maxEstimatedDecompressedSizeBytes = 100 * 1024 * 1024)
        {
            SetCompression(client, "gzip", accept, maxEstimatedDecompressedSizeBytes);
        }

        /// <summary>
        /// Whether this HTTP client accept deflate compression in response.
        /// </summary>
        /// <param name="client">The HTTP client that needs to accept or not deflate compression.</param>
        /// <param name="accept">Whether to accept or not deflate compression in response.</param>
        /// <param name="maxEstimatedDecompressedSizeBytes">Maximum expected decompressed size in bytes.</param>
        public static void AcceptDeflateCompressionInResponse(this HttpClient client, bool accept = true,
            long maxEstimatedDecompressedSizeBytes = 100 * 1024 * 1024)
        {
            SetCompression(client, "deflate", accept, maxEstimatedDecompressedSizeBytes);
        }

        /// <summary>
        /// Whether this HTTP client accept Brotli compression in response.
        /// </summary>
        /// <param name="client">The HTTP client that needs to accept or not Brotli compression.</param>
        /// <param name="accept">Whether to accept or not Brotli compression in response.</param>
        /// <param name="maxEstimatedDecompressedSizeBytes">Maximum expected decompressed size in bytes.</param>
        public static void AcceptBrotliCompressionInResponse(this HttpClient client, bool accept = true,
            long maxEstimatedDecompressedSizeBytes = 100 * 1024 * 1024)
        {
            SetCompression(client, "br", accept, maxEstimatedDecompressedSizeBytes);
        }

        /// <summary>
        /// Applies the specified compression method support on the client.
        /// </summary>
        /// <param name="client">The HTTP client.</param>
        /// <param name="encoding">Encoding name such as <c>gzip</c>, <c>deflate</c> or <c>br</c>.</param>
        /// <param name="accept">Whether to accept or remove the specified encoding.</param>
        /// <param name="maxEstimatedDecompressedSizeBytes">Maximum expected decompressed size in bytes.</param>
        private static void SetCompression(HttpClient client, string encoding, bool accept,
            long maxEstimatedDecompressedSizeBytes)
        {
            var header = new StringWithQualityHeaderValue(encoding);

            if (accept) client.DefaultRequestHeaders.AcceptEncoding.Add(header);
            else client.DefaultRequestHeaders.AcceptEncoding.Remove(header);

            client.MaxResponseContentBufferSize = maxEstimatedDecompressedSizeBytes;
        }

        #endregion Methods
    }
}
