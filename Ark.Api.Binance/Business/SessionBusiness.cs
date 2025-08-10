using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Ark.Api.Binance
{
    /// <summary>
    /// Provides session lifecycle operations.
    /// + Centralizes session creation and overview retrieval.
    /// - Sessions are stored in-memory and lost on application restart.
    /// Ref: <see href="https://binance-docs.github.io/apidocs/futures/en/"/>
    /// </summary>
    public static class SessionBusiness
    {
        #region Methods (Public)

        /// <summary>
        /// Creates a new Binance session.
        /// + Configures API credentials and rate limiter for isolated trading.
        /// - Returns <see cref="ResultStatus.Failure"/> when options are invalid.
        /// Ref: <see href="https://binance-docs.github.io/apidocs/futures/en/"/>
        /// </summary>
        /// <param name="options">Binance configuration options.</param>
        /// <returns>
        /// The created session identifier.
        /// Example JSON:
        /// {
        ///   "SessionId": "e0a1f5d9-1c2b-4d2b-9b4f-8f7a9e3c6b2d"
        /// }
        /// </returns>
        public static Result<System.Guid> CreateSession(BinanceOptions options)
        {
            var logger = Diag.Logs?.BinanceClient ?? NullLogger.Instance;
            var id = BinanceSessionManagerCache.CreateSession(options, logger);
            return Result<System.Guid>.Success.WithData(id);
        }

        /// <summary>
        /// Gets the options used when a session was created.
        /// + Allows diagnostics of runtime configuration.
        /// - Returns <see cref="ResultStatus.NotFound"/> if the session does not exist.
        /// Ref: <see href="https://binance-docs.github.io/apidocs/futures/en/"/>
        /// </summary>
        /// <param name="id">Session identifier.</param>
        /// <returns>
        /// The stored options or <see cref="ResultStatus.NotFound"/>.
        /// Example JSON:
        /// {
        ///   "ApiKey": "YOUR_KEY"
        /// }
        /// </returns>
        public static Result<BinanceOptions> GetSessionOptions(System.Guid id)
        {
            if (!BinanceSessionManagerCache.TryGetSession(id, out var session) || session is null)
                return Result<BinanceOptions>.NotFound;

            return Result<BinanceOptions>.Success.WithData(session!.Options);
        }

        /// <summary>
        /// Retrieves aggregated session data including orders and positions.
        /// + Combines current positions and order metrics in a single call.
        /// - Returns <see cref="ResultStatus.NotFound"/> if the session is absent.
        /// Ref: <see href="https://binance-docs.github.io/apidocs/futures/en/"/>
        /// </summary>
        /// <param name="id">Session identifier.</param>
        /// <param name="window">Time window to filter data.</param>
        /// <returns>
        /// The session overview or <see cref="ResultStatus.NotFound"/>.
        /// Example JSON:
        /// {
        ///   "Orders": [],
        ///   "Positions": []
        /// }
        /// </returns>
        public static async Task<Result<SessionOverviewDto>> GetSessionOverviewAsync(System.Guid id, TimeWindow window)
        {
            if (!BinanceSessionManagerCache.TryGetSession(id, out var session) || session is null)
                return Result<SessionOverviewDto>.NotFound;

            var dto = await session!.ToOverviewAsync(window);
            return Result<SessionOverviewDto>.Success.WithData(dto);
        }

        /// <summary>
        /// Simple ping result used for health checks.
        /// + Confirms API layer responsiveness.
        /// - Provides no trading functionality.
        /// Ref: <see href="https://en.wikipedia.org/wiki/Ping_(networking)"/>
        /// </summary>
        /// <returns>
        /// "pong" if the service is reachable.
        /// Example JSON:
        /// {
        ///   "Data": "pong"
        /// }
        /// </returns>
        public static Result<string> Ping()
            => Result<string>.Success.WithData("pong");

        #endregion Methods (Public)
    }
}
