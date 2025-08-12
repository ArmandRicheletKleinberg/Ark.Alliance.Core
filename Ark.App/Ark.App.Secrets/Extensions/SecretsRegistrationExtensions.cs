using System;
using Ark.App.Secrets;
using Ark.App.Secrets.Stores;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Amazon.Extensions.NETCore.Setup;

namespace Ark.App.Secrets.Extensions
{
    /// <summary>
    /// Dependency injection extensions to wire secret stores and the manager.
    /// </summary>
    public static class SecretsRegistrationExtensions
    {
        /// <summary>
        /// Registers a secret store based on configuration and exposes <see cref="SecretsManager"/>.
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <param name="configuration">Application configuration.</param>
        /// <returns>The same service collection for chaining.</returns>
        /// <remarks>
        /// Configuration examples:
        /// <code>
        /// "Secrets:Store": "AzureKeyVault",
        /// "Secrets:Azure:VaultUrl": "https://mykv.vault.azure.net/"
        /// // or
        /// "Secrets:Store": "AwsSecretsManager"
        /// // or
        /// "Secrets:Store": "GoogleSecretManager",
        /// "Secrets:Google:ProjectId": "my-project"
        /// // or
        /// "Secrets:Store": "Environment"
        /// </code>
        /// </remarks>
        public static IServiceCollection AddArkSecretsCore(this IServiceCollection services, IConfiguration configuration)
        {
            var storeType = configuration["Secrets:Store"] ?? "Environment";

            switch (storeType)
            {
                case "AzureKeyVault":
                {
                    var url = configuration["Secrets:Azure:VaultUrl"];
                    if (string.IsNullOrWhiteSpace(url)) throw new InvalidOperationException("Secrets:Azure:VaultUrl is required.");
                    services.AddSingleton<ISecretStore>(_ => new AzureKeyVaultSecretStore(new Uri(url)));
                    break;
                }
                case "AwsSecretsManager":
                {
                    services.AddAWSService<Amazon.SecretsManager.IAmazonSecretsManager>();
                    services.AddSingleton<ISecretStore, AwsSecretsManagerStore>();
                    break;
                }
                case "GoogleSecretManager":
                {
                    var projectId = configuration["Secrets:Google:ProjectId"];
                    if (string.IsNullOrWhiteSpace(projectId)) throw new InvalidOperationException("Secrets:Google:ProjectId is required.");
                    services.AddSingleton<ISecretStore>(_ => new GoogleSecretManagerStore(projectId));
                    break;
                }
                case "Environment":
                default:
                {
                    services.AddSingleton<ISecretStore, EnvironmentVariableSecretStore>();
                    break;
                }
            }

            if (bool.TryParse(configuration["Secrets:UseEnvFallbackOnGet"], out var useFallback) && useFallback)
            {
                // Wrap with a composite that falls back to environment on read miss.
                var provider = services.BuildServiceProvider();
                var primary = provider.GetRequiredService<ISecretStore>();
                var fallback = new EnvironmentVariableSecretStore();
                services.AddSingleton<ISecretStore>(_ => new CompositeSecretStore(primary, fallback));
            }

            services.AddMemoryCache();
            services.AddSingleton<SecretsManager>(sp =>
                new SecretsManager(sp.GetRequiredService<ISecretStore>(), sp.GetRequiredService<IMemoryCache>(), sp.GetService<Microsoft.Extensions.Logging.ILogger<SecretsManager>>())
            );

            return services;
        }
    }
}
