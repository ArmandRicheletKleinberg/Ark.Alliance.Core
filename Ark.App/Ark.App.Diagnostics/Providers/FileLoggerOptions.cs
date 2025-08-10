namespace Ark.App.Diagnostics
{
    /// <summary>
    /// Options that describe the behavior for file file logging.
    /// </summary>
    public class FileLoggerOptions : BatchingLoggerOptions
    {
        #region Properties (Public)

        /// <summary>
        /// The maximum log size in bytes or 0 for no limit.
        /// Default to <c>10MB</c>.
        /// </summary>
        public int FileSizeLimit { get; set; } = 10 * 1024 * 1024;

        /// <summary>
        /// Gets or sets a strictly positive value representing the maximum retained file count or 0 for no limit.
        /// Defaults to <c>2</c>.
        /// </summary>
        public int RetainedFileCountLimit { get; set; } = 2;

        /// <summary>
        /// Gets or sets the filename prefix to use for log files.
        /// Defaults to <c>log-</c>.
        /// </summary>
        public string FilePrefixName { get; set; } = "log-";

        /// <summary>
        /// Gets or sets the filename extension to use for log files.
        /// Defaults to <c>txt</c>.
        /// </summary>
        public string FileExtension { get; set; } = "txt";

        /// <summary>
        /// Gets or sets the periodicity for rolling over log files.
        /// </summary>
        public FileLoggerPeriodicityEnum Periodicity { get; set; } = FileLoggerPeriodicityEnum.Daily;

        /// <summary>
        /// The directory in which log files will be written, relative to the app process.
        /// Default to <c>Logs</c>
        /// </summary>
        /// <returns></returns>
        public string LogDirectory { get; set; } = "Logs";

        #endregion Properties (Public)
    }
}