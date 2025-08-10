using System;

namespace Ark.Alliance.Trading.Shared.Models.Responses;

/// <summary>
/// Result returned after attempting to start the trading bot.
/// + Provides session identifier on success.
/// - Does not include detailed failure diagnostics.
/// Ref: <see href="https://datatracker.ietf.org/doc/html/rfc7231#section-6" />
/// </summary>
public class StartBotResponse
{
    /// <summary>
    /// Identifier of the created session.
    /// + Use to query session status.
    /// - Null when <see cref="Success"/> is false.
    /// </summary>
    public Guid SessionId { get; set; }

    /// <summary>
    /// Indicates whether the start request succeeded.
    /// + Simplifies client branching logic.
    /// - True even if warnings occurred.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Error message when <see cref="Success"/> is false.
    /// + Helps troubleshoot configuration issues.
    /// - Not localised.
    /// </summary>
    public string? Error { get; set; }
}
