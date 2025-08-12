using System;
using System.Globalization;

namespace Ark.App.Secrets.Model
{
    /// <summary>
    /// Represents a fully qualified secret key for provider, service, environment and entry.
    /// + Produces canonical names compatible with common secret stores.
    /// - Does not validate parameter values or existence of entries.
    /// </summary>
    /// <param name="Provider">Provider name, e.g. "Bloomberg".</param>
    /// <param name="Service">Service name within the provider, e.g. "MarketData" (or empty if N/A).</param>
    /// <param name="Env">Target <see cref="SecretEnvironment"/>.</param>
    /// <param name="Name">Secret entry name, e.g. "ApiKey".</param>
    public readonly record struct SecretKey(string Provider, string Service, SecretEnvironment Env, string Name)
    {
        #region Methods
        /// <summary>
        /// Builds a canonical key string safe across common secret stores (Azure/AWS/GCP/Env).
        /// + Ensures consistent naming for cross-provider lookups.
        /// - Assumes <paramref name="Provider"/> and other fields are normalized.
        /// </summary>
        /// <returns>Canonical key.</returns>
        public string ToCanonicalName()
            => string.Create(CultureInfo.InvariantCulture, $"providers--{Provider.ToLowerInvariant()}--{Service.ToLowerInvariant()}--{Env.ToString().ToLowerInvariant()}--{Name.ToLowerInvariant()}");

        /// <summary>
        /// Creates a canonical prefix to group all entries for a provider/service/environment.
        /// + Useful when listing or deleting multiple secrets at once.
        /// - Returned prefix may be provider-specific.
        /// </summary>
        /// <returns>Canonical folder-like prefix.</returns>
        public string ToFolderPrefix()
            => string.Create(CultureInfo.InvariantCulture, $"providers--{Provider.ToLowerInvariant()}--{Service.ToLowerInvariant()}--{Env.ToString().ToLowerInvariant()}--");
        #endregion Methods
    }
}
