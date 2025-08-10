namespace Ark.Net.MqSeries
{
    /// <summary>
    /// This entity represents a file on the mainframe.
    /// </summary>
    public class FtpFile
    {
        /// <summary>
        /// Name of the file
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Content of the file
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Length of the file
        /// </summary>
        public int Length { get; set; }
    }
}
