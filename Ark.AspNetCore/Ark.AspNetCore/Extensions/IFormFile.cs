using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Ark.AspNetCore
{
    /// <summary>
    /// This class extend the <see cref="IFormFile"/> class.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public static class IFormFileExtensions
    {
        #region Methods (Public)

        /// <summary>
        /// Gets all the content from a HTTP form file.
        /// It actively waits all the file stream has been uploaded.
        /// </summary>
        /// <param name="file">The form file.</param>
        /// <returns>The file content read from the HTTP form file.</returns>
        public static async Task<byte[]> GetFileContent(this IFormFile file)
        {
            await using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            var fileContent = await stream.ReadAllBytesAsync();

            return fileContent;
        }

        #endregion Methods (Public)
    }
}