using System;
using System.Globalization;

namespace Ark.App.Secrets.Model
{
    /// <summary>
    /// Represents a fully-qualified secret key for a provider/service/environment/entry.
    /// </summary>
    /// <param name="Provider">Provider name, e.g. "Bloomberg".</param>
    /// <param name="Service">Service name within the provider, e.g. "MarketData" (or empty if N/A).</param>
    /// <param name="Env">Target <see cref="SecretEnvironment"/>.</param>
    /// <param name="Name">Secret entry name, e.g. "ApiKey".</param>
    public readonly record struct SecretKey(string Provider, string Service, SecretEnvironment Env, string Name)
    {
        /// <summary>
        /// Builds a canonical key string safe across common secret stores (Azure/AWS/GCP/Env).
        /// </summary>
        /// <returns>Canonical key.</returns>
        public string ToCanonicalName()
            => string.Create(CultureInfo.InvariantCulture, $"providers--{Provider.ToLowerInvariant()}--{Service.ToLowerInvariant()}--{Env.ToString().ToLowerInvariant()}--{Name.ToLowerInvariant()}");

        /// <summary>
        /// Creates a canonical prefix to group all entries for a provider/service/environment.
        /// </summary>
        /// <returns>Canonical folder-like prefix.</returns>
        public string ToFolderPrefix()
            => string.Create(CultureInfo.InvariantCulture, $"providers--{Provider.ToLowerInvariant()}--{Service.ToLowerInvariant()}--{Env.ToString().ToLowerInvariant()}--");
    }
}
