using System;
using System.Collections.Generic;
using Ark.App.Secrets.Stores;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Amazon.Extensions.NETCore.Setup;
using Amazon.SecretsManager;

namespace Ark.App.Secrets.Extensions
{
    /// <summary>
    /// Provides dependency injection helpers to wire secret stores and the manager.
    /// + Registers each supported provider and a composite router.
    /// - Does not configure any external store credentials automatically.
    /// </summary>
    public static class SecretsRegistrationExtensions
    {
        #region Methods
        /// <summary>
        /// Registers provider stores, the composite router and the <see cref="SecretsManager"/>.
        /// + Simplifies consumer configuration by bundling default providers.
        /// - Throws when required configuration sections are missing.
        /// </summary>
        /// <param name="services">Service collection to populate.</param>
        /// <param name="configuration">Configuration source for provider options.</param>
        /// <returns>The modified service collection.</returns>
        public static IServiceCollection AddArkSecrets(this IServiceCollection services, IConfiguration configuration)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            if (configuration is null) throw new ArgumentNullException(nameof(configuration));

            // AWS client (optional if you don't use AWS)
            services.AddDefaultAWSOptions(configuration.GetAWSOptions());
            services.AddAWSService<IAmazonSecretsManager>();

            // Providers
            services.AddSingleton<AwsSecretsManagerStore>();
            services.AddSingleton<AzureKeyVaultSecretStore>();
            services.AddSingleton<EnvironmentVariableSecretStore>();
            // Optional: GoogleSecretManagerStore can be added similarly if present
            // services.AddSingleton<GoogleSecretManagerStore>();

            // Composite (dictionary + routing table from embedded resource)
            services.AddSingleton<CompositeSecretStore>(sp =>
            {
                var routing = CompositeSecretStore.LoadRoutingFromResource("Ark.App.Secrets.Resources.providers.json");
                var providers = new Dictionary<string, ISecretStore>(StringComparer.OrdinalIgnoreCase)
                {
                    ["aws"] = sp.GetRequiredService<AwsSecretsManagerStore>(),
                    ["azure"] = sp.GetRequiredService<AzureKeyVaultSecretStore>(),
                    ["env"] = sp.GetRequiredService<EnvironmentVariableSecretStore>()
                };
                // If you register GCP, uncomment:
                // providers["gcp"] = sp.GetRequiredService<GoogleSecretManagerStore>();
                return new CompositeSecretStore(providers, routing);
            });

            // Unified abstraction
            services.AddSingleton<ISecretStore>(sp => sp.GetRequiredService<CompositeSecretStore>());

            // Cache and manager (example manager that depends on ISecretStore + IMemoryCache)
            services.AddMemoryCache();
            services.AddSingleton<SecretsManager>();

            return services;
        }
        #endregion Methods
    }
}
