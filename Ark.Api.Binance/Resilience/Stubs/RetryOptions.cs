using System;

namespace Microsoft.Extensions.Resilience
{
    /// <summary>
    /// Configures retry behavior for the <see cref="ResiliencePipeline"/> and <see cref="ResiliencePipeline{TResult}"/>.
    /// + Simplifies specifying maximum retry attempts.
    /// - Lacks support for backoff strategies.
    /// Ref: <see href="https://learn.microsoft.com/dotnet/core/extensions/resilience"/>
    /// </summary>
    public class RetryOptions
    {
        #region Properties

        /// <summary>
        /// Maximum number of retry attempts before failing.
        /// </summary>
        public int MaxRetryAttempts { get; set; } = 3;

        #endregion Properties
    }
}
