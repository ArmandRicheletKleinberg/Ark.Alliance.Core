using Ark.App.Secrets.Options;

namespace Ark.App.Secrets.Services;

/// <summary>
/// Exposes configured API secrets via strongly typed options.
/// + Centralizes credential access for consuming services.
/// - Offers read-only views without rotation capabilities.
/// </summary>
public interface ISecretsProvider
{
    #region Properties
    /// <summary>Gets Binance credentials.</summary>
    ServiceSecretOptions Binance { get; }

    /// <summary>Gets OpenAI credentials.</summary>
    ServiceSecretOptions OpenAI { get; }

    /// <summary>Gets Anthropic credentials.</summary>
    ServiceSecretOptions Anthropic { get; }

    /// <summary>Gets DeepSeek credentials.</summary>
    ServiceSecretOptions DeepSeek { get; }
    #endregion Properties
}
