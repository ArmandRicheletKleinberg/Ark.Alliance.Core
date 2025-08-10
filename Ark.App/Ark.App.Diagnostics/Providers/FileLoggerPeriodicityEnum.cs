namespace Ark.App.Diagnostics
{
    /// <summary>
    /// The periodicity to write into the same file for the file logger.
    /// </summary>
    public enum FileLoggerPeriodicityEnum
    {

        /// <summary>
        /// A file by day.
        /// Default.
        /// </summary>
        Daily = 0,

        /// <summary>
        /// A file by hour.
        /// </summary>
        Hourly = 1,

        /// <summary>
        /// A file by minute.
        /// </summary>
        Minutely = 2,

        /// <summary>
        /// A file by month.
        /// </summary>
        Monthly = 3
    }
}