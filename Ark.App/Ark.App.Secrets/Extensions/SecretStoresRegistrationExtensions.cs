using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Ark.App.Secrets.Stores
{
    /// <summary>
    /// Provides dependency injection helpers for the <see cref="CompositeSecretStore"/>.
    /// + Centralizes provider routing using an embedded JSON resource.
    /// - Assumes individual secret store services are already registered.
    /// </summary>
    public static class SecretStoresRegistrationExtensions
    {

        #region Methods
        /// <summary>
        /// Registers a <see cref="CompositeSecretStore"/> with built-in providers and default routing from resources.
        /// + Simplifies secret provider configuration for applications.
        /// - Fails if required provider services are missing from the container.
        /// </summary>
        /// <param name="services">Service collection to update.</param>
        /// <param name="resourceName">Embedded resource containing provider routing data.</param>
        /// <returns>The modified service collection.</returns>
        public static IServiceCollection AddCompositeSecretStore(this IServiceCollection services, string resourceName = "Ark.App.Secrets.Resources.providers.json")
        {
            services.AddSingleton(sp =>
            {
                var routing = CompositeSecretStore.LoadRoutingFromResource(resourceName);
                var providers = new Dictionary<string, ISecretStore>
                {
                    ["aws"] = sp.GetRequiredService<AwsSecretsManagerStore>(),
                    ["azure"] = sp.GetRequiredService<AzureKeyVaultSecretStore>(),
                    ["gcp"] = sp.GetRequiredService<GoogleSecretManagerStore>(),
                    ["env"] = sp.GetRequiredService<EnvironmentVariableSecretStore>()
                };
                return new CompositeSecretStore(providers, routing);
            });
            services.AddSingleton<ISecretStore>(sp => sp.GetRequiredService<CompositeSecretStore>());
            return services;
        }
        #endregion Methods
    }
}
