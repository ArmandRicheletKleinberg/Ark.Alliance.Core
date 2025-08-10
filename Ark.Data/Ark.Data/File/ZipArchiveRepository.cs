using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace Ark.Data
{
    /// <summary>
    /// Repository for compressing and extracting ZIP archives.
    /// + Wraps <see cref="ZipArchive"/> for in-memory operations.
    /// - Requires all file contents to be loaded into memory.
    /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.io.compression.ziparchive"/>
    /// </summary>
    public class ZipArchiveRepository
    {
        #region Methods (Public)

        /// <summary>
        /// Creates a ZIP archive from provided files.
        /// + Streams entries sequentially via <see cref="ZipArchive.CreateEntry(string)"/>.
        /// - Does not check for duplicate file names.
        /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.io.compression.ziparchive.createentry"/>
        /// </summary>
        /// <param name="files">Files to include in the archive keyed by name.</param>
        /// <returns>
        /// Success : The ZIP archive has been created.
        /// Unexpected : An unexpected error occurs.
        /// Example JSON: { "isSuccess": true, "value": "UEsDB..." }
        /// </returns>
        public virtual async Task<Result<byte[]>> CreateArchive(Dictionary<string, byte[]> files)
        {
            try
            {
                using (var zipStream = new MemoryStream())
                {
                    using (var zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Create, false))
                        foreach (var file in files)
                        {
                            var entry = zipArchive.CreateEntry(file.Key);
                            using (var entryStream = entry.Open())
                                await entryStream.WriteAsync(file.Value, 0, file.Value.Length);
                        }

                    return new Result<byte[]>(zipStream.ToArray());
                }
            }
            catch (Exception exception)
            {
                return new Result<byte[]>(exception);
            }
        }

        /// <summary>
        /// Extracts files from a ZIP archive.
        /// + Returns only entries with content.
        /// - Ignores directory entries and zero-length files.
        /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.io.compression.zipfile.openread"/>
        /// </summary>
        /// <param name="zipPathFile">Full path of the archive file.</param>
        /// <returns>
        /// Success : Dictionary mapping file name to byte content.
        /// Unexpected : An unexpected error occurs.
        /// Example JSON: { "isSuccess": true, "value": { "file.txt": "SGVsbG8=" } }
        /// </returns>
        public virtual Task<Result<Dictionary<string, byte[]>>> UnzipArchive(string zipPathFile) => Task.Run(() =>
        {
            try
            {
                using var zip = ZipFile.OpenRead(zipPathFile);
                return new Result<Dictionary<string, byte[]>>(zip.Entries.Where(e => e.Length > 0).ToDictionary(c => c.Name, c => c.Open().ReadFully()));
            }
            catch (Exception exception)
            {
                return new Result<Dictionary<string, byte[]>>(exception);
            }

        });

        #endregion Methods (Public)
    }
}