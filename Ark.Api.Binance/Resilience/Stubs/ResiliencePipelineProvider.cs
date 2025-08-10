using System;

namespace Microsoft.Extensions.Resilience
{
    /// <summary>
    /// Provides <see cref="ResiliencePipeline"/> instances with unified retry settings.
    /// + Centralizes pipeline creation logic.
    /// - Offers only simple retry configuration.
    /// Ref: <see href="https://learn.microsoft.com/dotnet/core/extensions/resilience"/>
    /// </summary>
    public class ResiliencePipelineProvider
    {
        #region Fields

        private readonly int _retryAttempts;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ResiliencePipelineProvider"/> class.
        /// + Exposes default retry attempt configuration.
        /// - Does not allow per-pipeline customization.
        /// </summary>
        /// <param name="retryAttempts">Maximum number of retry attempts for pipelines.</param>
        public ResiliencePipelineProvider(int retryAttempts = 3)
        {
            _retryAttempts = retryAttempts;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Retrieves a pipeline by name.
        /// + Creates simple retry pipelines on demand.
        /// - Ignores the provided <paramref name="name"/> parameter.
        /// </summary>
        /// <param name="name">Identifier of the pipeline.</param>
        /// <returns>
        /// <see cref="ResiliencePipeline"/> configured with the provider's retry attempts.
        /// </returns>
        public ResiliencePipeline GetPipeline(string name) => new ResiliencePipeline(_retryAttempts);

        /// <summary>
        /// Retrieves a pipeline for the specified result type.
        /// + Creates generic pipelines on demand.
        /// - Name parameter is not used.
        /// </summary>
        /// <typeparam name="TResult">Type of the operation result.</typeparam>
        /// <param name="name">Identifier of the pipeline.</param>
        /// <returns>
        /// <see cref="ResiliencePipeline{TResult}"/> configured with the provider's retry attempts.
        /// </returns>
        public ResiliencePipeline<TResult> GetPipeline<TResult>(string name) => new ResiliencePipeline<TResult>(_retryAttempts);

        #endregion Methods
    }
}
