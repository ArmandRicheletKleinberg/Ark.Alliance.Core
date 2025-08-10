using System;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Ark.Core.Api.TradingView.Models;
using Ark.Net.Http;
using Microsoft.Extensions.Logging;

#nullable enable

namespace Ark.Core.Api.TradingView
{
    /// <summary>
    /// Provides access to a subset of TradingView public APIs.
    /// + Wraps <see cref="HttpClient"/> calls with standardized <see cref="Result{T}"/> responses.
    /// - Covers only public, undocumented endpoints that may change without notice.
    /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.net.http.httpclient"/>
    /// </summary>
    public class TradingViewClient : HttpRepositoryBase
    {
        #region Fields

        private readonly HttpClient httpClient;
        private readonly JsonSerializerOptions jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters = { new JsonStringEnumConverter() }
        };

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="TradingViewClient"/>.
        /// + Configures JSON serialization and attaches the provided <see cref="HttpClient"/>.
        /// - Throws <see cref="ArgumentNullException"/> if <paramref name="client"/> is <see langword="null"/>.
        /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.net.http.httpclient"/>
        /// </summary>
        /// <param name="client">HTTP client used to perform requests.</param>
        /// <param name="logger">Optional logger for diagnostics.</param>
        public TradingViewClient(HttpClient client, ILogger<TradingViewClient>? logger = null)
        {
            this.httpClient = client ?? throw new ArgumentNullException(nameof(client));
            this.Logger = logger;
        }

        #endregion Constructors

        #region Properties (Protected)

        /// <summary>
        /// Gets or sets the logger instance.
        /// + Enables diagnostic logging of HTTP requests.
        /// - May be <see langword="null"/> when logging is not configured.
        /// Ref: <see href="https://learn.microsoft.com/dotnet/core/extensions/logging"/>
        /// </summary>
        protected ILogger<TradingViewClient>? Logger { get; set; }

        #endregion Properties (Protected)

        #region Methods (Override)

        /// <inheritdoc />
        protected override string RootUrl => string.Empty;

        /// <inheritdoc />
        protected override bool UseDefaultCredentials => false;

        #endregion Methods (Override)

        #region Methods (Public)

        /// <summary>
        /// Searches tickers by text query.
        /// + Calls TradingView symbol search endpoint.
        /// - Results are limited to publicly available symbols.
        /// Ref: <see href="https://www.tradingview.com/support/solutions/43000529348-symbol-search/"/>
        /// </summary>
        /// <param name="query">Text to search.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <param name="credentials">Optional <see cref="TradingViewCredentials"/> sent via basic authentication.</param>
        /// <returns>
        /// A list of matching <see cref="TickerInfo"/> objects.
        /// <code lang="json">[{"symbol":"BINANCE:BTCUSDT"}]</code>
        /// </returns>
        public async Task<Result<List<TickerInfo>>> GetTickersAsync(string query, CancellationToken cancellationToken = default, TradingViewCredentials? credentials = null)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return Result<List<TickerInfo>>.Failure.WithReason("Query is required.");
            }

            try
            {
                var requestUri = $"https://symbol-search.tradingview.com/symbol_search/?text={Uri.EscapeDataString(query)}&lang=en";
                using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                ApplyCredentials(request, credentials);
                using var response = await this.httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                {
                    return Result<List<TickerInfo>>.Failure.WithReason($"HTTP {response.StatusCode}");
                }

                await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
                var data = await JsonSerializer.DeserializeAsync<List<TickerInfo>>(stream, this.jsonOptions, cancellationToken).ConfigureAwait(false);
                return Result<List<TickerInfo>>.Success.WithData(data ?? []);
            }
            catch (Exception ex)
            {
                this.Logger?.LogError(ex, "TradingView tickers request failed");
                return Result<List<TickerInfo>>.Unexpected.WithException(ex);
            }
        }

        /// <summary>
        /// Retrieves a real time quote for a given symbol.
        /// + Uses TradingView's quote gateway to fetch last price data.
        /// - Requires valid symbol and may be delayed for free users.
        /// Ref: <see href="https://www.tradingview.com/support/solutions/43000529354-real-time-data/"/>
        /// </summary>
        /// <param name="symbol">Ticker symbol like BINANCE:BTCUSDT.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <param name="credentials">Optional <see cref="TradingViewCredentials"/> sent via basic authentication.</param>
        /// <returns>
        /// The current <see cref="Quote"/> with price and volume.
        /// <code lang="json">{"price":42000.0,"volume":0.5}</code>
        /// </returns>
        public async Task<Result<Quote>> GetRealTimeQuoteAsync(string symbol, CancellationToken cancellationToken = default, TradingViewCredentials? credentials = null)
        {
            if (string.IsNullOrWhiteSpace(symbol))
            {
                return Result<Quote>.Failure.WithReason("Symbol is required.");
            }

            try
            {
                var requestUri = "https://tvc4.tradingview.com/quotes-gw/quote";
                var body = JsonSerializer.Serialize(new { symbols = new[] { symbol } }, this.jsonOptions);
                using var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
                {
                    Content = new StringContent(body)
                };
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                ApplyCredentials(request, credentials);
                using var response = await this.httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                {
                    return Result<Quote>.Failure.WithReason($"HTTP {response.StatusCode}");
                }

                await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
                var json = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken).ConfigureAwait(false);
                if (!json.RootElement.TryGetProperty("d", out var data) || data.ValueKind != JsonValueKind.Array)
                {
                    return Result<Quote>.Failure.WithReason("Invalid response format");
                }

                var first = data[0];
                var quote = new Quote
                {
                    Price = first.GetProperty("lp").GetDecimal(),
                    Volume = first.TryGetProperty("volume", out var v) ? v.GetDecimal() : 0m,
                    Timestamp = DateTimeOffset.FromUnixTimeSeconds(first.GetProperty("timestamp").GetInt64()).UtcDateTime,
                    Currency = first.TryGetProperty("currency", out var c) ? c.GetString() ?? string.Empty : string.Empty,
                    Exchange = first.TryGetProperty("exchange", out var e) ? e.GetString() ?? string.Empty : string.Empty,
                    Underlying = GetUnderlyingSymbol(symbol),
                    IsFutures = IsFuturesSymbol(symbol)
                };

                return Result<Quote>.Success.WithData(quote);
            }
            catch (Exception ex)
            {
                this.Logger?.LogError(ex, "TradingView quote request failed");
                return Result<Quote>.Unexpected.WithException(ex);
            }
        }

        /// <summary>
        /// Retrieves historical candle data.
        /// + Provides OHLCV points for charting and analysis.
        /// - Large ranges may be limited by the service.
        /// Ref: <see href="https://www.tradingview.com/support/solutions/43000529350-history/"/>
        /// </summary>
        /// <param name="symbol">Ticker symbol.</param>
        /// <param name="start">Start date (UTC).</param>
        /// <param name="end">End date (UTC).</param>
        /// <param name="interval">Candle interval as <see cref="TradingViewInterval"/>.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <param name="credentials">Optional <see cref="TradingViewCredentials"/> sent via basic authentication.</param>
        /// <returns>
        /// Historical price and volume data as <see cref="QuoteHistory"/>.
        /// <code lang="json">{"symbol":"BTCUSDT","points":[{"open":1.0}]}</code>
        /// </returns>
        public async Task<Result<QuoteHistory>> GetHistoryAsync(string symbol, DateTime start, DateTime end, TradingViewInterval interval, CancellationToken cancellationToken = default, TradingViewCredentials? credentials = null)
        {
            if (string.IsNullOrWhiteSpace(symbol))
            {
                return Result<QuoteHistory>.Failure.WithReason("Symbol is required.");
            }

            var from = ((DateTimeOffset)start).ToUnixTimeSeconds();
            var to = ((DateTimeOffset)end).ToUnixTimeSeconds();
            var resolution = IntervalToResolution(interval);

            try
            {
                var requestUri = $"https://tvc4.tradingview.com/marks/history?symbol={Uri.EscapeDataString(symbol)}&from={from}&to={to}&resolution={resolution}";
                using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                using var response = await this.httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                {
                    return Result<QuoteHistory>.Failure.WithReason($"HTTP {response.StatusCode}");
                }

                await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
                var json = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken).ConfigureAwait(false);
                var history = new QuoteHistory
                {
                    Symbol = symbol,
                    Interval = interval,
                    Underlying = GetUnderlyingSymbol(symbol),
                    IsFutures = IsFuturesSymbol(symbol)
                };

                if (json.RootElement.TryGetProperty("t", out var times) &&
                    json.RootElement.TryGetProperty("o", out var opens) &&
                    json.RootElement.TryGetProperty("h", out var highs) &&
                    json.RootElement.TryGetProperty("l", out var lows) &&
                    json.RootElement.TryGetProperty("c", out var closes))
                {
                    for (int i = 0; i < times.GetArrayLength(); i++)
                    {
                        var point = new HistoryPoint
                        {
                            Timestamp = DateTimeOffset.FromUnixTimeSeconds(times[i].GetInt64()).UtcDateTime,
                            Open = opens[i].GetDecimal(),
                            High = highs[i].GetDecimal(),
                            Low = lows[i].GetDecimal(),
                            Close = closes[i].GetDecimal(),
                            Volume = json.RootElement.TryGetProperty("v", out var vols) && vols.ValueKind == JsonValueKind.Array && i < vols.GetArrayLength() ? vols[i].GetDecimal() : 0m,
                            OrderCount = json.RootElement.TryGetProperty("n", out var cnt) && cnt.ValueKind == JsonValueKind.Array && i < cnt.GetArrayLength() ? cnt[i].GetInt32() : 0
                        };
                        history.Points.Add(point);
                    }
                }

                return Result<QuoteHistory>.Success.WithData(history);
            }
            catch (Exception ex)
            {
                this.Logger?.LogError(ex, "TradingView history request failed");
                return Result<QuoteHistory>.Unexpected.WithException(ex);
            }
        }

        /// <summary>
        /// Retrieves a short technical analysis summary for the given symbol.
        /// + Calls TradingView's scanner API to obtain a consolidated recommendation.
        /// - Uses an undocumented endpoint that may change without notice.
        /// Ref: <see href="https://www.tradingview.com/support/"/>
        /// </summary>
        /// <param name="symbol">Ticker symbol.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <param name="credentials">Optional <see cref="TradingViewCredentials"/> sent via basic authentication.</param>
        /// <returns>A <see cref="Result{AnalysisSummary}"/> containing the recommendation or a failure reason.</returns>
        /// <remarks>
        /// The mapping of numeric scores to <see cref="AnalysisSummary.Recommendation"/> values is based on
        /// public TradingView behaviour and may need adjustments if the service changes. The endpoint does not
        /// provide a timestamp, therefore <see cref="DateTime.UtcNow"/> is used.
        /// </remarks>
        public async Task<Result<AnalysisSummary>> GetAnalysisSummaryAsync(string symbol, CancellationToken cancellationToken = default, TradingViewCredentials? credentials = null)
        {
            if (string.IsNullOrWhiteSpace(symbol))
            {
                return Result<AnalysisSummary>.Failure.WithReason("Symbol is required.");
            }

            try
            {
                var requestUri = "https://scanner.tradingview.com/crypto/scan";
                var payload = new
                {
                    symbols = new { tickers = new[] { symbol }, query = new { types = Array.Empty<string>() } },
                    columns = new[] { "Recommend.All" }
                };
                var body = JsonSerializer.Serialize(payload, this.jsonOptions);
                using var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
                {
                    Content = new StringContent(body, Encoding.UTF8, "application/json")
                };
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                ApplyCredentials(request, credentials);

                using var response = await this.httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                {
                    return Result<AnalysisSummary>.Failure.WithReason($"HTTP {response.StatusCode}");
                }

                await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
                var json = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken).ConfigureAwait(false);
                if (!json.RootElement.TryGetProperty("data", out var data) || data.ValueKind != JsonValueKind.Array || data.GetArrayLength() == 0)
                {
                    return Result<AnalysisSummary>.Failure.WithReason("Invalid response format");
                }

                var first = data[0];
                if (!first.TryGetProperty("d", out var arr) || arr.ValueKind != JsonValueKind.Array || arr.GetArrayLength() == 0)
                {
                    return Result<AnalysisSummary>.Failure.WithReason("Invalid response format");
                }

                var score = arr[0].GetDecimal();
                var recommendation = score switch
                {
                    >= 0.5m => "StrongBuy",
                    >= 0.1m => "Buy",
                    <= -0.5m => "StrongSell",
                    <= -0.1m => "Sell",
                    _ => "Neutral"
                };

                var summary = new AnalysisSummary
                {
                    Symbol = symbol,
                    Recommendation = recommendation,
                    Timestamp = DateTime.UtcNow
                };

                return Result<AnalysisSummary>.Success.WithData(summary);
            }
            catch (Exception ex)
            {
                this.Logger?.LogError(ex, "TradingView analysis request failed");
                return Result<AnalysisSummary>.Unexpected.WithException(ex);
            }
        }

        #endregion Methods (Public)

        #region Methods (Private)

        private static string IntervalToResolution(TradingViewInterval interval) => interval switch
        {
            TradingViewInterval.Second15 => "15S",
            TradingViewInterval.Second30 => "30S",
            TradingViewInterval.Minute1 => "1",
            TradingViewInterval.Minute5 => "5",
            TradingViewInterval.Minute15 => "15",
            TradingViewInterval.Hour1 => "60",
            TradingViewInterval.Hour4 => "240",
            TradingViewInterval.Day1 => "D",
            TradingViewInterval.Week1 => "W",
            TradingViewInterval.Month1 => "M",
            _ => "D"
        };

        private static void ApplyCredentials(HttpRequestMessage request, TradingViewCredentials? credentials)
        {
            if (credentials is null || string.IsNullOrWhiteSpace(credentials.Username))
                return;

            var raw = $"{credentials.Username}:{credentials.Password}";
            var token = Convert.ToBase64String(Encoding.UTF8.GetBytes(raw));
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", token);
        }

        private static bool IsFuturesSymbol(string symbol)
            => symbol.Contains("PERP", StringComparison.OrdinalIgnoreCase) || symbol.Contains("FUT", StringComparison.OrdinalIgnoreCase);

        private static string GetUnderlyingSymbol(string symbol)
        {
            var core = symbol.Contains(':') ? symbol.Split(':')[1] : symbol;
            core = core.Replace("PERP", string.Empty, StringComparison.OrdinalIgnoreCase);
            core = core.Replace("FUT", string.Empty, StringComparison.OrdinalIgnoreCase);
            return core;
        }

        #endregion Methods (Private)
    }
}
