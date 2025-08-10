using Microsoft.Extensions.Logging;

namespace Ark.Net.CrossCutting
{
    /// <summary>
    /// The main class of the cross cutting services.
    /// Static and only used for services initialization.
    /// </summary>
    public static class CrossCuttingServices
    {
        #region Static

        /// <summary>
        /// The cross cutting server root URL.
        /// </summary>
        public static string CrossCuttingServerUrl { get; private set; }

        /// <summary>
        /// The identifier of the application that will use the cross cutting services.
        /// </summary>
        public static string ApplicationId { get; private set; }

        /// <summary>
        /// The version number for this client library.
        /// </summary>
        public static string ClientVersion { get; private set; }

        /// <summary>
        /// The logger used to log the problem with the CrossCutting services.
        /// </summary>
        internal static ILogger Logger { get; private set; }

        /// <summary>
        /// Initializes the cross cutting services by providing a server root URL.
        /// </summary>
        /// <param name="crossCuttingServerUrl">The cross cutting server root URL.</param>
        /// <param name="applicationId">The identifier of the application that needs to use the services.</param>
        /// <param name="logger">The logger used to log the problem with the CrossCutting services.</param>
        public static void Initialize(string crossCuttingServerUrl, string applicationId, ILogger logger)
        {
            CrossCuttingServerUrl = crossCuttingServerUrl;
            ApplicationId = applicationId;
            ClientVersion = typeof(CrossCuttingServices).Assembly.GetName().Version.ToString();
            Logger = logger;
        }

        #endregion Static
    }
}