namespace Ark.App.Secrets.Options;

/// <summary>
/// Represents API credentials for an external service.
/// + Provides simple options binding for service authentication.
/// - Lacks validation and rotation logic by design.
/// </summary>
public class ServiceSecretOptions
{
    #region Properties
    /// <summary>
    /// API key used to authenticate requests.
    /// + Often paired with an API secret for HMAC signatures.
    /// - Use "CHANGE_ME" placeholders in committed configuration files.
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// API secret paired with the API key.
    /// + Required for signing authorized requests.
    /// - Keep secrets out of source control at all times.
    /// </summary>
    public string ApiSecret { get; set; } = string.Empty;
    #endregion Properties
}
