using System.Collections.Generic;
using Ark.App.Secrets.Model;

namespace Ark.App.Secrets.Providers
{
    /// <summary>
    /// Well-known providers/services and their default secret names.
    /// Each entry maps a service to the list of secret names expected under that (provider, service, env).
    /// </summary>
    public static class ProviderProfiles
    {
        #region Fields
        #endregion

        #region Ctors
        #endregion

        #region Properties
        #endregion

        #region Public Methods
        #endregion

        #region Private Methods
        #endregion

        /// <summary>Bloomberg: API key/secret + endpoint per service.</summary>
        public static readonly (string Service, string[] Names)[] Bloomberg = new[]
        {
            ("MarketData", new[] { "ApiKey", "ApiSecret", "EndpointUrl" }),
            ("ReferenceData", new[] { "ApiKey", "ApiSecret", "EndpointUrl" }),
        };

        /// <summary>Reuters/Refinitiv: RDP credentials.</summary>
        public static readonly (string Service, string[] Names)[] Reuters = new[]
        {
            ("RDP", new[] { "AppKey", "Username", "Password", "EndpointUrl" })
        };

        /// <summary>EDIFACT/AS2: identifiers, certs and endpoints.</summary>
        public static readonly (string Service, string[] Names)[] Edifact = new[]
        {
            ("AS2", new[] { "As2Id", "PartnerAs2Id", "As2Url", "ClientCertificatePem", "ClientPrivateKeyPem", "PrivateKeyPassphrase" })
        };

        /// <summary>SWIFT FIN: BIC, client cert/key, passphrase and endpoint.</summary>
        public static readonly (string Service, string[] Names)[] Swift = new[]
        {
            ("Fin", new[] { "Bic", "ClientCertificatePem", "ClientPrivateKeyPem", "PrivateKeyPassphrase", "EndpointUrl" })
        };

        /// <summary>TradingView: webhook secret and endpoint.</summary>
        public static readonly (string Service, string[] Names)[] TradingView = new[]
        {
            ("Webhook", new[] { "WebhookSecret", "EndpointUrl" })
        };

        /// <summary>Google APIs: API key and OAuth2 client info.</summary>
        public static readonly (string Service, string[] Names)[] Google = new[]
        {
            ("Core", new[] { "ApiKey", "OAuth2ClientId", "OAuth2ClientSecret" })
        };

        
        /// <summary>VesselFinder API.</summary>
        public static readonly (string Service, string[] Names)[] VesselFinder = new[]
        {
            ("Core", new[] { "ApiKey", "EndpointUrl" })
        };

        /// <summary>NASA Open APIs.</summary>
        public static readonly (string Service, string[] Names)[] Nasa = new[]
        {
            ("Core", new[] { "ApiKey", "EndpointUrl" })
        };

        /// <summary>OpenAI API.</summary>
        public static readonly (string Service, string[] Names)[] OpenAI = new[]
        {
            ("Core", new[] { "ApiKey", "EndpointUrl", "OrganizationId" })
        };

        /// <summary>Anthropic API.</summary>
        public static readonly (string Service, string[] Names)[] Anthropic = new[]
        {
            ("Core", new[] { "ApiKey", "EndpointUrl" })
        };

        /// <summary>DeepSeek API.</summary>
        public static readonly (string Service, string[] Names)[] DeepSeek = new[]
        {
            ("Core", new[] { "ApiKey", "EndpointUrl" })
        };

        /// <summary>Binance (Futures and Spot).</summary>
        public static readonly (string Service, string[] Names)[] Binance = new[]
        {
            ("Futures", new[] { "ApiKey", "ApiSecret", "EndpointUrl" }),
            ("Spot", new[] { "ApiKey", "ApiSecret", "EndpointUrl" })
        };

        /// <summary>Deribit (options/futures).</summary>
        public static readonly (string Service, string[] Names)[] Deribit = new[]
        {
            ("Core", new[] { "ClientId", "ClientSecret", "EndpointUrl" })
        };

        
        /// <summary>Azure OpenAI: per-service deployments (e.g., ChatCompletions, Embeddings).</summary>
        public static readonly (string Service, string[] Names)[] AzureOpenAI = new[]
        {
            ("ChatCompletions", new[] { "ApiKey", "EndpointUrl", "DeploymentName", "ApiVersion" }),
            ("Embeddings", new[] { "ApiKey", "EndpointUrl", "DeploymentName", "ApiVersion" })
        };

        /// <summary>Generic trading REST service (for brokers/exchanges not explicitly modeled).</summary>
        public static readonly (string Service, string[] Names)[] Trading = new[]
        {
            ("REST", new[] { "ApiKey", "ApiSecret", "EndpointUrl" })
        };

        /// <summary>HuggingFace APIs.</summary>
        public static readonly (string Service, string[] Names)[] HuggingFace = new[]
        {
            ("InferenceApi", new[] { "ApiToken", "EndpointUrl" }),
            ("Spaces", new[] { "ApiToken", "EndpointUrl" })
        };

        /// <summary>Facebook Graph API.</summary>
        public static readonly (string Service, string[] Names)[] Facebook = new[]
        {
            ("GraphApi", new[] { "AppId", "AppSecret", "AccessToken", "EndpointUrl" })
        };

        /// <summary>X (Twitter) API v2 / OAuth2.</summary>
        public static readonly (string Service, string[] Names)[] X = new[]
        {
            ("ApiV2", new[] { "ApiKey", "ApiSecret", "BearerToken", "ClientId", "ClientSecret", "EndpointUrl" })
        };

        /// <summary>Instagram Graph API.</summary>
        public static readonly (string Service, string[] Names)[] Instagram = new[]
        {
            ("GraphApi", new[] { "AppId", "AppSecret", "AccessToken", "EndpointUrl" })
        };

        /// <summary>Cloudflare services.</summary>
        public static readonly (string Service, string[] Names)[] Cloudflare = new[]
        {
            ("Api", new[] { "ApiToken", "AccountId", "EndpointUrl" }),
            ("DNS", new[] { "ApiToken", "AccountId", "ZoneId", "EndpointUrl" }),
            ("Workers", new[] { "ApiToken", "AccountId", "EndpointUrl" }),
            ("R2", new[] { "AccessKeyId", "SecretAccessKey", "AccountId", "EndpointUrl", "Bucket" })
        };

        /// <summary>RapidAPI marketplace.</summary>
        public static readonly (string Service, string[] Names)[] RapidAPI = new[]
        {
            ("Core", new[] { "ApiKey", "EndpointUrl" })
        };
    }
}
