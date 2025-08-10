namespace Ark.Api.Binance
{
    /// <summary>
    /// Helper methods for websocket usage.
    /// </summary>
    /// <example>
    /// <code>
    /// var url = WebSocketHelper.GetTickerEndpoint("BTCUSDT");
    /// </code>
    /// </example>
    public static class WebSocketHelper
    {
        #region Methods (Public)
        /// <summary>
        /// Formats a websocket endpoint for a ticker symbol.
        /// </summary>
        /// <param name="symbol">Trading symbol.</param>
        /// <returns>Endpoint string to use with the socket client.</returns>
        public static string GetTickerEndpoint(string symbol) => $"ws/{symbol.ToLower()}@kline_1m";

        #endregion Methods (Public)
    }
}
