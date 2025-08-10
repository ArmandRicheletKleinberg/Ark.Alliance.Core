using Ark.App.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Ark.Api.Binance
{
    /// <summary>
    /// Defines the loggers used by the Binance API project.
    /// </summary>
    /// <remarks>
    /// Instances are created automatically by <see cref="Diag"/>.
    /// </remarks>
    /// <example>
    /// <code>
    /// Diag.Logs.TickerFollowupService.LogDebug("check");
    /// </code>
    /// </example>
    public class Loggers : LoggersBase
    {
        #region Properties (Public)

        /// <summary>Logger for ticker followup service.</summary>
        public ILogger TickerFollowupService { get; set; } = null!;

        /// <summary>Logger for order followup service.</summary>
        public ILogger OrderFollowupService { get; set; } = null!;

        /// <summary>Logger for position followup service.</summary>
        public ILogger PositionFollowupService { get; set; } = null!;

        /// <summary>Logger for client operations.</summary>
        public ILogger BinanceClient { get; set; } = null!;

        /// <summary>Logger for BinanceApiClientManager operations.</summary>
        public ILogger BinanceApiClientManager { get; set; } = null!;

        /// <summary>Logger for database operations.</summary>
        public ILogger Database { get; set; } = null!;


        #endregion Properties (Public)
    }
}
