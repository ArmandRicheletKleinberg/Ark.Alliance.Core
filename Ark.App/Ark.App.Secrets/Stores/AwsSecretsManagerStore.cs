using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Ark;

namespace Ark.App.Secrets.Stores
{
    /// <summary>
    /// AWS Secrets Manager backed store.
    /// </summary>
    public sealed class AwsSecretsManagerStore : ISecretStore
    {
        private readonly IAmazonSecretsManager _client;

        /// <summary>
        /// Initializes the store using an injected <see cref="IAmazonSecretsManager"/> client.
        /// </summary>
        /// <param name="client">AWS Secrets Manager client instance.</param>
        public AwsSecretsManagerStore(IAmazonSecretsManager client)
        {
            _client = client;
        }

        /// <inheritdoc />
        public async Task<Result<string?>> GetSecretAsync(string canonicalName, CancellationToken ct = default)
        {
            try
            {
                var req = new GetSecretValueRequest { SecretId = canonicalName };
                var resp = await _client.GetSecretValueAsync(req, ct).ConfigureAwait(false);
                if (!string.IsNullOrEmpty(resp.SecretString))
                    return Result.Success<string?>(resp.SecretString);
                if (resp.SecretBinary is not null)
                    return Result.Success<string?>(Encoding.UTF8.GetString(resp.SecretBinary.ToArray()));
                return Result.Success<string?>(null);
            }
            catch (ResourceNotFoundException)
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
                try
                {
                    var create = new CreateSecretRequest { Name = canonicalName, SecretString = value };
                    await _client.CreateSecretAsync(create, ct).ConfigureAwait(false);
                }
                catch (ResourceExistsException)
                {
                    var put = new PutSecretValueRequest { SecretId = canonicalName, SecretString = value };
                    await _client.PutSecretValueAsync(put, ct).ConfigureAwait(false);
                }

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
                await _client.DeleteSecretAsync(new DeleteSecretRequest { SecretId = canonicalName, ForceDeleteWithoutRecovery = true }, ct).ConfigureAwait(false);
                return Result.Success;
            }
            catch (ResourceNotFoundException)
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
            // Could implement via ListSecrets with client-side filtering and pagination.
            return Task.FromResult(Result.Success((IReadOnlyDictionary<string, string>)new Dictionary<string, string>()));
        }
    }
}
