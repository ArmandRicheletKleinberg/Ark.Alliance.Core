using Microsoft.Extensions.Logging;

namespace Ark.App.Diagnostics
{
    /// <summary>
    /// This class extends ILogger class.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public static class ILoggerExtensions
    {
        #region Methods (Public)

        /// <summary>
        /// Logs a result (generally not succeeded).
        /// By default the level will be set to Information for Success, Warning for minor error status: Already and Error for the others.
        /// </summary>
        /// <param name="logger">The logger to log into.</param>
        /// <param name="result">The result to log.</param>
        public static void LogResult(this ILogger logger, Result result)
        {
            LogLevel level;
            switch (result.Status)
            {
                case ResultStatus.Success: level = LogLevel.Information; break;
                case ResultStatus.Already:
                case ResultStatus.Cancelled: level = LogLevel.Warning; break;
                default: level = LogLevel.Error; break;
            }
            logger.Log(level, result.Exception, result.Reason);
        }

        #endregion Methods (Public)
    }
}