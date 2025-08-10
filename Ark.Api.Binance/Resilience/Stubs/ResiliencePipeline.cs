using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Resilience
{
    /// <summary>
    /// Executes asynchronous actions with basic retry logic.
    /// + Handles transient failures without extra dependencies.
    /// - Lacks delay or backoff between retries.
    /// Ref: <see href="https://learn.microsoft.com/dotnet/core/extensions/resilience"/>
    /// </summary>
    public class ResiliencePipeline
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
        /// Executes the provided asynchronous <paramref name="action"/> until it succeeds or the retry limit is reached.
        /// + Simplifies transient fault handling for <see cref="Task"/>-based work.
        /// - No delay is applied between retries.
        /// Ref: <see href="https://learn.microsoft.com/dotnet/core/extensions/resilience"/>
        /// </summary>
        /// <param name="action">Operation to execute.</param>
        /// <param name="cancellationToken">Token used to cancel the operation.</param>
        /// <returns>
        /// <see cref="Task"/> representing the asynchronous execution.
        /// </returns>
        public async Task ExecuteAsync(Func<CancellationToken, Task> action, CancellationToken cancellationToken = default)
        {
            for (var attempt = 0; ; attempt++)
            {
                try
                {
                    await action(cancellationToken);
                    return;
                }
                catch when (attempt < _retryAttempts)
                {
                }
            }
        }

        #endregion Methods
    }
}
