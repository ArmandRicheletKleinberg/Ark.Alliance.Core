using System.Collections.Concurrent;
using Ark;
using Ark.Api.Binance;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Ark.Api.Binance
{
    /// <summary>
    /// Represents a running Binance client session.
    /// + Holds state for API connectivity and cached data.
    /// - Not thread-safe for external mutations beyond provided dictionaries.
    /// </summary>
    public class BinanceSession
    {
        /// <summary>
        /// Identifier of the session.
        /// </summary>
        public System.Guid Id { get; }

        /// <summary>
        /// Options used to instantiate the client.
        /// </summary>
        public BinanceOptions Options { get; }

        /// <summary>
        /// Identifier of the Binance account owner.
        /// </summary>
        public string OwnerId { get; }

        /// <summary>
        /// The API client instance.
        /// </summary>
        public BinanceApiClient Client { get; }

        /// <summary>
        /// Stores the result of placed orders.
        /// </summary>
        public ConcurrentDictionary<long, Result<OrderResultDto>> Orders { get; } = new();

        /// <summary>
        /// Stores the latest known positions keyed by symbol.
        /// </summary>
        public ConcurrentDictionary<string, PositionDto> Positions { get; } = new();

        /// <summary>
        /// Available futures balances keyed by asset ticker.
        /// </summary>
        public ConcurrentDictionary<string, FuturesBalanceDto> FuturesBalances { get; } = new();
        /// <summary>
        /// UTC creation timestamp.
        /// </summary>
        public DateTime Created { get; } = DateTime.UtcNow;

        /// <summary>
        /// Initializes a new <see cref="BinanceSession"/> instance.
        /// </summary>
        public BinanceSession(BinanceOptions options, ILogger logger)
        {
            Id = System.Guid.NewGuid();
            Options = options;

            OwnerId = options.OwnerId;

            Client = new BinanceApiClient(Microsoft.Extensions.Options.Options.Create(options));

            logger.LogInformation("Binance session {Id} created", Id);
        }
    }

}
