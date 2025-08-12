namespace Ark.App.Secrets.Options;

/// <summary>
/// + Aggregates API credentials for external providers.
/// - Intended for configuration binding only.
/// </summary>
public class SecretsOptions
{
    /// <summary>
    /// + Credentials for Binance API access.
    /// - Do not commit real keys.
    /// </summary>
    public ServiceSecretOptions Binance { get; init; } = new();

    /// <summary>
    /// + Credentials for OpenAI API access.
    /// - Do not commit real keys.
    /// </summary>
    public ServiceSecretOptions OpenAI { get; init; } = new();

    /// <summary>
    /// + Credentials for Anthropic API access.
    /// - Do not commit real keys.
    /// </summary>
    public ServiceSecretOptions Anthropic { get; init; } = new();

    /// <summary>
    /// + Credentials for DeepSeek API access.
    /// - Do not commit real keys.
    /// </summary>
    public ServiceSecretOptions DeepSeek { get; init; } = new();
}
