using System.Threading;
using System.Threading.Tasks;

namespace Ark.App.Secrets.Typed
{
    /// <summary>
    /// Backward-compatibility shim delegating to <see cref="TypedSecretsFactory"/>.
    /// + Allows legacy callers to obtain typed secrets.
    /// - New implementations should consume <see cref="TypedSecretsFactory"/> directly.
    /// </summary>
    public static class SecretsTypedHelper
    {
        #region Methods
        /// <summary>
        /// Creates <c>XApiV2Secrets</c> from the declarative schema.
        /// + Simplifies migration from older helpers.
        /// - Only supports the XApiV2 schema.
        /// </summary>
        /// <param name="factory">Factory used to materialize secrets.</param>
        /// <param name="env">Target environment string.</param>
        /// <param name="ct">Optional cancellation token.</param>
        /// <returns>The typed secret instance when found.</returns>
        public static Task<Result<TypedSecrets.XApiV2Secrets?>> GetXApiV2Async(TypedSecretsFactory factory, string env, CancellationToken ct = default)
            => factory.CreateAsync<TypedSecrets.XApiV2Secrets>("XApiV2Secrets", env, ct);
        #endregion Methods
    }
}
