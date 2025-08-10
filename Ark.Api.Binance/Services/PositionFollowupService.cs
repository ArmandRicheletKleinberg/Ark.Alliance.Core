using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Ark.App;
using Ark.Data;
using Binance.Net.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Ark.Api.Binance
{
    /// <summary>
    /// Periodically requests open position information from Binance.
    /// + Updates in-memory positions and available balances for each session.
    /// - Frequent polling may count toward account weight limits.
    /// </summary>
    /// <remarks>
    /// The service communicates with Binance via <see cref="BinanceApiClientManager"/> and
    /// is scheduled using settings defined in <see cref="PositionFollowupSettings"/>.
    /// </remarks>
    /// <example>
    /// <code>
    /// var service = new PositionFollowupService(options);
    /// await service.ExecuteBlAsync(CancellationToken.None);
    /// </code>
    /// <output>Logs the open positions when successful.</output>
    /// </example>
    public class PositionFollowupService : ScheduledHostedService<PositionFollowupSettings>
    {
        #region Fields

        private readonly IOptions<BinanceOptions> options;

        #endregion Fields

        /// <summary>
        /// Initializes a new instance of the <see cref="PositionFollowupService"/> class.
        /// </summary>
        /// <param name="configuration">Service configuration provider.</param>
        /// <param name="options">Options used to configure the Binance client.</param>
        #region Constructors

        public PositionFollowupService(IConfiguration configuration, IOptions<BinanceOptions> options)
            : base(configuration)
        {
            this.options = options;
            BinanceApiClientManager.Configure(options);
        }

        #endregion Constructors

        /// <summary>
        /// Gets the logger instance used by this service.
        /// </summary>
        #region Properties (Public)

        protected override ILogger Logger => Diag.Logs?.PositionFollowupService ?? NullLogger.Instance;

        #endregion Properties (Public)

        /// <inheritdoc />
        /// <summary>
        /// Retrieves the open positions from Binance.
        /// </summary>
        /// <param name="cancellationToken">Token used to cancel the operation.</param>
        /// <returns>The result of the Binance API call.</returns>
        /// <example>
        /// <code>
        /// var result = await service.ExecuteBlAsync(CancellationToken.None);
        /// </code>
        /// <output>Returns <c>Result.Success</c> when the call succeeds.</output>
        /// </example>
        #region Methods (Public)

        public async Task<Result> ExecuteBlAsync(CancellationToken cancellationToken)
        {
            foreach (var sessionId in BinanceSessionManagerCache.GetSessionIds())
            {
                if (!BinanceSessionManagerCache.TryGetSession(sessionId, out var session) || session is null || session.Client is null)
                    continue;

                var result = await session.Client.GetPositionsAsync(cancellationToken);
                if (result.IsNotSuccess || result.Data == null)
                {
                    Ark.App.Diagnostics.ILoggerExtensions.LogResult(Logger, result);
                    continue;
                }

                var json = JsonSerializer.Serialize(result.Data);
                using var doc = JsonDocument.Parse(json);
                var symbols = new HashSet<string>();

                foreach (var element in doc.RootElement.EnumerateArray())
                {
                    var dto = ToPositionDto(element);
                    if (dto == null)
                        continue;

                    symbols.Add(dto.Symbol);

                    if (dto.Quantity == 0)
                    {
                        if (session.Positions.TryRemove(dto.Symbol, out _))
                            Logger.LogInformation("Position {Symbol} removed for session {SessionId}", dto.Symbol, sessionId);
                        continue;
                    }

                    if (!session.Positions.TryGetValue(dto.Symbol, out var existing) || !PositionEquals(existing, dto))
                    {
                        session.Positions[dto.Symbol] = dto;
                        Logger.LogInformation("Position {Symbol} updated for session {SessionId}", dto.Symbol, sessionId);
                    }
                }

                foreach (var key in session.Positions.Keys.Where(k => !symbols.Contains(k)).ToList())
                {
                    session.Positions.TryRemove(key, out _);
                    Logger.LogInformation("Position {Symbol} removed for session {SessionId}", key, sessionId);
                }

                var balance = await session.Client.GetFuturesQuoteAvailableAsync("USDT", cancellationToken);
                if (balance.IsSuccess)
                {
                    session.FuturesBalances["USDT"] = new FuturesBalanceDto
                    {
                        Asset = "USDT",
                        Available = balance.Data,
                        Timestamp = DateTime.UtcNow
                    };
                }
                else
                {
                    Ark.App.Diagnostics.ILoggerExtensions.LogResult(Logger, balance);
                }

                // Retrieve USDC futures balance as well
                var usdcBalance = await session.Client.GetFuturesQuoteAvailableAsync("USDC", cancellationToken);
                if (usdcBalance.IsSuccess)
                {
                    session.FuturesBalances["USDC"] = new FuturesBalanceDto
                    {
                        Asset = "USDC",
                        Available = usdcBalance.Data,
                        Timestamp = DateTime.UtcNow
                    };
                }
                else
                {
                    Ark.App.Diagnostics.ILoggerExtensions.LogResult(Logger, usdcBalance);
                }
            }

            return Result.Success;
        }

        #endregion Methods (Public)

        #region Methods (Protected)

        /// <inheritdoc />
        protected override Task Execute(CancellationToken cancellationToken)
            => ExecuteBlAsync(cancellationToken);

        #endregion Methods (Protected)

        #region Methods (Private)

        private static bool PositionEquals(PositionDto a, PositionDto b)
            => a.Quantity == b.Quantity && a.EntryPrice == b.EntryPrice && a.MarkPrice == b.MarkPrice && a.UnrealizedPnl == b.UnrealizedPnl;

        private static decimal GetDecimal(JsonElement element)
            => element.ValueKind == JsonValueKind.Number ? element.GetDecimal() : decimal.TryParse(element.GetString(), out var v) ? v : 0m;

        private static PositionDto? ToPositionDto(JsonElement element)
        {
            try
            {
                var side = Enum.TryParse<PositionSide>(element.GetProperty("positionSide").GetString(), true, out var ps) ? ps : PositionSide.Both;

                var dto = new PositionDto
                {
                    Symbol = element.GetProperty("symbol").GetString() ?? string.Empty,
                    Side = side,
                    Quantity = GetDecimal(element.GetProperty("positionAmt")),
                    EntryPrice = GetDecimal(element.GetProperty("entryPrice")),
                    MarkPrice = GetDecimal(element.GetProperty("markPrice")),
                    UnrealizedPnl = GetDecimal(element.GetProperty("unRealizedProfit")),
                    Leverage = GetDecimal(element.GetProperty("leverage")),
                    Timestamp = element.TryGetProperty("updateTime", out var ut) ? DateTimeOffset.FromUnixTimeMilliseconds(ut.GetInt64()).UtcDateTime : DateTime.UtcNow
                };

                return dto;
            }
            catch
            {
                return null;
            }
        }

        #endregion Methods (Private)
    }
}
