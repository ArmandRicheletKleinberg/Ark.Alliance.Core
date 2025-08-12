namespace Ark.App.Secrets.Options;

/// <summary>
/// Aggregates API credentials for external providers.
/// + Enables strongly typed access to service secrets.
/// - Holds placeholder keys only; never commit real credentials.
/// </summary>
public class SecretsOptions
{
    #region Properties
    /// <summary>
    /// Credentials for Binance API access.
    /// + Used to authenticate trading requests.
    /// - Populate with "CHANGE_ME" during development.
    /// </summary>
    public ServiceSecretOptions Binance { get; init; } = new();

    /// <summary>
    /// Credentials for OpenAI API access.
    /// + Supports AI-driven features in the application.
    /// - Values must be supplied via secure configuration.
    /// </summary>
    public ServiceSecretOptions OpenAI { get; init; } = new();

    /// <summary>
    /// Credentials for Anthropic API access.
    /// + Allows integration with Anthropic models.
    /// - Replace placeholders before deployment.
    /// </summary>
    public ServiceSecretOptions Anthropic { get; init; } = new();

    /// <summary>
    /// Credentials for DeepSeek API access.
    /// + Enables DeepSeek feature calls.
    /// - Requires secure storage for production.
    /// </summary>
    public ServiceSecretOptions DeepSeek { get; init; } = new();
    #endregion Properties
}
