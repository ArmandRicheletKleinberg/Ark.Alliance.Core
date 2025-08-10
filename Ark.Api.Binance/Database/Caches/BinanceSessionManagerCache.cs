using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;


namespace Ark.Api.Binance
{
    /// <summary>
    /// Manages multiple <see cref="BinanceSession"/> instances.
    /// </summary>
    /// <remarks>
    /// Sessions are stored in-memory and identified by a <see cref="System.Guid"/>.
    /// </remarks>
    /// <example>
    /// <code>
    /// var id = BinanceSessionManager.CreateSession(options, logger);
    /// </code>
    /// </example>
    public static class BinanceSessionManagerCache
    {
        #region Fields

        private static readonly ConcurrentDictionary<System.Guid, BinanceSession> Sessions = new();

        #endregion Fields

        /// <summary>
        /// Creates a new Binance client session with the given options.
        /// </summary>
        /// <param name="options">Binance connection options.</param>
        /// <param name="logger">Logger used by the session.</param>
        /// <returns>The identifier of the created session.</returns>
        /// <example>
        /// <code>
        /// var id = BinanceSessionManager.CreateSession(options, logger);
        /// </code>
        /// </example>
        #region Methods (Public)

        public static System.Guid CreateSession(BinanceOptions options, ILogger logger)
        {
            var session = new BinanceSession(options, logger);
            Sessions[session.Id] = session;
            return session.Id;
        }

        /// <summary>
        /// Tries to get an existing session.
        /// </summary>
        /// <param name="id">Session identifier.</param>
        /// <param name="session">The retrieved session when found.</param>
        /// <returns><c>true</c> if the session exists.</returns>
        public static bool TryGetSession(System.Guid id, out BinanceSession? session)
            => Sessions.TryGetValue(id, out session);

        /// <summary>
        /// Removes a session from the manager.
        /// </summary>
        /// <param name="id">Identifier of the session to remove.</param>
        /// <returns><c>true</c> if the session was removed.</returns>
        public static bool RemoveSession(System.Guid id) => Sessions.TryRemove(id, out _);

        /// <summary>
        /// Returns the active session identifiers.
        /// </summary>
        /// <returns>A collection of session ids.</returns>
        public static IEnumerable<System.Guid> GetSessionIds() => Sessions.Keys;

        #endregion Methods (Public)
    }


}
