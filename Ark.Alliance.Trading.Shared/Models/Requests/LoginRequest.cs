using System.ComponentModel.DataAnnotations;

namespace Ark.Alliance.Trading.Shared.Models.Requests;

/// <summary>
/// Credentials for username/password authentication.
/// + Used for interactive sign-in.
/// - Not suitable for API key login.
/// Ref: <see href="https://datatracker.ietf.org/doc/html/rfc6749" />
/// </summary>
public class LoginRequest
{
    /// <summary>User login name.</summary>
    [Required]
    public string Username { get; set; } = string.Empty;

    /// <summary>Account password.</summary>
    [Required]
    public string Password { get; set; } = string.Empty;
}

