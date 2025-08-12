using System;

namespace Ark.App.Secrets.Typed
{
    /// <summary>
    /// Strongly-typed secret bundles for well-known providers.
    /// </summary>
    public static class TypedSecrets
    {
        // --------- Base providers ---------

        /// <summary>Bloomberg MarketData secrets bundle.</summary>
        public sealed record BloombergMarketDataSecrets(string ApiKey, string ApiSecret, string EndpointUrl);

        /// <summary>Bloomberg ReferenceData secrets bundle.</summary>
        public sealed record BloombergReferenceDataSecrets(string ApiKey, string ApiSecret, string EndpointUrl);

        /// <summary>Reuters / Refinitiv RDP secrets bundle.</summary>
        public sealed record ReutersRdpSecrets(string AppKey, string Username, string Password, string EndpointUrl);

        /// <summary>EDIFACT AS2 secrets bundle.</summary>
        public sealed record EdifactAs2Secrets(string As2Id, string PartnerAs2Id, string As2Url, string ClientCertificatePem, string ClientPrivateKeyPem, string PrivateKeyPassphrase);

        /// <summary>SWIFT FIN secrets bundle.</summary>
        public sealed record SwiftFinSecrets(string Bic, string ClientCertificatePem, string ClientPrivateKeyPem, string PrivateKeyPassphrase, string EndpointUrl);

        /// <summary>TradingView Webhook secrets bundle.</summary>
        public sealed record TradingViewWebhookSecrets(string WebhookSecret, string EndpointUrl);

        /// <summary>Google Core API secrets bundle.</summary>
        public sealed record GoogleCoreSecrets(string ApiKey, string OAuth2ClientId, string OAuth2ClientSecret);

        
        /// <summary>VesselFinder core secrets bundle.</summary>
        public sealed record VesselFinderCoreSecrets(string ApiKey, string EndpointUrl);

        /// <summary>NASA core secrets bundle.</summary>
        public sealed record NasaCoreSecrets(string ApiKey, string EndpointUrl);

        /// <summary>OpenAI core secrets bundle.</summary>
        public sealed record OpenAICoreSecrets(string ApiKey, string EndpointUrl, string? OrganizationId);

        /// <summary>Anthropic core secrets bundle.</summary>
        public sealed record AnthropicCoreSecrets(string ApiKey, string EndpointUrl);

        /// <summary>DeepSeek core secrets bundle.</summary>
        public sealed record DeepSeekCoreSecrets(string ApiKey, string EndpointUrl);

        /// <summary>Binance Futures secrets bundle.</summary>
        public sealed record BinanceFuturesSecrets(string ApiKey, string ApiSecret, string EndpointUrl);

        /// <summary>Binance Spot secrets bundle.</summary>
        public sealed record BinanceSpotSecrets(string ApiKey, string ApiSecret, string EndpointUrl);

        /// <summary>Deribit core secrets bundle.</summary>
        public sealed record DeribitCoreSecrets(string ClientId, string ClientSecret, string EndpointUrl);

        
        /// <summary>Azure OpenAI Chat Completions deployment.</summary>
        public sealed record AzureOpenAIChatCompletionsSecrets(string ApiKey, string EndpointUrl, string DeploymentName, string ApiVersion);

        /// <summary>Azure OpenAI Embeddings deployment.</summary>
        public sealed record AzureOpenAIEmbeddingsSecrets(string ApiKey, string EndpointUrl, string DeploymentName, string ApiVersion);

        /// <summary>Generic Trading REST secrets.</summary>
        public sealed record TradingRestSecrets(string ApiKey, string ApiSecret, string EndpointUrl);

        /// <summary>HuggingFace Inference API.</summary>
        public sealed record HuggingFaceInferenceSecrets(string ApiToken, string EndpointUrl);

        /// <summary>HuggingFace Spaces API.</summary>
        public sealed record HuggingFaceSpacesSecrets(string ApiToken, string EndpointUrl);

        /// <summary>Facebook Graph API.</summary>
        public sealed record FacebookGraphSecrets(string AppId, string AppSecret, string AccessToken, string EndpointUrl);

        /// <summary>X (Twitter) API v2 secrets.</summary>
        public sealed record XApiV2Secrets(string ApiKey, string ApiSecret, string BearerToken, string ClientId, string ClientSecret, string EndpointUrl);

        /// <summary>Instagram Graph API.</summary>
        public sealed record InstagramGraphSecrets(string AppId, string AppSecret, string AccessToken, string EndpointUrl);

        /// <summary>Cloudflare general API.</summary>
        public sealed record CloudflareApiSecrets(string ApiToken, string AccountId, string EndpointUrl);

        /// <summary>Cloudflare DNS API.</summary>
        public sealed record CloudflareDnsSecrets(string ApiToken, string AccountId, string ZoneId, string EndpointUrl);

        /// <summary>Cloudflare Workers API.</summary>
        public sealed record CloudflareWorkersSecrets(string ApiToken, string AccountId, string EndpointUrl);

        /// <summary>Cloudflare R2 (S3-compatible) API.</summary>
        public sealed record CloudflareR2Secrets(string AccessKeyId, string SecretAccessKey, string AccountId, string EndpointUrl, string Bucket);

        /// <summary>RapidAPI Core.</summary>
        public sealed record RapidApiCoreSecrets(string ApiKey, string EndpointUrl);
    }
}
