using Ark.Api.Binance;

namespace Ark.Alliance.Trading.Shared.Models.Dtos;

/// <summary>
/// Basic information about a Binance session.
/// + Used to track server-side sessions from the UI.
/// - Does not expose security-sensitive details.
/// Ref: <see href="https://binance-docs.github.io/apidocs/futures/en/#user-data-streams" />
/// </summary>
public class SessionInfoDto
{
    /// <summary>
    /// Session identifier.
    /// + Correlates backend logs and user actions.
    /// - Unique only within the current database.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Creation timestamp of the session.
    /// + Helps compute session lifetime.
    /// - Uses server local time.
    /// </summary>
    public DateTime Created { get; set; }

    /// <summary>
    /// Identifier of the user owning the session.
    /// + Enables multi-user dashboards.
    /// - No role or permission information.
    /// </summary>
    public string OwnerId { get; set; } = string.Empty;

    /// <summary>
    /// Binance environment used for this session.
    /// + Distinguishes live vs testnet usage.
    /// - Enum may not include future environments.
    /// </summary>
    public BinanceEnvironment Environment { get; set; }
}
