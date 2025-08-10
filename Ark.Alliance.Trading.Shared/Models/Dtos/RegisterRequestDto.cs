using System.ComponentModel.DataAnnotations;

namespace Ark.Alliance.Trading.Shared.Models.Dtos;

/// <summary>
/// Request payload for user registration.
/// + Used to create an initial profile and API key.
/// - Password is hashed with a demo algorithm.
/// Ref: <see href="https://datatracker.ietf.org/doc/html/rfc7231#section-4.3.3" />
/// </summary>
public class RegisterRequestDto
{
    /// <summary>Desired username.</summary>
    [Required]
    public string Username { get; set; } = string.Empty;

    /// <summary>User e-mail address.</summary>
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>Account password.</summary>
    [Required]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;
}

