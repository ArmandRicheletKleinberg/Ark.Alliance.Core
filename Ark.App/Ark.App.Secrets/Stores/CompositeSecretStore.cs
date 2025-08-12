using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Ark.App.Secrets.Stores
{
    /// <summary>
    /// Routes secret operations to specific providers based on a prefix routing table.
    /// </summary>
    public sealed class CompositeSecretStore : SecretStoreBase
    {
        #region Fields

        private readonly IReadOnlyDictionary<string, ISecretStore> _providers;
        private readonly RoutingTable _routing;

        #endregion

        #region Ctors

        /// <summary>
        /// Creates a new composite store.
        /// </summary>
        /// <param name="providers">Map of provider keys (e.g. "aws") to concrete stores.</param>
        /// <param name="routing">Routing table mapping key prefixes to provider keys.</param>
        public CompositeSecretStore(IReadOnlyDictionary<string, ISecretStore> providers, RoutingTable routing)
        {
            _providers = providers ?? throw new ArgumentNullException(nameof(providers));
            _routing = routing ?? throw new ArgumentNullException(nameof(routing));
        }

        #endregion

        #region Public Overrides

        /// <inheritdoc />
        public override Task<Result<string?>> GetSecretAsync(string canonicalName, CancellationToken ct = default)
            => Route(canonicalName).GetSecretAsync(canonicalName, ct);

        /// <inheritdoc />
        public override Task<Result> SetSecretAsync(string canonicalName, string value, CancellationToken ct = default)
            => Route(canonicalName).SetSecretAsync(canonicalName, value, ct);

        /// <inheritdoc />
        public override Task<Result> DeleteSecretAsync(string canonicalName, CancellationToken ct = default)
            => Route(canonicalName).DeleteSecretAsync(canonicalName, ct);

        /// <inheritdoc />
        public override Task<Result<IReadOnlyDictionary<string, string>>> ListByPrefixAsync(string canonicalFolderPrefix, CancellationToken ct = default)
            => Route(canonicalFolderPrefix).ListByPrefixAsync(canonicalFolderPrefix, ct);

        #endregion

        #region Private Methods

        private ISecretStore Route(string key)
        {
            if (_routing.PrefixToProvider is null || _routing.PrefixToProvider.Count == 0)
                throw new InvalidOperationException("Routing table is empty.");
            // Choose the longest matching prefix for deterministic routing
            var match = _routing.PrefixToProvider.Keys
                .OrderByDescending(k => k.Length)
                .FirstOrDefault(prefix => key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));

            if (match is null)
                throw new InvalidOperationException($"No provider mapping found for key '{key}'.");

            var providerKey = _routing.PrefixToProvider[match];
            if (!_providers.TryGetValue(providerKey, out var store))
                throw new InvalidOperationException($"Provider '{providerKey}' not found in provider map.");

            return store;
        }

        #endregion

        #region Static Helpers

        /// <summary>
        /// Loads the routing table JSON from an embedded resource.
        /// </summary>
        /// <param name="resourceName">Fully qualified resource name.</param>
        public static RoutingTable LoadRoutingFromResource(string resourceName)
        {
            var asm = Assembly.GetExecutingAssembly();
            using var stream = asm.GetManifestResourceStream(resourceName)
                               ?? throw new InvalidOperationException($"Embedded resource '{resourceName}' not found.");
            var table = JsonSerializer.Deserialize<RoutingTable>(stream, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return table ?? new RoutingTable();
        }

        #endregion
    }
}
