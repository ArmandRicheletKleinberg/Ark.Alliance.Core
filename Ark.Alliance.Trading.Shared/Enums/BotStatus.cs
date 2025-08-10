namespace Ark.Alliance.Trading.Shared.Enums;

/// <summary>
/// Indicates the current lifecycle state of the trading bot.
/// + Allows UI components to reflect running status consistently.
/// - Does not convey internal health of sub-services.
/// Ref: <see href="https://learn.microsoft.com/dotnet/api/microsoft.extensions.hosting.backgroundservice" />
/// </summary>
public enum BotStatus
{
    /// <summary>Background service is not running.</summary>
    Stopped,
    /// <summary>Background service is executing the trading strategy.</summary>
    Running,
    /// <summary>Trading is temporarily paused due to external conditions.</summary>
    Paused
}
