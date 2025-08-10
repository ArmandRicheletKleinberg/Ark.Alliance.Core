using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ark.AspNetCore;
using Ark.Core.Api.TradingView.Models;
using Ark.Net.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore;


namespace Ark.Core.Api.TradingView
{
    /// <summary>
    /// Controller exposing TradingView client operations.
    /// + Thin wrapper over <see cref="TradingViewClient"/> to expose HTTP endpoints.
    /// - Responses are not cached and depend on TradingView availability.
    /// Ref: <see href="https://learn.microsoft.com/aspnet/core/web-api/?view=aspnetcore-9.0"/>
    /// </summary>
    /// <typeparam name="TUserProfileData">Type of user profile data consumed by the base controller.</typeparam>
    [ApiController]
    [ApiExplorerSettings(GroupName = "ο TradingView")]
    [Produces("application/json")]
    public class TradingViewController<TUserProfileData> : ControllerBase<TUserProfileData>
        where TUserProfileData : new()
    {
        #region Fields

        private readonly TradingViewClient client;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the controller.
        /// + Obtains a <see cref="TradingViewClient"/> via dependency injection.
        /// - Throws <see cref="ArgumentNullException"/> if <paramref name="client"/> is <see langword="null"/>.
        /// Ref: <see href="https://learn.microsoft.com/aspnet/core/fundamentals/dependency-injection"/>
        /// </summary>
        /// <param name="client">Client used to communicate with TradingView.</param>
        public TradingViewController(TradingViewClient client)
        {
            this.client = client ?? throw new ArgumentNullException(nameof(client));
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Searches tickers using the TradingView public API.
        /// + Wraps the <see cref="TradingViewClient.GetTickersAsync(string, System.Threading.CancellationToken, TradingViewCredentials?)"/> call.
        /// - Limited by TradingView rate limits and public search scope.
        /// Ref: <see href="https://www.tradingview.com/support/solutions/43000529348-symbol-search/"/>
        /// </summary>
        /// <param name="query">Text to search. Example: <c>BTC</c>.</param>
        /// <returns>
        /// Matching tickers serialized as JSON.
        /// <code lang="json">[{"symbol":"BINANCE:BTCUSDT"}]</code>
        /// </returns>
        /// <response code="200">Returns a list of tickers.</response>
        /// <response code="400">If the query is empty.</response>
        [HttpGet("tradingview/tickers")]
        [SwaggerOperation(Summary = "Searches tickers using the TradingView public API.", Description = "Wraps the TradingView symbol search endpoint." )]
        [ProducesResponseType(typeof(ResultDto<List<TickerInfo>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public Task<ResultDto<List<TickerInfo>>> GetTickers(string query)
            => ExecuteBlAsync(() => client.GetTickersAsync(query));

        /// <summary>
        /// Retrieves a real time quote.
        /// + Provides last price and volume for the symbol.
        /// - Credentials are optional and use basic authentication.
        /// Ref: <see href="https://www.tradingview.com/support/solutions/43000529354-real-time-data/"/>
        /// </summary>
        /// <param name="symbol">Ticker symbol like <c>BINANCE:BTCUSDT</c>.</param>
        /// <param name="user">Optional username for TradingView.</param>
        /// <param name="pass">Optional password for TradingView.</param>
        /// <returns>
        /// The current quote serialized as JSON.
        /// <code lang="json">{"price":42000.0,"volume":0.5}</code>
        /// </returns>
        /// <response code="200">Returns the real time quote.</response>
        /// <response code="400">If the symbol is missing.</response>
        [HttpGet("tradingview/quote")]
        [SwaggerOperation(Summary = "Retrieves a real time quote.", Description = "Calls TradingView's quote gateway to fetch last price data." )]
        [ProducesResponseType(typeof(ResultDto<Quote>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public Task<ResultDto<Quote>> GetQuote(string symbol, string user = null, string pass = null)
            => ExecuteBlAsync(() => client.GetRealTimeQuoteAsync(symbol, default, new TradingViewCredentials { Username = user, Password = pass }));

        /// <summary>
        /// Retrieves historical candle data.
        /// + Useful for charting and backtesting scenarios.
        /// - Large date ranges may be truncated by the upstream service.
        /// Ref: <see href="https://www.tradingview.com/support/solutions/43000529350-history/"/>
        /// </summary>
        /// <param name="symbol">Ticker symbol.</param>
        /// <param name="start">Start date in UTC.</param>
        /// <param name="end">End date in UTC.</param>
        /// <param name="interval">Candle interval as <see cref="TradingViewInterval"/>.</param>
        /// <param name="user">Optional username for TradingView.</param>
        /// <param name="pass">Optional password for TradingView.</param>
        /// <returns>
        /// History points serialized as JSON.
        /// <code lang="json">{"symbol":"BTCUSDT","points":[{"open":1.0}]}</code>
        /// </returns>
        /// <response code="200">Returns historical data.</response>
        /// <response code="400">If parameters are invalid.</response>
        [HttpGet("tradingview/history")]
        [SwaggerOperation(Summary = "Retrieves historical candle data.", Description = "Provides OHLCV points for charting and analysis." )]
        [ProducesResponseType(typeof(ResultDto<QuoteHistory>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public Task<ResultDto<QuoteHistory>> GetHistory(string symbol, DateTime start, DateTime end, TradingViewInterval interval, string user = null, string pass = null)
            => ExecuteBlAsync(() => client.GetHistoryAsync(symbol, start, end, interval, default, new TradingViewCredentials { Username = user, Password = pass }));

        #endregion Methods
    }
}
