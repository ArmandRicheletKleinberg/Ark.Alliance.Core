using System.IO;
using System.Threading.Tasks;

namespace Ark.Data
{
    /// <summary>
    /// Provides base helpers for manipulating files.
    /// + Encapsulates <see cref="File"/> operations for consistency.
    /// - Does not create directories or validate destination existence.
    /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.io.file"/>
    /// </summary>
    public class FileRepository
    {
        #region Methods (Public)

        /// <summary>
        /// Writes a file and sends it to a specific address.
        /// + Uses <see cref="File.WriteAllBytes(string,byte[])"/> for atomic writes.
        /// - Overwrites existing files without prompt.
        /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.io.file.writeallbytes"/>
        /// </summary>
        /// <param name="path">Absolute file path where the file will be written.</param>
        /// <param name="fileStream">Binary content of the file.</param>
        /// <returns>
        /// Success : The file has been written and sent successfully.
        /// BadParameters : The file path is null or empty.
        /// Unexpected : An unexpected error occurs.
        /// Example JSON: { "isSuccess": true, "value": "C:\\temp\\file.txt" }
        /// </returns>
        public virtual Task<Result<string>> SendFile(string path, byte[] fileStream) => Task.Run(() => Result<string>.SafeExecute(() =>
        {
            if (path.IsNullOrWhiteSpace())
                return Result<string>.BadParameters.WithReason("Path is empty.");

            File.WriteAllBytes(path, fileStream);
            return Result<string>.Success;
        }));


        /// <summary>
        /// Removes a file at a specific path.
        /// + Wraps <see cref="File.Delete(string)"/> and returns <see cref="Result{T}"/>.
        /// - Does not verify write permissions before deletion.
        /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.io.file.delete"/>
        /// </summary>
        /// <param name="path">The full path of the file (directory + file name).</param>
        /// <returns>
        /// Success : The file has been deleted from the directory.
        /// BadParameters : The file path is null or empty.
        /// Unexpected : An unexpected error occurs.
        /// Example JSON: { "isSuccess": true, "value": null }
        /// </returns>
        public virtual Task<Result<string>> DeleteFile(string path) => Task.Run(() => Result<string>.SafeExecute(() =>
        {
            if (path.IsNullOrWhiteSpace())
                return Result<string>.BadParameters.WithReason("Path is empty.");

            File.Delete(path);
            return Result<string>.Success;
        }));

        #endregion Methods (Public)
    }
}