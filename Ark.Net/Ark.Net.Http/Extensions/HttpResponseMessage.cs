using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace Ark.Net.Http
{
    /// <summary>
    /// This class is used to extend <see cref="HttpResponseMessage"/> class.
    /// </summary>
    public static class HttpResponseMessageExtensions
    {
        #region Methods (Public)

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
            using var responseStream = await response.Content.ReadAsStreamAsync();
            using var decompressedStream = new GZipStream(responseStream, CompressionMode.Decompress);
            using var streamReader = new StreamReader(decompressedStream);
            return await streamReader.ReadToEndAsync();
        }

        #endregion Methods (Public)
    }
}