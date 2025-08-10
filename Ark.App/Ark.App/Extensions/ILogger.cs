using Microsoft.Extensions.Logging;

// ReSharper disable UnusedMember.Global

namespace Ark.App
{
    /// <summary>
    /// Utility extensions for <see cref="ILogger"/> that simplify logging of
    /// <see cref="Result"/> objects produced by Ark libraries. Instead of
    /// checking the status manually, call <c>LogResult</c>.
    ///
    /// <para>Example usage:</para>
    /// <code>
    /// var res = DoWork();
    /// _logger.LogResult(res);
    /// </code>
    /// Without it, you would choose a <see cref="LogLevel"/> yourself and format
    /// the message.
    ///
    /// The performance overhead is negligible compared to direct
    /// <c>ILogger.Log</c> calls.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public static class ILoggerExtensions
    {
        #region Methods (Public)

        /// <summary>
        /// Logs a result (generally not succeeded).
        /// By default the level will be set to Information for Success, Warning for minor error status: Already, SuccessWithWarnings and Error for the others.
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