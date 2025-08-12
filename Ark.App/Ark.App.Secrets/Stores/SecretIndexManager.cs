using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Ark;
using Microsoft.Extensions.Logging;

namespace Ark.App.Secrets.Stores
{
    /// <summary>
    /// Maintains a lightweight index of canonical secret names for a given folder prefix,
    /// to compensate for providers that do not support server-side prefix listing.
    /// </summary>
    public sealed class SecretIndexManager
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
        private readonly ILogger? _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="SecretIndexManager"/>.
        /// </summary>
        /// <param name="store">Underlying secret store.</param>
        /// <param name="logger">Optional logger.</param>
        public SecretIndexManager(ISecretStore store, ILogger? logger = null)
        {
            _store = store;
            _logger = logger;
        }

        /// <summary>
        /// Derives the canonical folder prefix from a canonical secret name.
        /// </summary>
        /// <param name="canonicalName">Canonical secret name.</param>
        /// <returns>Canonical folder prefix.</returns>
        public static string GetFolderPrefixFromCanonical(string canonicalName)
        {
            var idx = canonicalName.LastIndexOf("--", StringComparison.Ordinal);
            if (idx < 0) return string.Empty;
            return canonicalName.Substring(0, idx + 2);
        }

        /// <summary>
        /// Builds the canonical index key for a given canonical folder prefix.
        /// This uses only lower-case letters and dashes to be compatible with Azure Key Vault.
        /// </summary>
        /// <param name="canonicalFolderPrefix">Canonical folder-like prefix.</param>
        /// <returns>Canonical index secret name.</returns>
        public static string BuildIndexKey(string canonicalFolderPrefix)
            => $"{canonicalFolderPrefix}index";

        /// <summary>
        /// Adds a secret name to its folder index (idempotent).
        /// </summary>
        /// <param name="canonicalName">Canonical secret name.</param>
        /// <param name="ct">Cancellation token.</param>
        public async Task<Result> AddToIndexAsync(string canonicalName, CancellationToken ct = default)
        {
            try
            {
                var prefix = GetFolderPrefixFromCanonical(canonicalName);
                var indexKey = BuildIndexKey(prefix);
                var existing = await _store.GetSecretAsync(indexKey, ct).ConfigureAwait(false);
                if (!existing.IsSuccess) return existing.ToVoid();

                var list = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                  if (!string.IsNullOrWhiteSpace(existing.Data))
                {
                    try
                    {
                          var parsed = JsonSerializer.Deserialize<string[]>(existing.Data!);
                        if (parsed is not null)
                            foreach (var s in parsed) list.Add(s);
                    }
                    catch (Exception ex)
                    {
                        _logger?.LogWarning(ex, "Failed to parse index {IndexKey}, reinitializing.", indexKey);
                    }
                }
                list.Add(canonicalName);
                var json = JsonSerializer.Serialize(list.OrderBy(s => s, StringComparer.OrdinalIgnoreCase));
                var set = await _store.SetSecretAsync(indexKey, json, ct).ConfigureAwait(false);
                return set;
            }
            catch (Exception ex)
            {
                return Result.Failure.WithReason(ex.Message);
            }
        }

        /// <summary>
        /// Removes a secret name from its folder index (idempotent).
        /// </summary>
        /// <param name="canonicalName">Canonical secret name.</param>
        /// <param name="ct">Cancellation token.</param>
        public async Task<Result> RemoveFromIndexAsync(string canonicalName, CancellationToken ct = default)
        {
            try
            {
                var prefix = GetFolderPrefixFromCanonical(canonicalName);
                var indexKey = BuildIndexKey(prefix);
                var existing = await _store.GetSecretAsync(indexKey, ct).ConfigureAwait(false);
                if (!existing.IsSuccess) return existing.ToVoid();

                  if (string.IsNullOrWhiteSpace(existing.Data))
                      return Result.Success;

                var list = new List<string>();
                try
                {
                      var parsed = JsonSerializer.Deserialize<string[]>(existing.Data!);
                    if (parsed is not null) list.AddRange(parsed);
                }
                catch (Exception ex)
                {
                    _logger?.LogWarning(ex, "Failed to parse index {IndexKey} during removal.", indexKey);
                }

                var newList = new List<string>();
                foreach (var s in list)
                    if (!string.Equals(s, canonicalName, StringComparison.OrdinalIgnoreCase))
                        newList.Add(s);

                var json = JsonSerializer.Serialize(newList);
                var set = await _store.SetSecretAsync(indexKey, json, ct).ConfigureAwait(false);
                return set;
            }
            catch (Exception ex)
            {
                return Result.Failure.WithReason(ex.Message);
            }
        }

        /// <summary>
        /// Reads key/value pairs for all canonical names under a given folder using the index.
        /// </summary>
        /// <param name="canonicalFolderPrefix">Canonical folder-like prefix.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Dictionary of canonical name to value.</returns>
        public async Task<Result<IReadOnlyDictionary<string, string>>> ReadBundleByIndexAsync(string canonicalFolderPrefix, CancellationToken ct = default)
        {
            try
            {

            
                var indexKey = BuildIndexKey(canonicalFolderPrefix);
                var existing = await _store.GetSecretAsync(indexKey, ct).ConfigureAwait(false);
                  if (!existing.IsSuccess) new Result<IReadOnlyDictionary<string, string>>().WithStatus(ResultStatus.Failure).WithReason(existing.Reason);
                  if (string.IsNullOrWhiteSpace(existing.Data))
                    return new Result<IReadOnlyDictionary<string, string>>(new Dictionary<string, string>()).WithReason("Secret Empty");

                string[]? names = null;
                try
                {
                      names = JsonSerializer.Deserialize<string[]>(existing.Data!);
                }
                catch (Exception ex)
                {
                    _logger?.LogWarning(ex, "Failed to parse index {IndexKey}.", indexKey);
                }

                var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                if (names is null || names.Length == 0)
                    return new Result<IReadOnlyDictionary<string, string>>(dict);

                foreach (var name in names)
                {
                    var v = await _store.GetSecretAsync(name, ct).ConfigureAwait(false);
                      if (v.IsSuccess && v.Data is not null)
                          dict[name] = v.Data;
                }
                return new Result<IReadOnlyDictionary<string, string>>(dict);
            }
            catch (Exception ex)
            {
                return new Result<IReadOnlyDictionary<string, string>>().WithReason(ex.Message).WithException(ex);
            }
        }
    }
}