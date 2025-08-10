using Microsoft.Extensions.Logging;

namespace Ark.Api.Binance
{
    /// <summary>
    /// Exposes a configurable logging level.
    /// + Enables dynamic verbosity control across services.
    /// - Single setting applies globally; no per-category granularity.
    /// </summary>
    public interface ILoggingOptions
    {
        /// <summary>
        /// Minimal log level to use for diagnostics.
        /// </summary>
        LogLevel LogLevel { get; set; }
    }
}
