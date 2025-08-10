using System.Text.Json.Serialization;

namespace Ark.Api.Binance
{
    /// <summary>
    /// Settings controlling ticker monitoring.
    /// + Configures which symbols to track and how updates are received.
    /// - Incorrect configuration may miss market movements.
    /// </summary>
    public class TickerFollowupSettings
    {
        /// <summary>
        /// Symbols to monitor.
        /// </summary>
        [JsonPropertyName("Tickers")]
        public List<string> Tickers { get; set; } = new();

        /// <summary>
        /// Type of ticker.
        /// </summary>
        [JsonPropertyName("TickerType")]
        public string TickerType { get; set; } = "All";

        /// <summary>
        /// Only tickers currently trading.
        /// </summary>
        [JsonPropertyName("OnlyTradingTickerAvailable")]
        public bool OnlyTradingTickerAvailable { get; set; }

        /// <summary>
        /// Use web sockets instead of polling.
        /// </summary>
        [JsonPropertyName("UseWebSocket")]
        public bool UseWebSocket { get; set; }
    }
}
