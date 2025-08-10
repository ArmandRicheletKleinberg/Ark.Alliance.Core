using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Ark.Net.Http
{
    /// <summary>
    /// The web download service is used to download from the web.
    /// </summary>
    public class WebDownloadService
    {
        #region Fields

        /// <summary>
        /// The HTTP client used to download the file.
        /// </summary>
        protected HttpClient HttpClient;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Creates a <see cref="WebDownloadService"/> instance.
        /// </summary>
        public WebDownloadService()
        {
            HttpClient = new HttpClient();
        }

        #endregion Constructors

        #region Methods (Public)

        /// <summary>
        /// Downloads a file from the web using HTTP services.
        /// </summary>
        /// <param name="fileUrl">The absolute file URL.</param>
        /// <param name="progressCallback">A callback to know the status of the download (bytes downloaded/total bytes).</param>
        /// <param name="ct">The cancellation token to cancel the procedure.</param>
        /// <returns></returns>
        public async Task<byte[]> DownloadFile(string fileUrl, Action<int, int> progressCallback = null, CancellationToken ct = new CancellationToken())
        {
            try
            {
                using (var stream = await HttpClient.GetStreamAsync(fileUrl))
                {
                    var totalBytes = Convert.ToInt32(stream.Length);
                    var receivedBytes = 0;
                    var fileBytes = new byte[stream.Length];
                    var buffer = new byte[4096];
                    while (true)
                    {
                        // If a cancellation has been requested
                        if (ct.IsCancellationRequested) return new byte[0];

                        var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, ct);
                        if (bytesRead == 0)
                        {
                            await Task.Yield();
                            break;
                        }

                        Array.Copy(buffer, 0, fileBytes, receivedBytes, bytesRead);
                        receivedBytes += bytesRead;

                        progressCallback?.Invoke(receivedBytes, totalBytes);
                    }
                    return fileBytes;
                }
            }
            catch (Exception) { return new byte[0]; }
        }

        #endregion Methods (Public)
    }
}
