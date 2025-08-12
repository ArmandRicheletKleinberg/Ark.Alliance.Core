using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;

namespace Ark.App.Secrets.Stores
{
    /// <summary>
    /// AWS Secrets Manager backed store.
    /// </summary>
    public sealed class AwsSecretsManagerStore : SecretStoreBase
    {
        #region Fields
        private readonly IAmazonSecretsManager _client;
        #endregion

        #region Ctors
        /// <summary>
        /// Initializes the store with a given AWS Secrets Manager client.
        /// </summary>
        public AwsSecretsManagerStore(IAmazonSecretsManager client)
            => _client = client ?? throw new ArgumentNullException(nameof(client));
        #endregion

        #region Public Overrides

        /// <inheritdoc />
        public override async Task<Result<string?>> GetSecretAsync(string canonicalName, CancellationToken ct = default)
        {
            try
            {
                var resp = await _client.GetSecretValueAsync(new GetSecretValueRequest
                {
                    SecretId = canonicalName
                }, ct).ConfigureAwait(false);

                return new Result<string?>(resp.SecretString);
            }
            catch (ResourceNotFoundException ex)
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
                try
                {
                    // Try update (PutSecretValue) if secret exists
                    await _client.PutSecretValueAsync(new PutSecretValueRequest
                    {
                        SecretId = canonicalName,
                        SecretString = value
                    }, ct).ConfigureAwait(false);
                }
                catch (ResourceNotFoundException)
                {
                    // Create if not exists
                    await _client.CreateSecretAsync(new CreateSecretRequest
                    {
                        Name = canonicalName,
                        SecretString = value
                    }, ct).ConfigureAwait(false);
                }
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
                await _client.DeleteSecretAsync(new DeleteSecretRequest
                {
                    SecretId = canonicalName,
                    ForceDeleteWithoutRecovery = true
                }, ct).ConfigureAwait(false);
                return Result.Success;
            }
            catch (ResourceNotFoundException)
            {
                return Result.Success; // idempotent
            }
            catch (Exception ex)
            {
                return Result.Failure.WithReason(ex.Message);
            }
        }

        /// <inheritdoc />
        public override async Task<Result<IReadOnlyDictionary<string, string>>> ListByPrefixAsync(string canonicalFolderPrefix, CancellationToken ct = default)
        {
            try
            {
                var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                string? token = null;
                do
                {
                    var resp = await _client.ListSecretsAsync(new ListSecretsRequest { NextToken = token }, ct).ConfigureAwait(false);
                    foreach (var s in resp.SecretList ?? Enumerable.Empty<SecretListEntry>())
                    {
                        if (!string.IsNullOrEmpty(s.Name) && s.Name.StartsWith(canonicalFolderPrefix, StringComparison.OrdinalIgnoreCase))
                        {
                            var val = await GetSecretAsync(s.Name, ct).ConfigureAwait(false);
                            if (val.IsSuccess && val.Data is not null) dict[s.Name] = val.Data;
                        }
                    }
                    token = resp.NextToken;
                } while (!string.IsNullOrEmpty(token));

                return new Result<IReadOnlyDictionary<string, string>>(dict);
            }
            catch (Exception ex)
            {
                return new Result<IReadOnlyDictionary<string, string>>().WithException(ex);
            }
        }

        #endregion
    }
}
