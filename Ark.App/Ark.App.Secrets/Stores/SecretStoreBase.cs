using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Ark.App.Secrets.Stores
{
    /// <summary>
    /// Provides a standardized base for secret store implementations.
    /// Unifies method signatures and exposes helper methods for consistent <see cref="Result"/> creation.
    /// </summary>
    public abstract class SecretStoreBase : ISecretStore
    {
        #region Public Abstract Methods

        /// <summary>
        /// Gets a secret value by its canonical name.
        /// </summary>
        /// <param name="canonicalName">Canonical secret key (e.g. "AWS:Binance:Prod:ApiKey").</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A <see cref="Result{T}"/> carrying the value or the failure reason.</returns>
        public abstract Task<Result<string?>> GetSecretAsync(string canonicalName, CancellationToken ct = default);

        /// <summary>
        /// Sets or updates a secret value.
        /// </summary>
        /// <param name="canonicalName">Canonical secret key.</param>
        /// <param name="value">Secret value to store.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A <see cref="Result"/> indicating success or failure.</returns>
        public abstract Task<Result> SetSecretAsync(string canonicalName, string value, CancellationToken ct = default);

        /// <summary>
        /// Deletes a secret value if it exists.
        /// </summary>
        /// <param name="canonicalName">Canonical secret key.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A <see cref="Result"/> indicating success or failure.</returns>
        public abstract Task<Result> DeleteSecretAsync(string canonicalName, CancellationToken ct = default);

        /// <summary>
        /// Lists secrets by a canonical folder-like prefix.
        /// Providers that do not support server-side filtering may emulate it client-side.
        /// </summary>
        /// <param name="canonicalFolderPrefix">The canonical folder prefix.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A <see cref="Result{T}"/> with a dictionary of key/value pairs.</returns>
        public abstract Task<Result<IReadOnlyDictionary<string, string>>> ListByPrefixAsync(string canonicalFolderPrefix, CancellationToken ct = default);

        #endregion

        #region Protected Helper Methods

        /// <summary>Creates a success result with data.</summary>
        protected static Result<T> Ok<T>(T value) => new Result<T>(value);

        /// <summary>Creates a success result without data.</summary>
        protected static Result Ok() => Result.Success;

        /// <summary>Creates a not-found result with an optional reason.</summary>
        protected static Result<T> NotFound<T>(string? reason = null)
            => new Result<T>().WithStatus(ResultStatus.NotFound).WithReason(reason ?? "Not found.");

        /// <summary>Creates a failure result from an exception.</summary>
        protected static Result<T> Error<T>(Exception ex) => Result<T>.Failure.WithReason(ex.Message);

        /// <summary>Creates a failure result from an exception.</summary>
        protected static Result Error(Exception ex) => Result.Failure.WithReason(ex.Message);

        #endregion
    }
}
