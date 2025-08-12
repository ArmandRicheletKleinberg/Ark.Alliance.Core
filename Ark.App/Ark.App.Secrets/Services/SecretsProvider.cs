using Ark.App.Secrets.Options;
using Microsoft.Extensions.Options;

namespace Ark.App.Secrets.Services;

/// <summary>
/// + Default implementation of <see cref="ISecretsProvider"/> backed by <see cref="IOptions{TOptions}"/>.
/// - Stores secrets in memory only.
/// </summary>
public class SecretsProvider : ISecretsProvider
{
        #region Fields
   

    private readonly SecretsOptions _options;
    #endregion


    #region Public Methods
   

    /// <summary>
    /// + Initializes the provider with configured options.
    /// - Options are not validated.
    /// </summary>
    /// <param name="options">Bound secret options.</param>
    public SecretsProvider(IOptions<SecretsOptions> options)
    {
        _options = options.Value;
    }

    /// <inheritdoc />
    public ServiceSecretOptions Binance => _options.Binance;

    /// <inheritdoc />
    public ServiceSecretOptions OpenAI => _options.OpenAI;

    /// <inheritdoc />
    public ServiceSecretOptions Anthropic => _options.Anthropic;

    /// <inheritdoc />
    public ServiceSecretOptions DeepSeek => _options.DeepSeek;

    #endregion
}
