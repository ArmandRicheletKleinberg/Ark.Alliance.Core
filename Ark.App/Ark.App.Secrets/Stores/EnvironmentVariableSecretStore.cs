using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ark;


namespace Ark.App.Secrets.Stores
{
    /// <summary>
    /// Local fallback store based on environment variables.
    /// </summary>
    public sealed class EnvironmentVariableSecretStore : ISecretStore
    {
        /// <inheritdoc />
        public Task<Result<string?>> GetSecretAsync(string canonicalName, CancellationToken ct = default)
        {
            try
            {
                var value = Environment.GetEnvironmentVariable(canonicalName);
                return Task.FromResult(Result.Success<string?>(value));
            }
            catch (Exception ex)
            {
                return Task.FromResult(Result.Failure<string?>(ex.Message));
            }
        }

        /// <inheritdoc />
        public Task<Result> SetSecretAsync(string canonicalName, string value, CancellationToken ct = default)
        {
            try
            {
                Environment.SetEnvironmentVariable(canonicalName, value, EnvironmentVariableTarget.User);
                return Task.FromResult(Result.Success);
            }
            catch (Exception ex)
            {
                return Task.FromResult(Result.Failure.WithReason(ex.Message));
            }
        }

        /// <inheritdoc />
        public Task<Result> DeleteSecretAsync(string canonicalName, CancellationToken ct = default)
        {
            try
            {
                Environment.SetEnvironmentVariable(canonicalName, null, EnvironmentVariableTarget.User);
                return Task.FromResult(Result.Success);
            }
            catch (Exception ex)
            {
                return Task.FromResult(Result.Failure.WithReason(ex.Message));
            }
        }

        /// <inheritdoc />
        public Task<Result<IReadOnlyDictionary<string, string>>> ListByPrefixAsync(string canonicalFolderPrefix, CancellationToken ct = default)
        {
            try
            {
                var dict = Environment.GetEnvironmentVariables()
                    .Cast<System.Collections.DictionaryEntry>()
                    .Where(e => e.Key is string k && k.StartsWith(canonicalFolderPrefix, StringComparison.OrdinalIgnoreCase))
                    .ToDictionary(e => (string)e.Key!, e => (string?)e.Value ?? string.Empty, StringComparer.OrdinalIgnoreCase);

                return Task.FromResult(Result.Success((IReadOnlyDictionary<string, string>)dict));
            }
            catch (Exception ex)
            {
                return Task.FromResult(Result.Failure<IReadOnlyDictionary<string, string>>(ex.Message));
            }
        }
    }
}
