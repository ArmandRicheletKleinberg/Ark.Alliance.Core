using Ark.Api.Binance;
using Ark.Alliance.Trading.Shared.Models;

namespace Ark.Alliance.Trading.Shared.Models.Requests;

/// <summary>
/// Request payload used to start the trading bot.
/// + Bundles Binance credentials and trading parameters.
/// - Does not validate configuration coherence.
/// Ref: <see href="https://binance-docs.github.io/apidocs/futures/en/#change-log" />
/// </summary>
public class StartBotRequest
{
    /// <summary>
    /// Binance connection options.
    /// + Supplies API keys and environment info.
    /// - Secrets must be secured externally.
    /// </summary>
    public BinanceOptions BinanceOptions { get; set; } = new();

    /// <summary>
    /// Initial trading settings.
    /// + Defines symbol and strategy thresholds.
    /// - Subsequent changes require restart.
    /// </summary>
    public TradingSettings TradingSettings { get; set; } = new();
}
