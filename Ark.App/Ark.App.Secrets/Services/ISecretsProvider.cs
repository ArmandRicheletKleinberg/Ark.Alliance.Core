using Ark.App.Secrets.Options;

namespace Ark.App.Secrets.Services;

/// <summary>
/// + Provides access to configured API secrets.
/// - Exposes only read-only options.
/// </summary>
public interface ISecretsProvider
{
    /// <summary>Gets Binance credentials.</summary>
    ServiceSecretOptions Binance { get; }

    /// <summary>Gets OpenAI credentials.</summary>
    ServiceSecretOptions OpenAI { get; }

    /// <summary>Gets Anthropic credentials.</summary>
    ServiceSecretOptions Anthropic { get; }

    /// <summary>Gets DeepSeek credentials.</summary>
    ServiceSecretOptions DeepSeek { get; }
}
