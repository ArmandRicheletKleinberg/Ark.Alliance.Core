
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Ark.App;
using Binance.Net.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Ark.Api.Binance
{
    /// <summary>
    /// Periodically queries Binance for open order information.
    /// + Keeps session order caches in sync with exchange state.
    /// - High frequency polling may increase API weight usage.
    /// </summary>
    /// <remarks>
    /// This hosted service relies on <see cref="BinanceApiClientManager"/> to access the Binance REST API
    /// and runs according to the configuration specified in <see cref="OrderFollowupSettings"/>.
    /// </remarks>
    /// <example>
    /// <code>
    /// var service = new OrderFollowupService(options);
    /// await service.ExecuteBlAsync(CancellationToken.None);
    /// </code>
    /// <output>Logs the list of open orders on success.</output>
    /// </example>
    public class OrderFollowupService : ScheduledHostedService<OrderFollowupSettings>
    {
        #region Fields

        private readonly IOptions<BinanceOptions> options;

        #endregion Fields

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderFollowupService"/> class.
        /// </summary>
        /// <param name="configuration">Service configuration provider.</param>
        /// <param name="options">Application options for the Binance client.</param>
        #region Constructors

        public OrderFollowupService(IConfiguration configuration, IOptions<BinanceOptions> options)
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

        protected override ILogger Logger => Diag.Logs?.OrderFollowupService ?? NullLogger.Instance;

        #endregion Properties (Public)

        /// <inheritdoc />
        /// <summary>
        /// Fetches the open orders from Binance.
        /// </summary>
        /// <param name="cancellationToken">Token used to cancel the operation.</param>
        /// <returns>The result of the Binance call.</returns>
        /// <example>
        /// <code>
        /// var result = await service.ExecuteBlAsync(CancellationToken.None);
        /// if (result.IsSuccess)
        /// {
        ///     // handle orders
        /// }
        /// </code>
        /// <output>Returns <c>Result.Success</c> when orders are successfully retrieved.</output>
        /// </example>
        #region Methods (Public)

        public async Task<Result> ExecuteBlAsync(CancellationToken cancellationToken)
        {
            foreach (var sessionId in BinanceSessionManagerCache.GetSessionIds())
            {

                if (!BinanceSessionManagerCache.TryGetSession(sessionId, out var session) || session is null || session.Client is null)
                    continue;

                var result = await session.Client.GetOpenOrdersAsync(null, cancellationToken);
                if (result.IsNotSuccess || result.Data == null)
                {
                    Ark.App.Diagnostics.ILoggerExtensions.LogResult(Logger, result);
                    continue;
                }

                var json = JsonSerializer.Serialize(result.Data);
                using var doc = JsonDocument.Parse(json);
                var current = new HashSet<long>();

                foreach (var element in doc.RootElement.EnumerateArray())
                {
                    var dto = ToOrderDto(element);
                    if (dto == null)
                        continue;

                    current.Add(dto.OrderId);

                    if (!session.Orders.TryGetValue(dto.OrderId, out var existing) ||
                        existing.Data == null || !OrderEquals(existing.Data, dto))
                    {
                        session.Orders[dto.OrderId] = Result<OrderResultDto>.Success.WithData(dto);
                        Logger.LogInformation("Order {OrderId} updated for session {SessionId}", dto.OrderId, sessionId);
                    }
                }

                foreach (var id in session.Orders.Keys.Where(k => !current.Contains(k)).ToList())
                {
                    session.Orders.TryRemove(id, out _);
                    Logger.LogInformation("Order {OrderId} removed for session {SessionId}", id, sessionId);
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

        private static bool OrderEquals(OrderResultDto a, OrderResultDto b)
            => a.Status == b.Status && a.Quantity == b.Quantity && a.Price == b.Price &&
               a.StopPrice == b.StopPrice && a.TimeInForce == b.TimeInForce && a.Type == b.Type;

        private static decimal GetDecimal(JsonElement element)
            => element.ValueKind == JsonValueKind.Number ? element.GetDecimal() : decimal.TryParse(element.GetString(), out var v) ? v : 0m;

        private static OrderResultDto? ToOrderDto(JsonElement element)
        {
            try
            {
                var side = Enum.Parse<OrderSide>(element.GetProperty("side").GetString()!, true);
                var type = Enum.Parse<FuturesOrderType>(element.GetProperty("type").GetString()!, true);
                var tif = Enum.TryParse<TimeInForce>(element.GetProperty("timeInForce").GetString(), true, out var tmpTif) ? tmpTif : TimeInForce.GoodTillCanceled;
                var posSide = Enum.TryParse<PositionSide>(element.GetProperty("positionSide").GetString(), true, out var ps) ? ps : PositionSide.Both;
                var status = Enum.Parse<OrderStatus>(element.GetProperty("status").GetString()!, true);

                var dto = new OrderResultDto
                {
                    OrderId = element.TryGetProperty("orderId", out var idEl) ? idEl.GetInt64() : element.GetProperty("id").GetInt64(),
                    Symbol = element.GetProperty("symbol").GetString() ?? string.Empty,
                    Side = side,
                    Type = type,
                    Quantity = GetDecimal(element.TryGetProperty("origQty", out var q) ? q : element.GetProperty("quantity")),
                    Price = GetDecimal(element.GetProperty("price")),
                    StopPrice = element.TryGetProperty("stopPrice", out var sp) ? GetDecimal(sp) : null,
                    TimeInForce = tif,
                    ReduceOnly = element.TryGetProperty("reduceOnly", out var ro) && ro.GetBoolean(),
                    PositionSide = posSide,
                    ClientOrderId = element.TryGetProperty("clientOrderId", out var coid) ? coid.GetString() ?? string.Empty : string.Empty,
                    Status = status,
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
