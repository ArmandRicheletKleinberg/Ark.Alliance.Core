using Ark.App.Secrets.Options;
using Ark.App.Secrets.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ark.App.Secrets.Extensions;

/// <summary>
/// Provides dependency injection helpers for registering secret options.
/// + Binds strongly typed settings from configuration for easy access.
/// - Does not integrate with external secret storage providers.
/// </summary>
public static class SecretsServiceCollectionExtensions
{
    #region Methods
    /// <summary>
    /// Registers <see cref="SecretsOptions"/> and the default <see cref="ISecretsProvider"/>.
    /// + Binds secrets from the <c>Secrets</c> configuration section.
    /// - Fails if the configuration section is missing.
    /// </summary>
    /// <param name="services">Service collection to modify.</param>
    /// <param name="configuration">Application configuration.</param>
    /// <returns>The modified service collection.</returns>
    public static IServiceCollection AddArkSecrets(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<SecretsOptions>(configuration.GetSection("Secrets"));
        services.AddSingleton<ISecretsProvider, SecretsProvider>();
        return services;
    }
    #endregion Methods
}
