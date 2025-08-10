using Ark.App.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Ark.Api.Binance
{
    /// <summary>
    /// Diagnostics entry point for the Binance API project.
    /// </summary>
    /// <remarks>
    /// Provides access to loggers and indicators used throughout the project.
    /// </remarks>
    /// <example>
    /// <code>
    /// Diag.Logs.BinanceClient.LogInformation("started");
    /// </code>
    /// </example>
    public class Diag : DiagBase<Loggers, Indicators, Reports>
    {
        /// <summary>Current minimal log level.</summary>
        public static LogLevel MinimumLevel { get; private set; } = LogLevel.Information;

        /// <summary>Applies the desired log level to all loggers.</summary>
        public static void ApplyLogLevel(LogLevel level)
        {
            MinimumLevel = level;
            if (Logs == null)
                return;

            foreach (var prop in typeof(Loggers).GetProperties())
            {
                if (prop.GetValue(Logs) is ILogger logger)
                    prop.SetValue(Logs, new LevelFilteredLogger(logger));
            }
        }
    }
}
