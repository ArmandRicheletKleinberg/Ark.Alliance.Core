using System.ComponentModel.DataAnnotations;

namespace Ark.Alliance.Trading.Shared.Models.Requests;

/// <summary>
/// Authentication payload using a previously issued API key.
/// + Enables headless clients.
/// - API key must be kept secret.
/// Ref: <see href="https://owasp.org/www-community/controls/Key_Management" />
/// </summary>
public class ApiKeyLoginRequest
{
    /// <summary>Raw API key.</summary>
    [Required]
    public string ApiKey { get; set; } = string.Empty;
}

