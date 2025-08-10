using System;

namespace Ark.Alliance.Trading.Shared.Models.Responses;

/// <summary>
/// Response returned after a successful login.
/// + Includes session token for subsequent requests.
/// - Token is temporary and non-JWT.
/// Ref: <see href="https://datatracker.ietf.org/doc/html/rfc7231#section-6.3.1" />
/// </summary>
public class LoginResponse
{
    /// <summary>Identifier of the authenticated user.</summary>
    public Guid UserId { get; set; }

    /// <summary>Authenticated username.</summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>Issued session token.</summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>Expiration timestamp of the session.</summary>
    public DateTime ExpiresAt { get; set; }
}

