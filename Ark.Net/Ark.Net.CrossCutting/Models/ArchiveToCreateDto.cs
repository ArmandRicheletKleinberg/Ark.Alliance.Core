using Ark.Net.Models;

namespace Ark.Net.CrossCutting.Models
{
    /// <summary>
    /// The data to the archive document to create.
    /// </summary>
    public class ArchiveToCreateDto
    {
        #region Properties (Public)

        /// <summary>
        /// The document file to upload.
        /// </summary>
        public FileDto File { get; set; }

        /// <summary>
        /// The application into which store the document.
        /// </summary>
        public string Application { get; set; }

        /// <summary>
        /// The time when the document has been created in the application in UTC ISO format.
        /// </summary>
        public string CreationTime { get; set; }

        /// <summary>
        /// The array of the meta data to link with the document in JSON because the auto deserialization of .NET Core does not work with JSON multipart form.
        /// </summary>
        public string MetadatasJson { get; set; }

        #endregion Properties (Public)
    }
}