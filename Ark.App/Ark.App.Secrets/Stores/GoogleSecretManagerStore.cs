using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Google.Cloud.SecretManager.V1;
using Google.Api.Gax.ResourceNames;
using Ark;

namespace Ark.App.Secrets.Stores
{
    /// <summary>
    /// Google Secret Manager backed store.
    /// </summary>
    public sealed class GoogleSecretManagerStore : ISecretStore
    {
        private readonly SecretManagerServiceClient _client;
        private readonly string _projectId;

        /// <summary>
        /// Initializes a new instance using default credentials and a GCP project ID.
        /// </summary>
        /// <param name="projectId">GCP project identifier.</param>
        public GoogleSecretManagerStore(string projectId)
        {
            _client = SecretManagerServiceClient.Create();
            _projectId = projectId;
        }

        /// <inheritdoc />
        public async Task<Result<string?>> GetSecretAsync(string canonicalName, CancellationToken ct = default)
        {
            try
            {
                var name = new SecretVersionName(_projectId, canonicalName, "latest");
                var resp = await _client.AccessSecretVersionAsync(name, cancellationToken: ct).ConfigureAwait(false);
                return Result.Success<string?>(resp.Payload.Data.ToStringUtf8());
            }
            catch (Grpc.Core.RpcException ex) when (ex.StatusCode == Grpc.Core.StatusCode.NotFound)
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
                var parent = new ProjectName(_projectId);
                try
                {
                    await _client.CreateSecretAsync(parent, canonicalName, new Secret { Replication = new Replication { Automatic = new Replication.Types.Automatic() } }, ct)
                                 .ConfigureAwait(false);
                }
                catch (Grpc.Core.RpcException ex) when (ex.StatusCode == Grpc.Core.StatusCode.AlreadyExists)
                {
                    // Secret already exists, continue to add a new version.
                }

                var secretName = new SecretName(_projectId, canonicalName);
                await _client.AddSecretVersionAsync(secretName, new SecretPayload { Data = Google.Protobuf.ByteString.CopyFromUtf8(value) }, cancellationToken: ct)
                             .ConfigureAwait(false);
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
                var secretName = new SecretName(_projectId, canonicalName);
                await _client.DeleteSecretAsync(secretName, cancellationToken: ct).ConfigureAwait(false);
                return Result.Success;
            }
            catch (Grpc.Core.RpcException ex) when (ex.StatusCode == Grpc.Core.StatusCode.NotFound)
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
            // Could be implemented via ListSecrets + client-side filtering.
            return Task.FromResult(Result.Success((IReadOnlyDictionary<string, string>)new Dictionary<string, string>()));
        }
    }
}
