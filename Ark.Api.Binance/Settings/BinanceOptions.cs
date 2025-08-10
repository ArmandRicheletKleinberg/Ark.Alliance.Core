using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;

namespace Ark.Api.Binance
{
    /// <summary>
    /// Global options for configuring the Binance API access.
    /// + Centralizes credentials, rate limits and service settings.
    /// - Misconfiguration may lead to authentication failures or bans.
    /// </summary>
    public class BinanceOptions : ILoggingOptions
    {
        /// <summary>
        /// API key for authenticated endpoints.
        /// </summary>
        [JsonPropertyName("ApiKey")]
        public string ApiKey { get; set; } = string.Empty;

        /// <summary>
        /// API secret for authenticated endpoints.
        /// </summary>
        [JsonPropertyName("ApiSecret")]
        public string ApiSecret { get; set; } = string.Empty;

        /// <summary>
        /// Base URL of the Binance Futures REST API.
        /// </summary>
        [JsonPropertyName("BaseUrl")]
        public string BaseUrl { get; set; } = "https://fapi.binance.com";

        /// <summary>
        /// Base URL of the Binance Futures Testnet.
        /// </summary>
        [JsonPropertyName("TestnetBaseUrl")]
        public string TestnetBaseUrl { get; set; } = "https://testnet.binancefuture.com";

        /// <summary>
        /// Maximum number of concurrent requests allowed by the host.
        /// </summary>
        [JsonPropertyName("MaxConcurrentRequests")]
        public int MaxConcurrentRequests { get; set; } = 5;

        /// <summary>
        /// Configured rate limits.
        /// </summary>
        [JsonPropertyName("Limits")]
        public Dictionary<string, LimitInfo> Limits { get; set; } = new();

        /// <summary>
        /// Gets or sets the environment used to connect to Binance.
        /// </summary>
        [JsonPropertyName("Environment")]
        public BinanceEnvironment Environment { get; set; } = BinanceEnvironment.Testnet;

        /// <summary>
        /// Specifies the minimal log level for Binance diagnostics.
        /// </summary>
        [JsonPropertyName("LogLevel")]
        public LogLevel LogLevel { get; set; } = LogLevel.Information;

        /// <summary>
        /// Number of retry attempts used by the resilience pipeline.
        /// </summary>
        [JsonPropertyName("RetryCount")]
        public int RetryCount { get; set; } = 3;

        /// <summary>
        /// Service specific settings.
        /// </summary>
        [JsonPropertyName("Services")]
        public Dictionary<string, BinanceServiceSettings> Services { get; set; } = new();

        /// <summary>
        /// Identifier of the Binance account owner.
        /// </summary>
        [JsonPropertyName("OwnerId")]
        public string OwnerId { get; set; } = string.Empty;
    }
}
