using System.IO.Compression;

namespace Ark
{
    /// <summary>
    /// This class extends the HttpResponseMessage class.
    /// </summary>
    public static class HttpResponseMessageExtensibility
    {
        #region Methods

        /// <summary>
        /// Reads the HTTP response content as string even if compressed.
        /// </summary>
        /// <param name="response">The HTTP response to read content as a string.</param>
        /// <returns>The string content read, decompressed if needed.</returns>
        public static async Task<string> ReadContentAsString(this HttpResponseMessage response)
        {
            // Check whether response is not compressed then simply read
            if (response.Content.Headers.ContentEncoding.All(x => x != "gzip"))
                return await response.Content.ReadAsStringAsync();

            // Decompress manually the response stream
            using (var responseStream = await response.Content.ReadAsStreamAsync())
            using (var decompressedStream = new GZipStream(responseStream, CompressionMode.Decompress))
            using (var streamReader = new StreamReader(decompressedStream))
                return await streamReader.ReadToEndAsync();
        }

        #endregion Methods
    }
}
