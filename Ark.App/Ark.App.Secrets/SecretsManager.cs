using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Ark.App.Secrets.Model;
using Ark.App.Secrets.Stores;
using Ark;

namespace Ark.App.Secrets
{
    /// <summary>
    /// Central access layer for provider secrets with environment awareness and caching.
    /// </summary>
    public sealed class SecretsManager
    {
        #region Fields
        #endregion

        #region Ctors
        #endregion

        #region Properties
        #endregion

        #region Public Methods
        #endregion

        #region Private Methods
        #endregion

        private readonly ISecretStore _store;
        private readonly IMemoryCache _cache;
        private readonly ILogger<SecretsManager>? _logger;
        private readonly SecretIndexManager _indexer;

        /// <summary>
        /// Initializes a new instance of <see cref="SecretsManager"/>.
        /// </summary>
        /// <param name="store">The underlying secret store implementation.</param>
        /// <param name="cache">The in-memory cache used to cache reads.</param>
        /// <param name="logger">Optional logger instance.</param>
        public SecretsManager(ISecretStore store, IMemoryCache cache, ILogger<SecretsManager>? logger = null)
        {
            _store = store;
            _cache = cache;
            _logger = logger;
            _indexer = new SecretIndexManager(store, logger);
        }

        /// <summary>
        /// Retrieves a single secret by key. Values are cached for a short duration.
        /// </summary>
        /// <param name="key">The strongly-typed <see cref="SecretKey"/>.</param>
        /// <param name="cacheTtl">Optional TTL for cache entries (defaults to 5 minutes).</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>The secret value or <c>null</c> when not found.</returns>
        public async Task<Result<string?>> GetAsync(SecretKey key, TimeSpan? cacheTtl = null, CancellationToken ct = default)
        {
            var canonical = key.ToCanonicalName();
            if (_cache.TryGetValue(canonical, out string? cached))
            {
                return new Result<string?>(cached);
            }

            var r = await _store.GetSecretAsync(canonical, ct).ConfigureAwait(false);
            if (r.IsSuccess)
            {
                _cache.Set(canonical, r.Data, cacheTtl ?? TimeSpan.FromMinutes(5));
            }
            else
            {
                _logger?.LogError("Failed to read secret {Key}: {Error}", canonical, r.Reason);
            }
            return r;
        }

        /// <summary>
        /// Creates or updates a secret value in the underlying store and refreshes cache.
        /// </summary>
        /// <param name="key">Secret key.</param>
        /// <param name="value">Secret value.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Operation result.</returns>
        public async Task<Result> SetAsync(SecretKey key, string value, CancellationToken ct = default)
        {
            var canonical = key.ToCanonicalName();
            var r = await _store.SetSecretAsync(canonical, value, ct).ConfigureAwait(false);
            if (r.IsSuccess)
            {
                _cache.Set(canonical, value, TimeSpan.FromMinutes(5));
                var _ = await _indexer.AddToIndexAsync(canonical, ct).ConfigureAwait(false);
            }
            else
            {
                _logger?.LogError("Failed to set secret {Key}: {Error}", canonical, r.Reason);
            }
            return r;
        }

        /// <summary>
        /// Deletes a secret if present and invalidates cache.
        /// </summary>
        /// <param name="key">Secret key.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Operation result.</returns>
        public async Task<Result> DeleteAsync(SecretKey key, CancellationToken ct = default)
        {
            var canonical = key.ToCanonicalName();
            var r = await _store.DeleteSecretAsync(canonical, ct).ConfigureAwait(false);
            _cache.Remove(canonical);
            if (r.IsSuccess)
            {
                var _ = await _indexer.RemoveFromIndexAsync(canonical, ct).ConfigureAwait(false);
            }
            return r;
        }

        /// <summary>
        /// Bulk setup of a provider/service/environment secret bundle (e.g., during provisioning).
        /// </summary>
        /// <param name="provider">Provider name.</param>
        /// <param name="service">Service name.</param>
        /// <param name="env">Environment.</param>
        /// <param name="entries">Dictionary of secret names to values.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Operation result.</returns>
        public async Task<Result> SetupProviderServiceAsync(
            string provider,
            string service,
            SecretEnvironment env,
            IReadOnlyDictionary<string, string> entries,
            CancellationToken ct = default)
        {
            foreach (var kv in entries)
            {
                var key = new SecretKey(provider, service, env, kv.Key);
                var r = await SetAsync(key, kv.Value, ct).ConfigureAwait(false);
                if (!r.IsSuccess) return r;
            }
            return Result.Success;
        }

        /// <summary>
        /// Reads an entire bundle (all entries under provider/service/environment) best-effort.
        /// </summary>
        /// <param name="provider">Provider name.</param>
        /// <param name="service">Service name.</param>
        /// <param name="env">Environment.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Dictionary of canonical key to value.</returns>
        public async Task<Result<IReadOnlyDictionary<string, string>>> ReadBundleAsync(
            string provider,
            string service,
            SecretEnvironment env,
            CancellationToken ct = default)
        {
            try
            {
                var prefix = new SecretKey(provider, service, env, "x").ToFolderPrefix();
                var r = await _store.ListByPrefixAsync(prefix, ct).ConfigureAwait(false);
                if (r.IsSuccess && (r.Data?.Count ?? 0) == 0)
                {
                    var byIdx = await _indexer.ReadBundleByIndexAsync(prefix, ct).ConfigureAwait(false);
                    return byIdx;
                }
                return r;
            }
            catch (Exception ex)
            {
                return new Result<IReadOnlyDictionary<string, string>>().WithStatus(ResultStatus.Failure).WithException(ex).WithReason(ex.Message);
            }
        }

        /// <summary>
        /// Migrates all secrets for a provider/service/environment from this manager's store to a destination store.
        /// </summary>
        /// <param name="destinationStore">Destination store.</param>
        /// <param name="provider">Provider name.</param>
        /// <param name="service">Service name.</param>
        /// <param name="env">Environment.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Operation result.</returns>
        public async Task<Result> MigrateAsync(
            ISecretStore destinationStore,
            string provider,
            string service,
            SecretEnvironment env,
            CancellationToken ct = default)
        {
            var prefix = new SecretKey(provider, service, env, "x").ToFolderPrefix();
            var list = await _store.ListByPrefixAsync(prefix, ct).ConfigureAwait(false);
            if (!list.IsSuccess) return Result.Failure.WithReason(list.Reason);

            foreach (var kvp in list.Data)
            {
                var set = await destinationStore.SetSecretAsync(kvp.Key, kvp.Value, ct).ConfigureAwait(false);
                if (!set.IsSuccess) return set;
            }
            return Result.Success;
        }
    }
}
