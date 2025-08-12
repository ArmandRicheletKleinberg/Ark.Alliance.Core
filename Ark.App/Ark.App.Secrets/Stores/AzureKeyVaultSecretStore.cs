using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Ark;

namespace Ark.App.Secrets.Stores
{
    /// <summary>
    /// Azure Key Vault backed secret store.
    /// </summary>
    public sealed class AzureKeyVaultSecretStore : ISecretStore
    {
        private readonly SecretClient _client;

        /// <summary>
        /// Initializes a store with the given Key Vault URI.
        /// </summary>
        /// <param name="vaultUri">The URI of the Azure Key Vault.</param>
        public AzureKeyVaultSecretStore(Uri vaultUri)
        {
            _client = new SecretClient(vaultUri, new DefaultAzureCredential());
        }

        /// <inheritdoc />
        public async Task<Result<string?>> GetSecretAsync(string canonicalName, CancellationToken ct = default)
        {
            try
            {
                var resp = await _client.GetSecretAsync(canonicalName, cancellationToken: ct).ConfigureAwait(false);
                return Result.Success<string?>(resp.Value.Value);
            }
            catch (Azure.RequestFailedException ex) when (ex.Status == 404)
            {
                return Result.Success<string?>(null);
            }
            catch (Exception ex)
            {
                return Result.Failure<string?>(ex.Message);
            }
        }

        /// <inheritdoc />
        public async Task<Result> SetSecretAsync(string canonicalName, string value, CancellationToken ct = default)
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
        public async Task<Result> DeleteSecretAsync(string canonicalName, CancellationToken ct = default)
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
        public Task<Result<IReadOnlyDictionary<string, string>>> ListByPrefixAsync(string canonicalFolderPrefix, CancellationToken ct = default)
        {
            // Azure Key Vault does not support server-side prefix listing by name.
            // Consider tagging secrets at creation time and filtering by tag here.
            return Task.FromResult(Result.Success((IReadOnlyDictionary<string, string>)new Dictionary<string, string>()));
        }
    }
}
