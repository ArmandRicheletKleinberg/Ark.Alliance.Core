using System;

namespace Microsoft.Extensions.Resilience
{
    /// <summary>
    /// Builds <see cref="ResiliencePipeline{TResult}"/> instances.
    /// + Provides fluent configuration for pipeline behavior.
    /// - Limited to retry options.
    /// Ref: <see href="https://learn.microsoft.com/dotnet/core/extensions/resilience"/>
    /// </summary>
    /// <typeparam name="TResult">Type of result produced by the pipeline.</typeparam>
    public class ResiliencePipelineBuilder<TResult>
    {
        #region Fields

        private int _retryAttempts = 0;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Adds retry support to the pipeline using the provided <paramref name="configure"/> delegate.
        /// + Enables custom retry configuration.
        /// - Cannot configure backoff or other strategies.
        /// </summary>
        /// <param name="configure">Delegate to configure <see cref="RetryOptions"/>.</param>
        /// <returns>
        /// The current <see cref="ResiliencePipelineBuilder{TResult}"/> instance for chaining.
        /// </returns>
        public ResiliencePipelineBuilder<TResult> AddRetry(Action<RetryOptions> configure)
        {
            var opts = new RetryOptions();
            configure(opts);
            _retryAttempts = opts.MaxRetryAttempts;
            return this;
        }

        /// <summary>
        /// Builds a <see cref="ResiliencePipeline{TResult}"/> using the configured options.
        /// + Produces a pipeline ready for execution.
        /// - Only retry attempts are considered.
        /// </summary>
        /// <returns>
        /// A configured <see cref="ResiliencePipeline{TResult}"/>.
        /// </returns>
        public ResiliencePipeline<TResult> Build() => new ResiliencePipeline<TResult>(_retryAttempts);

        #endregion Methods
    }
}
