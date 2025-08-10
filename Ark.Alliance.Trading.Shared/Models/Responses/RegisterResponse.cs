using System;

namespace Ark.Alliance.Trading.Shared.Models.Responses;

/// <summary>
/// Response returned after a successful user registration.
/// + Provides one-time API key to authenticate.
/// - API key is shown only once.
/// Ref: <see href="https://datatracker.ietf.org/doc/html/rfc7231#section-6.3.1" />
/// </summary>
public class RegisterResponse
{
    /// <summary>Identifier of the created user profile.</summary>
    public Guid UserId { get; set; }

    /// <summary>Registered username.</summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>One-time API key issued to the user.</summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>Informational message for the client.</summary>
    public string Message { get; set; } = string.Empty;
}

