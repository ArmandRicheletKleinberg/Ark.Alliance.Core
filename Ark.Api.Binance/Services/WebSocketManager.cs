using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Binance.Net.Clients;
using Binance.Net.Enums;
using Binance.Net.Interfaces;

namespace Ark.Api.Binance.Services
{
    /// <summary>
    /// Provides resilient WebSocket subscription management with automatic reconnection.
    /// + Handles heartbeat monitoring and exponential backoff.
    /// + Maintains subscription state across reconnects.
    /// - Connection pooling and subscription recovery are not implemented.
    /// Ref: <see href="https://github.com/JKorf/Binance.Net"/>
    /// </summary>
    public class WebSocketManager : IDisposable
    {
        #region Fields
        private readonly BinanceSocketClient _client;
        private readonly ILogger _logger;
        private readonly ConcurrentDictionary<string, SubscriptionInfo> _subscriptions = new();
        private readonly Timer _heartbeatTimer;
        private readonly CancellationTokenSource _cancellationTokenSource = new();

        private volatile bool _isDisposed = false;
        private DateTime _lastHeartbeat = DateTime.UtcNow;
        private readonly TimeSpan _heartbeatInterval = TimeSpan.FromSeconds(30);
        private readonly TimeSpan _connectionTimeout = TimeSpan.FromSeconds(10);
        #endregion Fields

        #region Events
        /// <summary>
        /// Fired when the connection state changes for any subscription.
        /// + Allows external monitoring of socket connectivity.
        /// - Event ordering is not guaranteed during reconnection storms.
        /// Ref: <see href="https://github.com/JKorf/Binance.Net"/>
        /// </summary>
        public event EventHandler<string>? ConnectionStatusChanged;

        /// <summary>
        /// Fired when an exception occurs during WebSocket processing.
        /// + Enables centralized error handling and logging.
        /// - Exceptions originate from background threads.
        /// Ref: <see href="https://github.com/JKorf/Binance.Net"/>
        /// </summary>
        public event EventHandler<Exception>? ErrorOccurred;
        #endregion Events

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="WebSocketManager"/> class.
        /// + Accepts an injected <see cref="BinanceSocketClient"/> for testing.
        /// + Starts heartbeat monitoring immediately.
        /// - Caller remains responsible for disposing the manager.
        /// Ref: <see href="https://github.com/JKorf/Binance.Net"/>
        /// </summary>
        public WebSocketManager(BinanceSocketClient client, ILogger? logger = null)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? NullLogger.Instance;

            _heartbeatTimer = new Timer(CheckHeartbeat, null, _heartbeatInterval, _heartbeatInterval);
        }
        #endregion Constructors

        #region Methods
        /// <summary>
        /// Subscribes to kline updates with automatic reconnection.
        /// + Performs exponential backoff on failures.
        /// - Returns <c>false</c> when cancellation is requested.
        /// Ref: <see href="https://github.com/JKorf/Binance.Net"/>
        /// </summary>
        /// <param name="symbol">Trading pair symbol.</param>
        /// <param name="onTick">Callback invoked with ticker data.</param>
        /// <param name="interval">Aggregation interval for klines.</param>
        /// <returns><c>true</c> when subscription is established.</returns>
        public async Task<bool> SubscribeTickerAsync(string symbol, Action<TickerDto> onTick,
            KlineInterval interval = KlineInterval.OneSecond)
        {
            if (_isDisposed) throw new ObjectDisposedException(nameof(WebSocketManager));

            var subscriptionKey = $"{symbol}_{interval}";
            var subscriptionInfo = new SubscriptionInfo
            {
                Symbol = symbol,
                Interval = interval,
                Callback = onTick,
                IsActive = true,
                LastUpdate = DateTime.UtcNow
            };

            _subscriptions.AddOrUpdate(subscriptionKey, subscriptionInfo, (k, v) => subscriptionInfo);

            return await ConnectWithRetry(subscriptionKey, subscriptionInfo);
        }

        /// <summary>
        /// Unsubscribes from ticker updates.
        /// + Releases underlying WebSocket resources.
        /// - Returns <c>false</c> when no subscription exists.
        /// Ref: <see href="https://github.com/JKorf/Binance.Net"/>
        /// </summary>
        /// <param name="symbol">Trading pair symbol.</param>
        /// <param name="interval">Aggregation interval for klines.</param>
        /// <returns><c>true</c> when subscription was removed.</returns>
        public async Task<bool> UnsubscribeTickerAsync(string symbol, KlineInterval interval = KlineInterval.OneSecond)
        {
            var subscriptionKey = $"{symbol}_{interval}";

            if (_subscriptions.TryRemove(subscriptionKey, out var subscription))
            {
                subscription.IsActive = false;

                if (subscription.SubscriptionId is int id)
                    await _client.UnsubscribeAsync(id);

                _logger.LogInformation("Unsubscribed from {Symbol} {Interval}", symbol, interval);
                ConnectionStatusChanged?.Invoke(this, $"Disconnected: {subscriptionKey}");
                return true;
            }

            return false;
        }

        private async Task<bool> ConnectWithRetry(string subscriptionKey, SubscriptionInfo info)
        {
            var maxRetries = 5;
            var baseDelay = TimeSpan.FromSeconds(1);

            for (int attempt = 0; attempt <= maxRetries; attempt++)
            {
                if (_cancellationTokenSource.Token.IsCancellationRequested || !info.IsActive)
                    return false;

                try
                {
                    var result = await _client.UsdFuturesApi.ExchangeData.SubscribeToKlineUpdatesAsync(
                        info.Symbol,
                        info.Interval,
                        data => HandleTickerUpdate(info, data.Data.Data),
                        false,
                        _cancellationTokenSource.Token);

                    if (result.Success)
                    {
                        info.SubscriptionId = result.Data.Id;
                        info.LastUpdate = DateTime.UtcNow;

                        _logger.LogInformation("Successfully subscribed to {Symbol} {Interval}",
                            info.Symbol, info.Interval);

                        ConnectionStatusChanged?.Invoke(this, $"Connected: {subscriptionKey}");
                        return true;
                    }
                    else
                    {
                        _logger.LogWarning("Failed to subscribe to {Symbol}: {Error}",
                            info.Symbol, result.Error?.Message);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Exception during subscription attempt {Attempt} for {Symbol}",
                        attempt + 1, info.Symbol);
                    ErrorOccurred?.Invoke(this, ex);
                }

                if (attempt < maxRetries)
                {
                    var delay = TimeSpan.FromMilliseconds(baseDelay.TotalMilliseconds * Math.Pow(2, attempt));
                    delay = TimeSpan.FromMilliseconds(Math.Min(delay.TotalMilliseconds, 30000)); // Max 30s

                    _logger.LogInformation("Retrying subscription in {Delay}ms (attempt {Attempt}/{MaxRetries})",
                        delay.TotalMilliseconds, attempt + 1, maxRetries);

                    await Task.Delay(delay, _cancellationTokenSource.Token);
                }
            }

            _logger.LogError("Failed to establish subscription for {Symbol} after {MaxRetries} attempts",
                info.Symbol, maxRetries);
            return false;
        }

        /// <summary>
        /// Converts incoming kline data to <see cref="TickerDto"/> and forwards it to subscribers.
        /// + Maps close price and time from Binance.Net models.
        /// - Assumes payload implements <see cref="IBinanceKline"/>.
        /// Ref: <see href="https://github.com/JKorf/Binance.Net"/>
        /// </summary>
        private void HandleTickerUpdate(SubscriptionInfo info, IBinanceKline kline)
        {
            try
            {
                info.LastUpdate = DateTime.UtcNow;
                _lastHeartbeat = DateTime.UtcNow;

                var ticker = new TickerDto
                {
                    Symbol = info.Symbol,
                    Price = kline.ClosePrice,
                    Volume = kline.Volume,
                    Timestamp = kline.CloseTime
                };

                info.Callback?.Invoke(ticker);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling ticker update for {Symbol}", info.Symbol);
                ErrorOccurred?.Invoke(this, ex);
            }
        }

        private void CheckHeartbeat(object? state)
        {
            if (_isDisposed) return;

            var timeSinceLastHeartbeat = DateTime.UtcNow - _lastHeartbeat;

            if (timeSinceLastHeartbeat > _heartbeatInterval.Add(TimeSpan.FromSeconds(10)))
            {
                _logger.LogWarning("Heartbeat timeout detected. Last heartbeat: {LastHeartbeat}", _lastHeartbeat);
                ConnectionStatusChanged?.Invoke(this, "Disconnected");

                // Attempt to reconnect all subscriptions
                _ = Task.Run(async () =>
                {
                    foreach (var kvp in _subscriptions)
                    {
                        if (kvp.Value.IsActive)
                        {
                            await ConnectWithRetry(kvp.Key, kvp.Value);
                        }
                    }
                });
            }
        }

        /// <summary>
        /// Releases managed resources and unsubscribes active streams.
        /// + Cancels heartbeat timer and outstanding operations.
        /// - Pending unsubscribe calls are not awaited.
        /// Ref: <see href="https://github.com/JKorf/Binance.Net"/>
        /// </summary>
        public void Dispose()
        {
            if (_isDisposed) return;
            _isDisposed = true;

            _cancellationTokenSource.Cancel();
            _heartbeatTimer?.Dispose();

            foreach (var subscription in _subscriptions.Values)
            {
                subscription.IsActive = false;

                if (subscription.SubscriptionId is int id)
                    _ = _client.UnsubscribeAsync(id);
            }

            _subscriptions.Clear();
            _cancellationTokenSource.Dispose();
        }
        #endregion Methods

        #region Nested Types
        /// <summary>
        /// Holds state for an individual WebSocket subscription.
        /// + Facilitates reconnection attempts and cleanup.
        /// - Exposed only internally.
        /// </summary>
        private class SubscriptionInfo
        {
            #region Properties
            /// <summary>
            /// Symbol being monitored.
            /// + Used as part of the subscription key.
            /// - Case sensitivity is not enforced.
            /// </summary>
            public string Symbol { get; set; } = string.Empty;

            /// <summary>
            /// Interval of kline updates.
            /// + Defines the aggregation period for ticks.
            /// - Only futures intervals are supported.
            /// </summary>
            public KlineInterval Interval { get; set; }

            /// <summary>
            /// Callback invoked on new ticker data.
            /// + Allows higher layers to process updates.
            /// - Executed on thread pool context.
            /// </summary>
            public Action<TickerDto>? Callback { get; set; }

            /// <summary>
            /// Indicates whether the subscription is active.
            /// + Checked before reconnection attempts.
            /// - Not thread safe for external modification.
            /// </summary>
            public bool IsActive { get; set; }

            /// <summary>
            /// Timestamp of the last received update.
            /// + Helps detect stale connections.
            /// - Drift possible if system clock changes.
            /// </summary>
            public DateTime LastUpdate { get; set; }

            /// <summary>
            /// Identifier returned by the Binance client.
            /// + Required for unsubscription.
            /// - Valid only for the lifetime of the connection.
            /// </summary>
            public int? SubscriptionId { get; set; }
            #endregion Properties
        }
        #endregion Nested Types
    }
}
