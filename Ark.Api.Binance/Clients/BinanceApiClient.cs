

using Ark;
using Ark.Api.Binance;
using Ark.Api.Binance.Services;
using Ark.App.Diagnostics;
using Binance.Net.Clients;
using Binance.Net.Enums;
using Binance.Net.Objects.Options;
using CryptoExchange.Net.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Text.Json;

namespace Ark.Api.Binance
{
    /// <summary>
    /// Wrapper around <see cref="BinanceRestClient"/> and <see cref="BinanceSocketClient"/> to expose common operations.
    /// + Provides convenience methods with integrated logging and error handling.
    /// - Mirrors Binance endpoints closely; breaking API changes propagate here.
    /// </summary>
    /// <remarks>
    /// This client exposes higher level methods used by <see cref="BinanceApiClientManager"/> and
    /// <see cref="BinanceSession"/> to communicate with the Binance REST and WebSocket APIs.
    /// </remarks>
    /// <example>
    /// <code>
    /// var client = new BinanceApiClient(options);
    /// var result = await client.GetTickerAsync("BTCUSDT", CancellationToken.None);
    /// </code>
    /// </example>
    
      
public class BinanceApiClient
    {

        private readonly LatencyManagementService _latencyService = default!;

        // Dans le constructeur, ajouter:
        // _latencyService = latencyService;

        // Wrapper pour mesurer la latence des appels API
        private async Task<Result<T>> ExecuteWithLatencyMeasurement<T>(
            string endpoint,
            Func<Task<Result<T>>> operation)
        {
            using var tracker = _latencyService.StartLatencyMeasurement(endpoint, "REST");

            try
            {
                var result = await operation();
                await tracker.CompleteAsync(DateTime.UtcNow, JsonSerializer.Serialize(new { Success = result.IsSuccess }));
                return result;
            }
            catch (Exception ex)
            {
                await tracker.CompleteWithErrorAsync(ex.GetType().Name);
                throw;
            }
        }

        #region Fields

        private readonly BinanceRestClient restClient;
        private readonly BinanceSocketClient socketClient;
        private readonly ILogger _logger = Diag.Logs?.BinanceClient ?? NullLogger.Instance;

        #endregion Fields

        /// <summary>
        /// Initializes a new instance of the <see cref="BinanceApiClient"/> class.
        /// </summary>
        /// <param name="options">Configuration options for Binance connectivity.</param>
        /// <remarks>
        /// The API and socket clients are created with the provided credentials.
        /// </remarks>
        #region Constructors

        public BinanceApiClient(IOptions<BinanceOptions> options)
        {
            var opts = options.Value;
            var credentials = string.IsNullOrEmpty(opts.ApiKey) ? null : new ApiCredentials(opts.ApiKey, opts.ApiSecret);
            this.restClient = new BinanceRestClient(o =>
            {
                o.ApiCredentials = credentials;
            });

            this.socketClient = new BinanceSocketClient(o =>
            {
                o.ApiCredentials = credentials;
            });
        }

        #endregion Constructors

        #region Methods (Public)

        /// <summary>
        /// Subscribe to kline updates for a symbol.
        /// </summary>
        /// <param name="symbol">Trading symbol.</param>
        /// <param name="onTick">Callback invoked for each ticker update.</param>
        /// <param name="token">Token used to cancel the subscription.</param>
        /// <param name="interval">The kline update interval. Defaults to <see cref="KlineInterval.OneSecond"/>.</param>
        /// <returns>A <see cref="Result"/> indicating success or failure.</returns>
        /// <remarks>
        /// The method connects to the Binance websocket API and listens for updates.
        /// </remarks>
        /// <example>
        /// <code>
        /// await client.SubscribeTickerAsync("BTCUSDT", CancellationToken.None);
        /// </code>
        /// </example>
        public async Task<Result> SubscribeTickerAsync(string symbol, Action<TickerDto> onTick, CancellationToken token, KlineInterval interval = KlineInterval.OneSecond)
        {
            var result = await Result.SafeExecute(async () =>
            {
                await this.socketClient.UsdFuturesApi.ExchangeData.SubscribeToKlineUpdatesAsync(symbol, interval, e =>
                {
                    var dto = new TickerDto
                    {
                        Symbol = symbol,
                        Price = e.Data.Data.ClosePrice,
                        Volume = e.Data.Data.Volume,
                        Timestamp = e.Data.Data.CloseTime
                    };
                    onTick(dto);
                }, false, token);
                return Result.Success;
            }, ex => _logger.LogError(ex, "WebSocket subscribe failed"));

            if (result.IsNotSuccess)
                _logger.LogResult(result);

            return result;
        }

        /// <summary>
        /// Gets the current ticker price for a symbol.
        /// </summary>
        /// <param name="symbol">Trading symbol.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>The ticker price wrapped in a <see cref="Result{T}"/>.</returns>
        /// <remarks>
        /// The returned object is the raw response from Binance.
        /// </remarks>
        /// <example>
        /// <code>
        /// var ticker = await client.GetTickerAsync("BTCUSDT", CancellationToken.None);
        /// </code>
        /// </example>
        public async Task<Result<object>> GetTickerAsync(string symbol, CancellationToken token)
        {
            var result = await Result<object>.SafeExecute(async () =>
            {
                var res = await this.restClient.UsdFuturesApi.ExchangeData.GetTickerAsync(symbol, token);
                if (!res.Success)
                    return Result<object>.Failure.WithReason(res.Error?.Message ?? "Ticker failed");

                return Result<object>.Success.WithData(res.Data);
            });

            if (result.IsNotSuccess)
                _logger.LogResult(result);

            return result;
        }

        /// <summary>
        /// Gets open orders for the current account.
        /// </summary>
        /// <param name="symbol">Optional trading symbol to filter orders.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>A <see cref="Result{T}"/> containing the orders.</returns>
        /// <remarks>
        /// When no symbol is provided Binance returns all open orders for the account.
        /// </remarks>
        /// <example>
        /// <code>
        /// var orders = await client.GetOpenOrdersAsync("BTCUSDT", CancellationToken.None);
        /// </code>
        /// </example>
        public async Task<Result<object>> GetOpenOrdersAsync(string? symbol = null, CancellationToken token = default)
        {
            var result = await Result<object>.SafeExecute(async () =>
            {
                var res = await this.restClient.UsdFuturesApi.Trading.GetOpenOrdersAsync(symbol, null, token);
                if (!res.Success)
                    return Result<object>.Failure.WithReason(res.Error?.Message ?? "OpenOrders failed");

                return Result<object>.Success.WithData(res.Data);
            });

            if (result.IsNotSuccess)
                _logger.LogResult(result);

            return result;
        }

        /// <summary>
        /// Returns available account balances.
        /// </summary>
        /// <param name="token">Cancellation token.</param>
        /// <returns>The account balances.</returns>
        /// <remarks>
        /// The result contains the raw Binance balance objects.
        /// </remarks>
        /// <example>
        /// <code>
        /// var balances = await client.GetBalancesAsync();
        /// </code>
        /// </example>
        public async Task<Result<object>> GetBalancesAsync(CancellationToken token = default)
        {
            var result = await Result<object>.SafeExecute(async () =>
            {
                var res = await this.restClient.UsdFuturesApi.Account.GetBalancesAsync(null, token);
                if (!res.Success)
                    return Result<object>.Failure.WithReason(res.Error?.Message ?? "Balances failed");

                return Result<object>.Success.WithData(res.Data);
            });

            if (result.IsNotSuccess)
                _logger.LogResult(result);

            return result;
        }

        /// <summary>
        /// Returns current open positions.
        /// </summary>
        /// <param name="token">Cancellation token.</param>
        /// <returns>The list of positions.</returns>
        /// <remarks>
        /// This call queries the futures trading API.
        /// </remarks>
        /// <example>
        /// <code>
        /// var positions = await client.GetPositionsAsync();
        /// </code>
        /// </example>
        public async Task<Result<object>> GetPositionsAsync(CancellationToken token = default)
        {
            var result = await Result<object>.SafeExecute(async () =>
            {
                var res = await this.restClient.UsdFuturesApi.Trading.GetPositionsAsync(null, null, token);
                if (!res.Success)
                    return Result<object>.Failure.WithReason(res.Error?.Message ?? "Positions failed");

                return Result<object>.Success.WithData(res.Data);
            });

            if (result.IsNotSuccess)
                _logger.LogResult(result);

            return result;
        }

        /// <summary>
        /// Returns trade history for a symbol.
        /// </summary>
        /// <param name="symbol">Trading symbol.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>List of trades for the given symbol.</returns>
        /// <example>
        /// <code>
        /// var trades = await client.GetTradeHistoryAsync("BTCUSDT");
        /// </code>
        /// </example>
        public async Task<Result<object>> GetTradeHistoryAsync(string symbol, CancellationToken token = default)
        {
            var result = await Result<object>.SafeExecute(async () =>
            {
                var res = await this.restClient.UsdFuturesApi.Trading.GetUserTradesAsync(symbol, null, null, null, null, null, null, token);
                if (!res.Success)
                    return Result<object>.Failure.WithReason(res.Error?.Message ?? "Trades failed");

                return Result<object>.Success.WithData(res.Data);
            });

            if (result.IsNotSuccess)
                _logger.LogResult(result);

            return result;
        }

        /// <summary>
        /// Returns futures income history.
        /// </summary>
        /// <param name="symbol">Optional trading symbol.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>The income events recorded by Binance.</returns>
        /// <example>
        /// <code>
        /// var income = await client.GetIncomeHistoryAsync();
        /// </code>
        /// </example>
        public async Task<Result<object>> GetIncomeHistoryAsync(string? symbol = null, CancellationToken token = default)
        {
            var result = await Result<object>.SafeExecute(async () =>
            {
                var res = await this.restClient.UsdFuturesApi.Account.GetIncomeHistoryAsync(symbol, null, null, null, null, null, null, token);
                if (!res.Success)
                    return Result<object>.Failure.WithReason(res.Error?.Message ?? "Income failed");

                return Result<object>.Success.WithData(res.Data);
            });

            if (result.IsNotSuccess)
                _logger.LogResult(result);

            return result;
        }

        /// <summary>
        /// Returns the exchange info containing rate limits.
        /// </summary>
        /// <param name="token">Cancellation token.</param>
        /// <returns>Exchange information provided by Binance.</returns>
        /// <example>
        /// <code>
        /// var info = await client.GetExchangeInfoAsync();
        /// </code>
        /// </example>
        public async Task<Result<object>> GetExchangeInfoAsync(CancellationToken token = default)
        {
            var result = await Result<object>.SafeExecute(async () =>
            {
                var res = await this.restClient.UsdFuturesApi.ExchangeData.GetExchangeInfoAsync(token);
                if (!res.Success)
                    return Result<object>.Failure.WithReason(res.Error?.Message ?? "Exchange info failed");

                return Result<object>.Success.WithData(res.Data);
            });

            if (result.IsNotSuccess)
                _logger.LogResult(result);

            return result;
        }

        /// <summary>
        /// Gets the maximum initial leverage for a symbol.
        /// </summary>
        /// <param name="symbol">Trading symbol.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>The maximum leverage or <c>null</c> if unavailable.</returns>
        /// <example>
        /// <code>
        /// var leverage = await client.GetMaxLeverageAsync("BTCUSDT");
        /// </code>
        /// </example>
        public async Task<Result<int?>> GetMaxLeverageAsync(string symbol, CancellationToken token = default)
        {
            var result = await Result<int?>.SafeExecute(async () =>
            {
                var brackets = await this.restClient.UsdFuturesApi.Account.GetBracketsAsync(symbol, null, token);
                var leverage = brackets.Data?.FirstOrDefault()?.Brackets.MaxBy(b => b.InitialLeverage)?.InitialLeverage;
                return Result<int?>.Success.WithData(leverage);
            });

            if (result.IsNotSuccess)
                _logger.LogResult(result);

            return result;
        }

        /// <summary>
        /// Gets the quantity precision allowed for a futures symbol.
        /// </summary>
        /// <param name="symbol">Trading symbol.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>Number of decimal places permitted or <c>null</c> if unknown.</returns>
        public async Task<Result<int?>> GetQuantityPrecisionAsync(string symbol, CancellationToken token = default)
        {
            var result = await Result<int?>.SafeExecute(async () =>
            {
                var info = await this.restClient.UsdFuturesApi.ExchangeData.GetExchangeInfoAsync(token);
                if (!info.Success)
                    return Result<int?>.Failure.WithReason(info.Error?.Message ?? "Exchange info failed");

                var sym = info.Data.Symbols.FirstOrDefault(s => s.Name.Equals(symbol, StringComparison.OrdinalIgnoreCase));
                if (sym == null)
                    return Result<int?>.Failure.WithReason("Symbol not found");

                return Result<int?>.Success.WithData(sym.QuantityPrecision);
            });

            if (result.IsNotSuccess)
                _logger.LogResult(result);

            return result;
        }

        /// <summary>
        /// Changes the initial leverage for a symbol.
        /// </summary>
        /// <param name="symbol">Trading symbol.</param>
        /// <param name="leverage">Leverage to apply.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>A result describing the outcome.</returns>
        public async Task<Result> ChangeInitialLeverageAsync(string symbol, int leverage, CancellationToken token = default)
        {
            var result = await Result.SafeExecute(async () =>
            {
                var res = await this.restClient.UsdFuturesApi.Account.ChangeInitialLeverageAsync(symbol, leverage, ct: token);
                if (!res.Success)
                    return Result.Failure.WithReason(res.Error?.Message ?? "Change leverage failed");

                return Result.Success;
            }, ex => _logger.LogError(ex, "ChangeInitialLeverage failed"));

            if (result.IsNotSuccess)
                _logger.LogResult(result);

            return result;
        }

        /// <summary>
        /// Places a new futures order.
        /// </summary>
        /// <param name="order">Order parameters.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>Information about the created order.</returns>
        /// <example>
        /// <code>
        /// var result = await client.PlaceOrderAsync(order);
        /// </code>
        /// </example>
        public async Task<Result<OrderResultDto>> PlaceOrderAsync(FuturesOrder order, CancellationToken token = default)
        {
            var result = await Result<OrderResultDto>.SafeExecute(async () =>
            {
                var res = await this.restClient.UsdFuturesApi.Trading.PlaceOrderAsync(
                    order.Symbol,
                    order.Side,
                    order.Type,
                    quantity: order.Quantity,
                    price: order.Price,
                    timeInForce: order.TimeInForce,
                    stopPrice: order.StopPrice,
                    reduceOnly: order.ReduceOnly,
                    positionSide: order.PositionSide,
                    newClientOrderId: string.IsNullOrWhiteSpace(order.ClientOrderId) ? null : order.ClientOrderId,
                    ct: token);

                if (!res.Success)
                    return Result<OrderResultDto>.Failure.WithReason(res.Error?.Message ?? "Order failed");

                var data = new OrderResultDto
                {
                    Symbol = order.Symbol,
                    Side = order.Side,
                    Type = order.Type,
                    Quantity = order.Quantity,
                    Price = order.Price,
                    StopPrice = order.StopPrice,
                    TimeInForce = order.TimeInForce,
                    ReduceOnly = order.ReduceOnly,
                    PositionSide = order.PositionSide,
                    ClientOrderId = order.ClientOrderId,
                    OrderId = res.Data.Id,
                    Timestamp = res.Data.UpdateTime,
                    Status = res.Data.Status
                };
                return Result<OrderResultDto>.Success.WithData(data);
            });

            if (result.IsNotSuccess)
                _logger.LogResult(result);

            return result;
        }

        /// <summary>
        /// Cancels an existing order.
        /// </summary>
        /// <param name="symbol">Trading symbol.</param>
        /// <param name="orderId">Identifier assigned by Binance.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>A <see cref="Result"/> describing the outcome.</returns>
        /// <example>
        /// <code>
        /// await client.CancelOrderAsync("BTCUSDT", 1);
        /// </code>
        /// </example>
        public async Task<Result> CancelOrderAsync(string symbol, long orderId, CancellationToken token = default)
        {
            var result = await Result.SafeExecute(async () =>
            {
                await this.restClient.UsdFuturesApi.Trading.CancelOrderAsync(symbol, orderId, null, null, token);
                return Result.Success;
            }, ex => _logger.LogError(ex, "CancelOrder failed"));

            if (result.IsNotSuccess)
                _logger.LogResult(result);

            return result;
        }

        /// <summary>
        /// Modifies an order by cancelling and creating a new one.
        /// </summary>
        /// <param name="orderId">Identifier of the order to replace.</param>
        /// <param name="newOrder">New order parameters.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>The result of the newly placed order.</returns>
        /// <example>
        /// <code>
        /// var updated = await client.ModifyOrderAsync(id, order);
        /// </code>
        /// </example>
        public async Task<Result<OrderResultDto>> ModifyOrderAsync(long orderId, FuturesOrder newOrder, CancellationToken token = default)
        {
            var cancel = await CancelOrderAsync(newOrder.Symbol, orderId, token);
            if (cancel.IsNotSuccess)
            {
                _logger.LogResult(cancel);
                return Result<OrderResultDto>.Failure.WithReason(cancel.Reason);
            }

            var result = await PlaceOrderAsync(newOrder, token);
            if (result.IsNotSuccess)
                _logger.LogResult(result);

            return result;
        }

        #region Wallet Operations

        /// <summary>
        /// Gets the available balance for a futures quote asset.
        /// </summary>
        /// <param name="asset">Quote asset ticker (USDT or USDC).</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>The available amount for trading.</returns>
        /// <example>
        /// <code>
        /// var amount = await client.GetFuturesQuoteAvailableAsync("USDT");
        /// </code>
        /// </example>
        public async Task<Result<decimal>> GetFuturesQuoteAvailableAsync(string asset = "USDT", CancellationToken token = default)
        {
            var result = await Result<decimal>.SafeExecute(async () =>
            {
                var res = await this.restClient.UsdFuturesApi.Account.GetBalancesAsync(null, token);
                if (!res.Success)
                    return Result<decimal>.Failure.WithReason(res.Error?.Message ?? "Futures balance failed");

                var balance = res.Data?.FirstOrDefault(b => string.Equals(b.Asset, asset, StringComparison.OrdinalIgnoreCase))?.AvailableBalance ?? 0m;
                return Result<decimal>.Success.WithData(balance);
            });

            if (result.IsNotSuccess)
                _logger.LogResult(result);

            return result;
        }

        /// <summary>
        /// Gets the assets available in the funding wallet.
        /// </summary>
        /// <param name="token">Cancellation token.</param>
        /// <returns>The raw funding wallet assets.</returns>
        /// <example>
        /// <code>
        /// var assets = await client.GetFundingAssetsAsync();
        /// </code>
        /// </example>
        public async Task<Result<object>> GetFundingAssetsAsync(CancellationToken token = default)
        {
            var result = await Result<object>.SafeExecute(async () =>
            {
                var res = await this.restClient.SpotApi.Account.GetFundingWalletAsync(null, null, ct: token);
                if (!res.Success)
                    return Result<object>.Failure.WithReason(res.Error?.Message ?? "Funding wallet failed");

                return Result<object>.Success.WithData(res.Data);
            });

            if (result.IsNotSuccess)
                _logger.LogResult(result);

            return result;
        }

        /// <summary>
        /// Transfers funds from the futures wallet to the funding wallet.
        /// </summary>
        /// <param name="asset">Asset ticker.</param>
        /// <param name="quantity">Quantity to transfer.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>A <see cref="Result"/> describing the transfer.</returns>
        /// <example>
        /// <code>
        /// await client.TransferFuturesToFundingAsync("USDT", 10m);
        /// </code>
        /// </example>
        public async Task<Result> TransferFuturesToFundingAsync(string asset, decimal quantity, CancellationToken token = default)
        {
            var result = await Result.SafeExecute(async () =>
            {
                var res = await this.restClient.SpotApi.Account.TransferAsync(UniversalTransferType.UsdFuturesToFunding, asset, quantity, null, null, null, token);
                if (!res.Success)
                    return Result.Failure.WithReason(res.Error?.Message ?? "Transfer failed");

                return Result.Success;
            }, ex => _logger.LogError(ex, "TransferFuturesToFunding failed"));

            if (result.IsNotSuccess)
                _logger.LogResult(result);

            return result;
        }

        /// <summary>
        /// Transfers funds from the funding wallet to the futures wallet.
        /// </summary>
        /// <param name="asset">Asset ticker.</param>
        /// <param name="quantity">Quantity to transfer.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>A <see cref="Result"/> describing the transfer.</returns>
        /// <example>
        /// <code>
        /// await client.TransferFundingToFuturesAsync("USDT", 10m);
        /// </code>
        /// </example>
        public async Task<Result> TransferFundingToFuturesAsync(string asset, decimal quantity, CancellationToken token = default)
        {
            var result = await Result.SafeExecute(async () =>
            {
                var res = await this.restClient.SpotApi.Account.TransferAsync(UniversalTransferType.FundingToUsdFutures, asset, quantity, null, null, null, token);
                if (!res.Success)
                    return Result.Failure.WithReason(res.Error?.Message ?? "Transfer failed");

                return Result.Success;
            }, ex => _logger.LogError(ex, "TransferFundingToFutures failed"));

            if (result.IsNotSuccess)
                _logger.LogResult(result);

            return result;
        }

        #endregion Wallet Operations

        #endregion Methods (Public)
    }
}
