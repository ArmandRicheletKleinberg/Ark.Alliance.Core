using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ark.App.Secrets.Stores
{
    /// <summary>
    /// Fallback store backed by environment variables.
    /// </summary>
    public sealed class EnvironmentVariableSecretStore : SecretStoreBase
    {
        #region Public Overrides

        /// <inheritdoc />
        public override Task<Result<string?>> GetSecretAsync(string canonicalName, CancellationToken ct = default)
        {
            try
            {
                var val = Environment.GetEnvironmentVariable(canonicalName);
                return Task.FromResult(new Result<string?>(val));
            }
            catch (Exception ex)
            {
                return Task.FromResult(new Result<string?>().WithException(ex));
            }
        }

        /// <inheritdoc />
        public override Task<Result> SetSecretAsync(string canonicalName, string value, CancellationToken ct = default)
        {
            try
            {
                Environment.SetEnvironmentVariable(canonicalName, value, EnvironmentVariableTarget.Process);
                return Task.FromResult(Result.Success);
            }
            catch (Exception ex)
            {
                return Task.FromResult(Result.Failure.WithReason(ex.Message));
            }
        }

        /// <inheritdoc />
        public override Task<Result> DeleteSecretAsync(string canonicalName, CancellationToken ct = default)
        {
            try
            {
                Environment.SetEnvironmentVariable(canonicalName, null, EnvironmentVariableTarget.Process);
                return Task.FromResult(Result.Success);
            }
            catch (Exception ex)
            {
                return Task.FromResult(Result.Failure.WithReason(ex.Message));
            }
        }

        /// <inheritdoc />
        public override Task<Result<IReadOnlyDictionary<string, string>>> ListByPrefixAsync(string canonicalFolderPrefix, CancellationToken ct = default)
        {
            try
            {
                var env = Environment.GetEnvironmentVariables(EnvironmentVariableTarget.Process);
                var dict = env.Cast<DictionaryEntry>()
                              .Where(e => e.Key is string k && k.StartsWith(canonicalFolderPrefix, StringComparison.OrdinalIgnoreCase))
                              .ToDictionary(e => (string)e.Key!, e => (string?)e.Value ?? string.Empty, StringComparer.OrdinalIgnoreCase);
                return Task.FromResult(new Result<IReadOnlyDictionary<string, string>>(dict));
            }
            catch (Exception ex)
            {
                return Task.FromResult(new Result<IReadOnlyDictionary<string, string>>().WithException(ex));
            }
        }

        #endregion
    }
}
