namespace Ark.App.Secrets.Options;

/// <summary>
/// + Represents API credentials for an external service.
/// - Does not include validation or rotation logic.
/// </summary>
public class ServiceSecretOptions
{
    /// <summary>
    /// + API key used to authenticate requests.
    /// - Store only placeholder values such as "CHANGE_ME" in configuration.
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// + API secret paired with the API key.
    /// - Keep secrets out of source control.
    /// </summary>
    public string ApiSecret { get; set; } = string.Empty;
}
