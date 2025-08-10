
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Ark;
using Ark.App;
using Ark.App.Diagnostics;
using Ark.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Ark.Api.Binance
{
    /// <summary>
    /// Service that periodically fetches ticker information from Binance.
    /// + Supports both REST polling and WebSocket streaming.
    /// - Retains ticker history only in memory; data is lost on restart.
    /// </summary>
    /// <remarks>
    /// The service contacts Binance through <see cref="BinanceApiClientManager"/> and
    /// uses the configuration provided via <see cref="TickerFollowupSettings"/> to determine
    /// which symbols should be monitored.
    /// </remarks>
    /// <example>
    /// <code>
    /// var service = new TickerFollowupService(options);
    /// await service.ExecuteBlAsync(CancellationToken.None);
    /// </code>
    /// <output>Retrieves the latest ticker data for the configured symbols.</output>
    /// </example>
    public class TickerFollowupService : ScheduledHostedService<TickerFollowupSettings>
    {
        #region Fields

        private readonly IOptions<BinanceOptions> options;
        private static readonly ConcurrentDictionary<string, byte> ActiveSymbols = new();
        private static readonly ConcurrentDictionary<string, ConcurrentQueue<TickerDto>> History
            = new();

        #endregion Fields

        /// <summary>
        /// Initializes a new instance of the <see cref="TickerFollowupService"/> class.
        /// </summary>
        /// <param name="configuration">Service configuration provider.</param>
        /// <param name="options">Options used to configure the Binance client.</param>
        #region Constructors

        public TickerFollowupService(IConfiguration configuration, IOptions<BinanceOptions> options)
            : base(configuration)
        {
            var opts = options.Value;
            var path = Path.Combine(AppContext.BaseDirectory, "appconfig.json");
            if (File.Exists(path))
            {
                try
                {
                    var json = File.ReadAllText(path);
                    var cfg = JsonSerializer.Deserialize<BinanceOptions>(json);
                    if (cfg != null)
                    {
                        opts.ApiKey = cfg.ApiKey;
                        opts.ApiSecret = cfg.ApiSecret;
                    }
                }
                catch { }
            }

            this.options = Microsoft.Extensions.Options.Options.Create(opts);
            BinanceApiClientManager.Configure(this.options);
        }

        #endregion Constructors

        /// <summary>
        /// Gets the logger instance used by this service.
        /// </summary>
        #region Properties (Public)

        protected override ILogger Logger => Diag.Logs?.TickerFollowupService ?? NullLogger.Instance;

        #endregion Properties (Public)

        /// <summary>
        /// Retrieves the list of symbols currently tracked by the service.
        /// </summary>
        /// <returns>Distinct list of tickers.</returns>
        public IReadOnlyList<string> GetTickers() => Data.Tickers;

        /// <summary>
        /// Updates the set of symbols to monitor.
        /// + Clears previously active subscriptions.
        /// - Does not validate symbol availability on Binance.
        /// </summary>
        /// <param name="symbols">Symbols to track.</param>
        public void SetTickers(IEnumerable<string> symbols)
        {
            Data.Tickers = symbols.Distinct().ToList();
            ActiveSymbols.Clear();
        }

        /// <summary>
        /// Returns a copy of the accumulated ticker history for each tracked symbol.
        /// + Useful for diagnostics or backtesting.
        /// - Only in-memory data is available; no persistence is performed.
        /// </summary>
        /// <returns>A dictionary mapping each symbol to its recorded ticker values.</returns>
        public IReadOnlyDictionary<string, TickerDto[]> GetHistorySnapshot()
            => History.ToDictionary(kv => kv.Key, kv => kv.Value.ToArray());

        /// <inheritdoc />
        /// <summary>
        /// Retrieves ticker information from Binance for each configured symbol.
        /// </summary>
        /// <param name="cancellationToken">Token used to cancel the operation.</param>
        /// <returns>The result of the ticker retrieval.</returns>
        /// <example>
        /// <code>
        /// var result = await service.ExecuteBlAsync(CancellationToken.None);
        /// </code>
        /// <output>Returns <c>Result.Success</c> if all ticker calls succeed.</output>
        /// </example>
        #region Methods (Public)

        public async Task<Result> ExecuteBlAsync(CancellationToken cancellationToken)

        {
            foreach (var symbol in Data.Tickers.Distinct())
            {
                if (!ActiveSymbols.TryAdd(symbol, 0))
                    continue;

                Result res;
                if (Data.UseWebSocket)
                {
                    res = await BinanceApiClientManager.ExecuteAsync(
                        (c, t) => c.SubscribeTickerAsync(symbol, dto =>
                        {
                            var queue = History.GetOrAdd(symbol, _ => new ConcurrentQueue<TickerDto>());
                            queue.Enqueue(dto);
                        }, t),
                        "ticker",
                        cancellationToken);
                }
                else
                {
                    var typed = await BinanceApiClientManager.ExecuteAsync<object>(
                        (c, t) => c.GetTickerAsync(symbol, t),
                        "ticker",
                        cancellationToken);
                    if (typed.IsSuccess && typed.Data != null)
                    {
                        var json = JsonSerializer.Serialize(typed.Data);
                        using var doc = JsonDocument.Parse(json);
                        var price = doc.RootElement.TryGetProperty("lastPrice", out var lp)
                            ? lp.GetDecimal()
                            : doc.RootElement.GetProperty("price").GetDecimal();
                        var volume = doc.RootElement.TryGetProperty("volume", out var vol)
                            ? vol.GetDecimal()
                            : 0m;
                        var dto = new TickerDto { Symbol = symbol, Price = price, Volume = volume, Timestamp = DateTime.UtcNow };
                        var queue = History.GetOrAdd(symbol, _ => new ConcurrentQueue<TickerDto>());
                        queue.Enqueue(dto);
                    }
                    res = new Result(typed);
                }

                if (res.IsNotSuccess)
                {
                    Ark.App.Diagnostics.ILoggerExtensions.LogResult(Logger, res);
                    ActiveSymbols.TryRemove(symbol, out _);
                    return res;
                }
            }
            return Result.Success;
        }

        /// <inheritdoc />
        protected override Task Execute(CancellationToken cancellationToken)
            => ExecuteBlAsync(cancellationToken);

        #endregion Methods (Public)
    }
}
