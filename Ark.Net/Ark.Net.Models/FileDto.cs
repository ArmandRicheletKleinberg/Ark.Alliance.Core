namespace Ark.Net.Models
{
    /// <summary>
    /// This DTO holds the basic info of a file (its name, type and content).
    /// </summary>
    public class FileDto
    {
        /// <summary>
        /// The name of the file with extension.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The mime type of the file.
        /// </summary>
        public string MimeType { get; set; }

        /// <summary>
        /// The content of the file.
        /// </summary>
        public byte[] Content { get; set; }
    }
}
