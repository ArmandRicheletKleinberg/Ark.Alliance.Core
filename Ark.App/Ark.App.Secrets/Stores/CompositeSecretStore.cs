using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ark;

namespace Ark.App.Secrets.Stores
{
    /// <summary>
    /// Composite secret store: tries a primary store and falls back to another store for reads.
    /// </summary>
    public sealed class CompositeSecretStore : ISecretStore
    {
        private readonly ISecretStore _primary;
        private readonly ISecretStore _fallback;

        /// <summary>
        /// Initializes a new instance of <see cref="CompositeSecretStore"/>.
        /// </summary>
        /// <param name="primary">Primary store used for read/write.</param>
        /// <param name="fallback">Fallback store used only when a value is missing.</param>
        public CompositeSecretStore(ISecretStore primary, ISecretStore fallback)
        {
            _primary = primary;
            _fallback = fallback;
        }

        /// <inheritdoc />
        public async Task<Result<string?>> GetSecretAsync(string canonicalName, CancellationToken ct = default)
        {
            var r = await _primary.GetSecretAsync(canonicalName, ct).ConfigureAwait(false);
            if (r.IsSuccess && r.Data is null)
            {
                return await _fallback.GetSecretAsync(canonicalName, ct).ConfigureAwait(false);
            }
            return r;
        }

        /// <inheritdoc />
        public Task<Result> SetSecretAsync(string canonicalName, string value, CancellationToken ct = default)
            => _primary.SetSecretAsync(canonicalName, value, ct);

        /// <inheritdoc />
        public Task<Result> DeleteSecretAsync(string canonicalName, CancellationToken ct = default)
            => _primary.DeleteSecretAsync(canonicalName, ct);

        /// <inheritdoc />
        public Task<Result<IReadOnlyDictionary<string, string>>> ListByPrefixAsync(string canonicalFolderPrefix, CancellationToken ct = default)
            => _primary.ListByPrefixAsync(canonicalFolderPrefix, ct);
    }
}
