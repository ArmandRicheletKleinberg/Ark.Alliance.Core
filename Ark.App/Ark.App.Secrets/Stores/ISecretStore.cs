using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ark;

namespace Ark.App.Secrets.Stores
{
    /// <summary>
    /// Abstraction over different secret backends such as Azure, AWS, Google and environment variables.
    /// + Provides a unified API for retrieving and managing secrets.
    /// - Implementation specifics vary per provider and may have limitations.
    /// </summary>
    public interface ISecretStore
    {
        #region Methods
        /// <summary>
        /// Gets a secret value by its canonical name.
        /// + Useful for configuration retrieval at runtime.
        /// - Returns <c>null</c> when the secret is missing.
        /// </summary>
        /// <param name="canonicalName">Canonical name created by <c>SecretKey.ToCanonicalName()</c>.</param>
        /// <param name="ct">A token to observe while waiting for the task to complete.</param>
        /// <returns>Secret value, or <c>null</c> if not found.</returns>
        Task<Result<string?>> GetSecretAsync(string canonicalName, CancellationToken ct = default);

        /// <summary>
        /// Creates or updates a secret value.
        /// + Overwrites existing entries when supported by the provider.
        /// - May be disallowed on read-only stores.
        /// </summary>
        /// <param name="canonicalName">Canonical name.</param>
        /// <param name="value">Value to store.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Operation result.</returns>
        Task<Result> SetSecretAsync(string canonicalName, string value, CancellationToken ct = default);

        /// <summary>
        /// Deletes a secret value if it exists.
        /// + Helps clean up obsolete configuration entries.
        /// - Some providers may retain historical versions.
        /// </summary>
        /// <param name="canonicalName">Canonical name.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Operation result.</returns>
        Task<Result> DeleteSecretAsync(string canonicalName, CancellationToken ct = default);

        /// <summary>
        /// Lists all secrets under a canonical folder prefix.
        /// + Allows bulk processing of grouped secrets.
        /// - Some providers emulate this behavior client-side.
        /// </summary>
        /// <param name="canonicalFolderPrefix">Canonical folder-like prefix.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Dictionary of canonical key to value.</returns>
        Task<Result<IReadOnlyDictionary<string, string>>> ListByPrefixAsync(string canonicalFolderPrefix, CancellationToken ct = default);
        #endregion Methods
    }
}
