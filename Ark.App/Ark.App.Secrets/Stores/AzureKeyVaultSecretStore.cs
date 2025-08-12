using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace Ark.App.Secrets.Stores
{
    /// <summary>
    /// Azure Key Vault backed secret store.
    /// </summary>
    public sealed class AzureKeyVaultSecretStore : SecretStoreBase
    {
        #region Fields
        private readonly SecretClient _client;
        #endregion

        #region Ctors

        /// <summary>
        /// Initializes a store with the given Key Vault URI using DefaultAzureCredential.
        /// </summary>
        /// <param name="vaultUri">The URI of the Azure Key Vault.</param>
        public AzureKeyVaultSecretStore(Uri vaultUri)
        {
            _client = new SecretClient(vaultUri, new DefaultAzureCredential());
        }

        /// <summary>
        /// Initializes a store with a custom SecretClient (preferred for DI).
        /// </summary>
        public AzureKeyVaultSecretStore(SecretClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        #endregion

        #region Public Overrides

        /// <inheritdoc />
        public override async Task<Result<string?>> GetSecretAsync(string canonicalName, CancellationToken ct = default)
        {
            try
            {
                var resp = await _client.GetSecretAsync(canonicalName, cancellationToken: ct).ConfigureAwait(false);
                return new Result<string?>(resp.Value.Value);
            }
            catch (Azure.RequestFailedException ex) when (ex.Status == 404)
            {
                return new Result<string?>().WithException(ex);
            }
            catch (Exception ex)
            {
                return new Result<string?>().WithException(ex);
            }
        }

        /// <inheritdoc />
        public override async Task<Result> SetSecretAsync(string canonicalName, string value, CancellationToken ct = default)
        {
            try
            {
                await _client.SetSecretAsync(new KeyVaultSecret(canonicalName, value), ct).ConfigureAwait(false);
                return Result.Success;
            }
            catch (Exception ex)
            {
                return Result.Failure.WithReason(ex.Message);
            }
        }

        /// <inheritdoc />
        public override async Task<Result> DeleteSecretAsync(string canonicalName, CancellationToken ct = default)
        {
            try
            {
                var op = await _client.StartDeleteSecretAsync(canonicalName, ct).ConfigureAwait(false);
                await op.WaitForCompletionAsync(ct).ConfigureAwait(false);
                return Result.Success;
            }
            catch (Azure.RequestFailedException ex) when (ex.Status == 404)
            {
                return Result.Success;
            }
            catch (Exception ex)
            {
                return Result.Failure.WithReason(ex.Message);
            }
        }

        /// <inheritdoc />
        public override Task<Result<IReadOnlyDictionary<string, string>>> ListByPrefixAsync(string canonicalFolderPrefix, CancellationToken ct = default)
        {
            // Azure Key Vault does not support server-side name prefix filtering. Return empty set.
            return Task.FromResult(new Result<IReadOnlyDictionary<string, string>>(new Dictionary<string, string>()));
        }

        #endregion
    }
}
