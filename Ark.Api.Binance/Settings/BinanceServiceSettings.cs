using System.Text.Json.Serialization;
using Ark.App;
using Ark.Data;

namespace Ark.Api.Binance
{
    /// <summary>
    /// Settings for a Binance hosted service.
    /// + Aggregates ticker, order and position follow-up configurations.
    /// - Misaligned settings can lead to excessive API usage.
    /// </summary>
    public class BinanceServiceSettings : ScheduledHostedServiceSettings
    {
        /// <summary>
        /// Settings for ticker follow-up.
        /// </summary>
        [JsonPropertyName("TickerFollowupSettings")]
        public TickerFollowupSettings? TickerFollowupSettings { get; set; }

        /// <summary>
        /// Settings for order follow-up.
        /// </summary>
        [JsonPropertyName("OrderFollowupSettings")]
        public OrderFollowupSettings? OrderFollowupSettings { get; set; }

        /// <summary>
        /// Settings for position follow-up.
        /// </summary>
        [JsonPropertyName("PositionFollowupSettings")]
        public PositionFollowupSettings? PositionFollowupSettings { get; set; }
    }
}
