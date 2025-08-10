namespace Ark.Alliance.Trading.Shared.Models.Requests;

/// <summary>
/// Optional parameters for stopping the trading bot.
/// + Allows graceful or immediate shutdown.
/// - Does not persist state for later resume.
/// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken" />
/// </summary>
public class StopBotRequest
{
    /// <summary>
    /// When true, open positions are liquidated at market.
    /// + Ensures no exposure after stop.
    /// - Market orders may incur slippage.
    /// </summary>
    public bool LiquidateMarket { get; set; } = true;
}
