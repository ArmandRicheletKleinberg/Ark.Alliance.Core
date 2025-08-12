using Ark.App.Secrets.Options;
using Ark.App.Secrets.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ark.App.Secrets.Extensions;

/// <summary>
/// + Dependency injection helpers for registering secret options.
/// - Does not provide external secret storage integration.
/// </summary>
public static class SecretsServiceCollectionExtensions
{
    /// <summary>
    /// + Registers <see cref="SecretsOptions"/> and the default <see cref="ISecretsProvider"/>.
    /// - Binds secrets from the <c>Secrets</c> configuration section.
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
}
