using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Resilience
{
    /// <summary>
    /// Executes asynchronous functions with retry behavior and a result.
    /// + Handles transient failures for operations returning a value.
    /// - Does not support jitter or exponential backoff.
    /// Ref: <see href="https://learn.microsoft.com/dotnet/core/extensions/resilience"/>
    /// </summary>
    /// <typeparam name="TResult">Type of the result returned by the operation.</typeparam>
    public class ResiliencePipeline<TResult>
    {
        #region Fields

        private readonly int _retryAttempts;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance with the specified retry limit.
        /// + Allows customizing retry attempts.
        /// - No validation on negative values.
        /// </summary>
        /// <param name="retryAttempts">Maximum retry attempts.</param>
        public ResiliencePipeline(int retryAttempts = 3) => _retryAttempts = retryAttempts;

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Executes the provided asynchronous <paramref name="action"/> with retry behavior.
        /// + Simplifies transient fault handling.
        /// - No delay between retries.
        /// Ref: <see href="https://learn.microsoft.com/dotnet/core/extensions/resilience"/>
        /// </summary>
        /// <param name="action">Operation to execute.</param>
        /// <param name="cancellationToken">Token used to cancel the operation.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation result.
        /// </returns>
        public async Task<TResult> ExecuteAsync(Func<CancellationToken, Task<TResult>> action, CancellationToken cancellationToken = default)
        {
            for (var attempt = 0; ; attempt++)
            {
                try
                {
                    return await action(cancellationToken);
                }
                catch when (attempt < _retryAttempts)
                {
                }
            }
        }

        #endregion Methods
    }
}
