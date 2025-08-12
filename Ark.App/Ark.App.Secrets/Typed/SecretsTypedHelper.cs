using System;
using System.Threading;
using System.Threading.Tasks;
using Ark.App.Secrets.Model;
using Ark;

namespace Ark.App.Secrets.Typed
{
    /// <summary>
    /// Helper methods to read/write strongly-typed secret bundles.
    /// </summary>
    public static class SecretsTypedHelper
    {
        // ----------------------- Bloomberg -----------------------

        /// <summary>
        /// Retrieves Bloomberg MarketData secrets for a given environment.
        /// </summary>
        /// <param name="manager">The secrets manager.</param>
        /// <param name="env">Target environment (Production/Sandbox/Test/Development).</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Result containing <see cref="TypedSecrets.BloombergMarketDataSecrets"/> or null if not fully configured.</returns>
        public static async Task<Result<TypedSecrets.BloombergMarketDataSecrets?>> GetBloombergMarketDataAsync(SecretsManager manager, SecretEnvironment env, CancellationToken ct = default)
        {
            var apiKey = await manager.GetAsync(new SecretKey("Bloomberg", "MarketData", env, "ApiKey"), ct: ct).ConfigureAwait(false);
            if (!apiKey.IsSuccess) return Result.Failure<TypedSecrets.BloombergMarketDataSecrets?>(apiKey.Reason);
            var apiSecret = await manager.GetAsync(new SecretKey("Bloomberg", "MarketData", env, "ApiSecret"), ct: ct).ConfigureAwait(false);
            if (!apiSecret.IsSuccess) return Result.Failure<TypedSecrets.BloombergMarketDataSecrets?>(apiSecret.Reason);
            var endpoint = await manager.GetAsync(new SecretKey("Bloomberg", "MarketData", env, "EndpointUrl"), ct: ct).ConfigureAwait(false);
            if (!endpoint.IsSuccess) return Result.Failure<TypedSecrets.BloombergMarketDataSecrets?>(endpoint.Reason);

            if (apiKey.Data is null || apiSecret.Data is null || endpoint.Data is null)
                return Result.Success<TypedSecrets.BloombergMarketDataSecrets?>(null);

            return Result.Success<TypedSecrets.BloombergMarketDataSecrets?>(
                new TypedSecrets.BloombergMarketDataSecrets(apiKey.Data, apiSecret.Data, endpoint.Data));
        }

        /// <summary>
        /// Sets Bloomberg MarketData secrets in a single call.
        /// </summary>
        /// <param name="manager">The secrets manager.</param>
        /// <param name="env">Target environment.</param>
        /// <param name="apiKey">Bloomberg API key.</param>
        /// <param name="apiSecret">Bloomberg API secret.</param>
        /// <param name="endpointUrl">Service endpoint URL.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Operation result.</returns>
        public static Task<Result> SetBloombergMarketDataAsync(SecretsManager manager, SecretEnvironment env, string apiKey, string apiSecret, string endpointUrl, CancellationToken ct = default)
            => manager.SetupProviderServiceAsync("Bloomberg", "MarketData", env, new System.Collections.Generic.Dictionary<string, string>
            {
                ["ApiKey"] = apiKey,
                ["ApiSecret"] = apiSecret,
                ["EndpointUrl"] = endpointUrl,
            }, ct);

        /// <summary>
        /// Retrieves Bloomberg ReferenceData secrets.
        /// </summary>
        public static async Task<Result<TypedSecrets.BloombergReferenceDataSecrets?>> GetBloombergReferenceDataAsync(SecretsManager manager, SecretEnvironment env, CancellationToken ct = default)
        {
            var apiKey = await manager.GetAsync(new SecretKey("Bloomberg", "ReferenceData", env, "ApiKey"), ct: ct).ConfigureAwait(false);
            if (!apiKey.IsSuccess) return Result.Failure<TypedSecrets.BloombergReferenceDataSecrets?>(apiKey.Reason);
            var apiSecret = await manager.GetAsync(new SecretKey("Bloomberg", "ReferenceData", env, "ApiSecret"), ct: ct).ConfigureAwait(false);
            if (!apiSecret.IsSuccess) return Result.Failure<TypedSecrets.BloombergReferenceDataSecrets?>(apiSecret.Reason);
            var endpoint = await manager.GetAsync(new SecretKey("Bloomberg", "ReferenceData", env, "EndpointUrl"), ct: ct).ConfigureAwait(false);
            if (!endpoint.IsSuccess) return Result.Failure<TypedSecrets.BloombergReferenceDataSecrets?>(endpoint.Reason);

            if (apiKey.Data is null || apiSecret.Data is null || endpoint.Data is null)
                return Result.Success<TypedSecrets.BloombergReferenceDataSecrets?>(null);

            return Result.Success<TypedSecrets.BloombergReferenceDataSecrets?>(
                new TypedSecrets.BloombergReferenceDataSecrets(apiKey.Data, apiSecret.Data, endpoint.Data));
        }

        /// <summary>
        /// Sets Bloomberg ReferenceData secrets.
        /// </summary>
        public static Task<Result> SetBloombergReferenceDataAsync(SecretsManager manager, SecretEnvironment env, string apiKey, string apiSecret, string endpointUrl, CancellationToken ct = default)
            => manager.SetupProviderServiceAsync("Bloomberg", "ReferenceData", env, new System.Collections.Generic.Dictionary<string, string>
            {
                ["ApiKey"] = apiKey,
                ["ApiSecret"] = apiSecret,
                ["EndpointUrl"] = endpointUrl,
            }, ct);

        // ----------------------- Reuters / Refinitiv -----------------------

        /// <summary>Retrieves Reuters/Refinitiv RDP secrets.</summary>
        public static async Task<Result<TypedSecrets.ReutersRdpSecrets?>> GetReutersRdpAsync(SecretsManager manager, SecretEnvironment env, CancellationToken ct = default)
        {
            var appKey = await manager.GetAsync(new SecretKey("Reuters", "RDP", env, "AppKey"), ct: ct).ConfigureAwait(false);
            if (!appKey.IsSuccess) return Result.Failure<TypedSecrets.ReutersRdpSecrets?>(appKey.Reason);
            var username = await manager.GetAsync(new SecretKey("Reuters", "RDP", env, "Username"), ct: ct).ConfigureAwait(false);
            if (!username.IsSuccess) return Result.Failure<TypedSecrets.ReutersRdpSecrets?>(username.Reason);
            var password = await manager.GetAsync(new SecretKey("Reuters", "RDP", env, "Password"), ct: ct).ConfigureAwait(false);
            if (!password.IsSuccess) return Result.Failure<TypedSecrets.ReutersRdpSecrets?>(password.Reason);
            var endpoint = await manager.GetAsync(new SecretKey("Reuters", "RDP", env, "EndpointUrl"), ct: ct).ConfigureAwait(false);
            if (!endpoint.IsSuccess) return Result.Failure<TypedSecrets.ReutersRdpSecrets?>(endpoint.Reason);

            if (appKey.Data is null || username.Data is null || password.Data is null || endpoint.Data is null)
                return Result.Success<TypedSecrets.ReutersRdpSecrets?>(null);

            return Result.Success<TypedSecrets.ReutersRdpSecrets?>(
                new TypedSecrets.ReutersRdpSecrets(appKey.Data, username.Data, password.Data, endpoint.Data));
        }

        /// <summary>Sets Reuters/Refinitiv RDP secrets.</summary>
        public static Task<Result> SetReutersRdpAsync(SecretsManager manager, SecretEnvironment env, string appKey, string username, string password, string endpointUrl, CancellationToken ct = default)
            => manager.SetupProviderServiceAsync("Reuters", "RDP", env, new System.Collections.Generic.Dictionary<string, string>
            {
                ["AppKey"] = appKey,
                ["Username"] = username,
                ["Password"] = password,
                ["EndpointUrl"] = endpointUrl,
            }, ct);

        // ----------------------- EDIFACT / AS2 -----------------------

        /// <summary>Retrieves EDIFACT/AS2 secrets.</summary>
        public static async Task<Result<TypedSecrets.EdifactAs2Secrets?>> GetEdifactAs2Async(SecretsManager manager, SecretEnvironment env, CancellationToken ct = default)
        {
            var as2Id = await manager.GetAsync(new SecretKey("Edifact", "AS2", env, "As2Id"), ct: ct).ConfigureAwait(false);
            if (!as2Id.IsSuccess) return Result.Failure<TypedSecrets.EdifactAs2Secrets?>(as2Id.Reason);
            var partnerId = await manager.GetAsync(new SecretKey("Edifact", "AS2", env, "PartnerAs2Id"), ct: ct).ConfigureAwait(false);
            if (!partnerId.IsSuccess) return Result.Failure<TypedSecrets.EdifactAs2Secrets?>(partnerId.Reason);
            var url = await manager.GetAsync(new SecretKey("Edifact", "AS2", env, "As2Url"), ct: ct).ConfigureAwait(false);
            if (!url.IsSuccess) return Result.Failure<TypedSecrets.EdifactAs2Secrets?>(url.Reason);
            var cert = await manager.GetAsync(new SecretKey("Edifact", "AS2", env, "ClientCertificatePem"), ct: ct).ConfigureAwait(false);
            if (!cert.IsSuccess) return Result.Failure<TypedSecrets.EdifactAs2Secrets?>(cert.Reason);
            var key = await manager.GetAsync(new SecretKey("Edifact", "AS2", env, "ClientPrivateKeyPem"), ct: ct).ConfigureAwait(false);
            if (!key.IsSuccess) return Result.Failure<TypedSecrets.EdifactAs2Secrets?>(key.Reason);
            var pass = await manager.GetAsync(new SecretKey("Edifact", "AS2", env, "PrivateKeyPassphrase"), ct: ct).ConfigureAwait(false);
            if (!pass.IsSuccess) return Result.Failure<TypedSecrets.EdifactAs2Secrets?>(pass.Reason);

            if (as2Id.Data is null || partnerId.Data is null || url.Data is null || cert.Data is null || key.Data is null || pass.Data is null)
                return Result.Success<TypedSecrets.EdifactAs2Secrets?>(null);

            return Result.Success<TypedSecrets.EdifactAs2Secrets?>(
                new TypedSecrets.EdifactAs2Secrets(as2Id.Data, partnerId.Data, url.Data, cert.Data, key.Data, pass.Data));
        }

        /// <summary>Sets EDIFACT/AS2 secrets.</summary>
        public static Task<Result> SetEdifactAs2Async(SecretsManager manager, SecretEnvironment env, string as2Id, string partnerAs2Id, string as2Url, string clientCertPem, string clientPrivateKeyPem, string privateKeyPassphrase, CancellationToken ct = default)
            => manager.SetupProviderServiceAsync("Edifact", "AS2", env, new System.Collections.Generic.Dictionary<string, string>
            {
                ["As2Id"] = as2Id,
                ["PartnerAs2Id"] = partnerAs2Id,
                ["As2Url"] = as2Url,
                ["ClientCertificatePem"] = clientCertPem,
                ["ClientPrivateKeyPem"] = clientPrivateKeyPem,
                ["PrivateKeyPassphrase"] = privateKeyPassphrase,
            }, ct);

        // ----------------------- SWIFT FIN -----------------------

        /// <summary>Retrieves SWIFT FIN secrets.</summary>
        public static async Task<Result<TypedSecrets.SwiftFinSecrets?>> GetSwiftFinAsync(SecretsManager manager, SecretEnvironment env, CancellationToken ct = default)
        {
            var bic = await manager.GetAsync(new SecretKey("Swift", "Fin", env, "Bic"), ct: ct).ConfigureAwait(false);
            if (!bic.IsSuccess) return Result.Failure<TypedSecrets.SwiftFinSecrets?>(bic.Reason);
            var cert = await manager.GetAsync(new SecretKey("Swift", "Fin", env, "ClientCertificatePem"), ct: ct).ConfigureAwait(false);
            if (!cert.IsSuccess) return Result.Failure<TypedSecrets.SwiftFinSecrets?>(cert.Reason);
            var key = await manager.GetAsync(new SecretKey("Swift", "Fin", env, "ClientPrivateKeyPem"), ct: ct).ConfigureAwait(false);
            if (!key.IsSuccess) return Result.Failure<TypedSecrets.SwiftFinSecrets?>(key.Reason);
            var pass = await manager.GetAsync(new SecretKey("Swift", "Fin", env, "PrivateKeyPassphrase"), ct: ct).ConfigureAwait(false);
            if (!pass.IsSuccess) return Result.Failure<TypedSecrets.SwiftFinSecrets?>(pass.Reason);
            var endpoint = await manager.GetAsync(new SecretKey("Swift", "Fin", env, "EndpointUrl"), ct: ct).ConfigureAwait(false);
            if (!endpoint.IsSuccess) return Result.Failure<TypedSecrets.SwiftFinSecrets?>(endpoint.Reason);

            if (bic.Data is null || cert.Data is null || key.Data is null || pass.Data is null || endpoint.Data is null)
                return Result.Success<TypedSecrets.SwiftFinSecrets?>(null);

            return Result.Success<TypedSecrets.SwiftFinSecrets?>(
                new TypedSecrets.SwiftFinSecrets(bic.Data, cert.Data, key.Data, pass.Data, endpoint.Data));
        }

        /// <summary>Sets SWIFT FIN secrets.</summary>
        public static Task<Result> SetSwiftFinAsync(SecretsManager manager, SecretEnvironment env, string bic, string clientCertPem, string clientPrivateKeyPem, string privateKeyPassphrase, string endpointUrl, CancellationToken ct = default)
            => manager.SetupProviderServiceAsync("Swift", "Fin", env, new System.Collections.Generic.Dictionary<string, string>
            {
                ["Bic"] = bic,
                ["ClientCertificatePem"] = clientCertPem,
                ["ClientPrivateKeyPem"] = clientPrivateKeyPem,
                ["PrivateKeyPassphrase"] = privateKeyPassphrase,
                ["EndpointUrl"] = endpointUrl,
            }, ct);

        // ----------------------- TradingView -----------------------

        /// <summary>Retrieves TradingView Webhook secrets.</summary>
        public static async Task<Result<TypedSecrets.TradingViewWebhookSecrets?>> GetTradingViewWebhookAsync(SecretsManager manager, SecretEnvironment env, CancellationToken ct = default)
        {
            var secret = await manager.GetAsync(new SecretKey("TradingView", "Webhook", env, "WebhookSecret"), ct: ct).ConfigureAwait(false);
            if (!secret.IsSuccess) return Result.Failure<TypedSecrets.TradingViewWebhookSecrets?>(secret.Reason);
            var endpoint = await manager.GetAsync(new SecretKey("TradingView", "Webhook", env, "EndpointUrl"), ct: ct).ConfigureAwait(false);
            if (!endpoint.IsSuccess) return Result.Failure<TypedSecrets.TradingViewWebhookSecrets?>(endpoint.Reason);

            if (secret.Data is null || endpoint.Data is null)
                return Result.Success<TypedSecrets.TradingViewWebhookSecrets?>(null);

            return Result.Success<TypedSecrets.TradingViewWebhookSecrets?>(
                new TypedSecrets.TradingViewWebhookSecrets(secret.Data, endpoint.Data));
        }

        /// <summary>Sets TradingView Webhook secrets.</summary>
        public static Task<Result> SetTradingViewWebhookAsync(SecretsManager manager, SecretEnvironment env, string webhookSecret, string endpointUrl, CancellationToken ct = default)
            => manager.SetupProviderServiceAsync("TradingView", "Webhook", env, new System.Collections.Generic.Dictionary<string, string>
            {
                ["WebhookSecret"] = webhookSecret,
                ["EndpointUrl"] = endpointUrl,
            }, ct);

        // ----------------------- Google -----------------------

        /// <summary>Retrieves Google Core API secrets.</summary>
        public static async Task<Result<TypedSecrets.GoogleCoreSecrets?>> GetGoogleCoreAsync(SecretsManager manager, SecretEnvironment env, CancellationToken ct = default)
        {
            var apiKey = await manager.GetAsync(new SecretKey("Google", "Core", env, "ApiKey"), ct: ct).ConfigureAwait(false);
            if (!apiKey.IsSuccess) return Result.Failure<TypedSecrets.GoogleCoreSecrets?>(apiKey.Reason);
            var cid = await manager.GetAsync(new SecretKey("Google", "Core", env, "OAuth2ClientId"), ct: ct).ConfigureAwait(false);
            if (!cid.IsSuccess) return Result.Failure<TypedSecrets.GoogleCoreSecrets?>(cid.Reason);
            var csecret = await manager.GetAsync(new SecretKey("Google", "Core", env, "OAuth2ClientSecret"), ct: ct).ConfigureAwait(false);
            if (!csecret.IsSuccess) return Result.Failure<TypedSecrets.GoogleCoreSecrets?>(csecret.Reason);

            if (apiKey.Data is null || cid.Data is null || csecret.Data is null)
                return Result.Success<TypedSecrets.GoogleCoreSecrets?>(null);

            return Result.Success<TypedSecrets.GoogleCoreSecrets?>(
                new TypedSecrets.GoogleCoreSecrets(apiKey.Data, cid.Data, csecret.Data));
        }

        /// <summary>Sets Google Core API secrets.</summary>
        public static Task<Result> SetGoogleCoreAsync(SecretsManager manager, SecretEnvironment env, string apiKey, string oauth2ClientId, string oauth2ClientSecret, CancellationToken ct = default)
            => manager.SetupProviderServiceAsync("Google", "Core", env, new System.Collections.Generic.Dictionary<string, string>
            {
                ["ApiKey"] = apiKey,
                ["OAuth2ClientId"] = oauth2ClientId,
                ["OAuth2ClientSecret"] = oauth2ClientSecret,
            }, ct);

        
        /// <summary>Retrieves VesselFinder Core secrets.</summary>
        public static async Task<Result<TypedSecrets.VesselFinderCoreSecrets?>> GetVesselFinderCoreAsync(SecretsManager manager, SecretEnvironment env, CancellationToken ct = default)
        {
            var key = await manager.GetAsync(new SecretKey("VesselFinder", "Core", env, "ApiKey"), ct: ct).ConfigureAwait(false);
            if (!key.IsSuccess) return Result.Failure<TypedSecrets.VesselFinderCoreSecrets?>(key.Reason);
            var endpoint = await manager.GetAsync(new SecretKey("VesselFinder", "Core", env, "EndpointUrl"), ct: ct).ConfigureAwait(false);
            if (!endpoint.IsSuccess) return Result.Failure<TypedSecrets.VesselFinderCoreSecrets?>(endpoint.Reason);

            if (key.Data is null || endpoint.Data is null)
                return Result.Success<TypedSecrets.VesselFinderCoreSecrets?>(null);

            return Result.Success<TypedSecrets.VesselFinderCoreSecrets?>(
                new TypedSecrets.VesselFinderCoreSecrets(key.Data, endpoint.Data));
        }

        /// <summary>Sets VesselFinder Core secrets.</summary>
        public static Task<Result> SetVesselFinderCoreAsync(SecretsManager manager, SecretEnvironment env, string apiKey, string endpointUrl, CancellationToken ct = default)
            => manager.SetupProviderServiceAsync("VesselFinder", "Core", env, new System.Collections.Generic.Dictionary<string, string>
            {
                ["ApiKey"] = apiKey,
                ["EndpointUrl"] = endpointUrl,
            }, ct);

        /// <summary>Retrieves NASA Core secrets.</summary>
        public static async Task<Result<TypedSecrets.NasaCoreSecrets?>> GetNasaCoreAsync(SecretsManager manager, SecretEnvironment env, CancellationToken ct = default)
        {
            var key = await manager.GetAsync(new SecretKey("Nasa", "Core", env, "ApiKey"), ct: ct).ConfigureAwait(false);
            if (!key.IsSuccess) return Result.Failure<TypedSecrets.NasaCoreSecrets?>(key.Reason);
            var endpoint = await manager.GetAsync(new SecretKey("Nasa", "Core", env, "EndpointUrl"), ct: ct).ConfigureAwait(false);
            if (!endpoint.IsSuccess) return Result.Failure<TypedSecrets.NasaCoreSecrets?>(endpoint.Reason);

            if (key.Data is null || endpoint.Data is null)
                return Result.Success<TypedSecrets.NasaCoreSecrets?>(null);

            return Result.Success<TypedSecrets.NasaCoreSecrets?>(
                new TypedSecrets.NasaCoreSecrets(key.Data, endpoint.Data));
        }

        /// <summary>Sets NASA Core secrets.</summary>
        public static Task<Result> SetNasaCoreAsync(SecretsManager manager, SecretEnvironment env, string apiKey, string endpointUrl, CancellationToken ct = default)
            => manager.SetupProviderServiceAsync("Nasa", "Core", env, new System.Collections.Generic.Dictionary<string, string>
            {
                ["ApiKey"] = apiKey,
                ["EndpointUrl"] = endpointUrl,
            }, ct);

        /// <summary>Retrieves OpenAI Core secrets.</summary>
        public static async Task<Result<TypedSecrets.OpenAICoreSecrets?>> GetOpenAICoreAsync(SecretsManager manager, SecretEnvironment env, CancellationToken ct = default)
        {
            var apiKey = await manager.GetAsync(new SecretKey("OpenAI", "Core", env, "ApiKey"), ct: ct).ConfigureAwait(false);
            if (!apiKey.IsSuccess) return Result.Failure<TypedSecrets.OpenAICoreSecrets?>(apiKey.Reason);
            var endpoint = await manager.GetAsync(new SecretKey("OpenAI", "Core", env, "EndpointUrl"), ct: ct).ConfigureAwait(false);
            if (!endpoint.IsSuccess) return Result.Failure<TypedSecrets.OpenAICoreSecrets?>(endpoint.Reason);
            var org = await manager.GetAsync(new SecretKey("OpenAI", "Core", env, "OrganizationId"), ct: ct).ConfigureAwait(false);
            if (!org.IsSuccess) return Result.Failure<TypedSecrets.OpenAICoreSecrets?>(org.Reason);

            if (apiKey.Data is null || endpoint.Data is null)
                return Result.Success<TypedSecrets.OpenAICoreSecrets?>(null);

            return Result.Success<TypedSecrets.OpenAICoreSecrets?>(
                new TypedSecrets.OpenAICoreSecrets(apiKey.Data, endpoint.Data, org.Data));
        }

        /// <summary>Sets OpenAI Core secrets.</summary>
        public static Task<Result> SetOpenAICoreAsync(SecretsManager manager, SecretEnvironment env, string apiKey, string endpointUrl, string? organizationId = null, CancellationToken ct = default)
        {
            var dict = new System.Collections.Generic.Dictionary<string, string>
            {
                ["ApiKey"] = apiKey,
                ["EndpointUrl"] = endpointUrl,
            };
            if (!string.IsNullOrWhiteSpace(organizationId))
                dict["OrganizationId"] = organizationId!;

            return manager.SetupProviderServiceAsync("OpenAI", "Core", env, dict, ct);
        }

        /// <summary>Retrieves Anthropic Core secrets.</summary>
        public static async Task<Result<TypedSecrets.AnthropicCoreSecrets?>> GetAnthropicCoreAsync(SecretsManager manager, SecretEnvironment env, CancellationToken ct = default)
        {
            var apiKey = await manager.GetAsync(new SecretKey("Anthropic", "Core", env, "ApiKey"), ct: ct).ConfigureAwait(false);
            if (!apiKey.IsSuccess) return Result.Failure<TypedSecrets.AnthropicCoreSecrets?>(apiKey.Reason);
            var endpoint = await manager.GetAsync(new SecretKey("Anthropic", "Core", env, "EndpointUrl"), ct: ct).ConfigureAwait(false);
            if (!endpoint.IsSuccess) return Result.Failure<TypedSecrets.AnthropicCoreSecrets?>(endpoint.Reason);

            if (apiKey.Data is null || endpoint.Data is null)
                return Result.Success<TypedSecrets.AnthropicCoreSecrets?>(null);

            return Result.Success<TypedSecrets.AnthropicCoreSecrets?>(
                new TypedSecrets.AnthropicCoreSecrets(apiKey.Data, endpoint.Data));
        }

        /// <summary>Sets Anthropic Core secrets.</summary>
        public static Task<Result> SetAnthropicCoreAsync(SecretsManager manager, SecretEnvironment env, string apiKey, string endpointUrl, CancellationToken ct = default)
            => manager.SetupProviderServiceAsync("Anthropic", "Core", env, new System.Collections.Generic.Dictionary<string, string>
            {
                ["ApiKey"] = apiKey,
                ["EndpointUrl"] = endpointUrl,
            }, ct);

        /// <summary>Retrieves DeepSeek Core secrets.</summary>
        public static async Task<Result<TypedSecrets.DeepSeekCoreSecrets?>> GetDeepSeekCoreAsync(SecretsManager manager, SecretEnvironment env, CancellationToken ct = default)
        {
            var apiKey = await manager.GetAsync(new SecretKey("DeepSeek", "Core", env, "ApiKey"), ct: ct).ConfigureAwait(false);
            if (!apiKey.IsSuccess) return Result.Failure<TypedSecrets.DeepSeekCoreSecrets?>(apiKey.Reason);
            var endpoint = await manager.GetAsync(new SecretKey("DeepSeek", "Core", env, "EndpointUrl"), ct: ct).ConfigureAwait(false);
            if (!endpoint.IsSuccess) return Result.Failure<TypedSecrets.DeepSeekCoreSecrets?>(endpoint.Reason);

            if (apiKey.Data is null || endpoint.Data is null)
                return Result.Success<TypedSecrets.DeepSeekCoreSecrets?>(null);

            return Result.Success<TypedSecrets.DeepSeekCoreSecrets?>(
                new TypedSecrets.DeepSeekCoreSecrets(apiKey.Data, endpoint.Data));
        }

        /// <summary>Sets DeepSeek Core secrets.</summary>
        public static Task<Result> SetDeepSeekCoreAsync(SecretsManager manager, SecretEnvironment env, string apiKey, string endpointUrl, CancellationToken ct = default)
            => manager.SetupProviderServiceAsync("DeepSeek", "Core", env, new System.Collections.Generic.Dictionary<string, string>
            {
                ["ApiKey"] = apiKey,
                ["EndpointUrl"] = endpointUrl,
            }, ct);

        /// <summary>Retrieves Binance Futures secrets.</summary>
        public static async Task<Result<TypedSecrets.BinanceFuturesSecrets?>> GetBinanceFuturesAsync(SecretsManager manager, SecretEnvironment env, CancellationToken ct = default)
        {
            var apiKey = await manager.GetAsync(new SecretKey("Binance", "Futures", env, "ApiKey"), ct: ct).ConfigureAwait(false);
            if (!apiKey.IsSuccess) return Result.Failure<TypedSecrets.BinanceFuturesSecrets?>(apiKey.Reason);
            var apiSecret = await manager.GetAsync(new SecretKey("Binance", "Futures", env, "ApiSecret"), ct: ct).ConfigureAwait(false);
            if (!apiSecret.IsSuccess) return Result.Failure<TypedSecrets.BinanceFuturesSecrets?>(apiSecret.Reason);
            var endpoint = await manager.GetAsync(new SecretKey("Binance", "Futures", env, "EndpointUrl"), ct: ct).ConfigureAwait(false);
            if (!endpoint.IsSuccess) return Result.Failure<TypedSecrets.BinanceFuturesSecrets?>(endpoint.Reason);

            if (apiKey.Data is null || apiSecret.Data is null || endpoint.Data is null)
                return Result.Success<TypedSecrets.BinanceFuturesSecrets?>(null);

            return Result.Success<TypedSecrets.BinanceFuturesSecrets?>(
                new TypedSecrets.BinanceFuturesSecrets(apiKey.Data, apiSecret.Data, endpoint.Data));
        }

        /// <summary>Sets Binance Futures secrets.</summary>
        public static Task<Result> SetBinanceFuturesAsync(SecretsManager manager, SecretEnvironment env, string apiKey, string apiSecret, string endpointUrl, CancellationToken ct = default)
            => manager.SetupProviderServiceAsync("Binance", "Futures", env, new System.Collections.Generic.Dictionary<string, string>
            {
                ["ApiKey"] = apiKey,
                ["ApiSecret"] = apiSecret,
                ["EndpointUrl"] = endpointUrl,
            }, ct);

        /// <summary>Retrieves Binance Spot secrets.</summary>
        public static async Task<Result<TypedSecrets.BinanceSpotSecrets?>> GetBinanceSpotAsync(SecretsManager manager, SecretEnvironment env, CancellationToken ct = default)
        {
            var apiKey = await manager.GetAsync(new SecretKey("Binance", "Spot", env, "ApiKey"), ct: ct).ConfigureAwait(false);
            if (!apiKey.IsSuccess) return Result.Failure<TypedSecrets.BinanceSpotSecrets?>(apiKey.Reason);
            var apiSecret = await manager.GetAsync(new SecretKey("Binance", "Spot", env, "ApiSecret"), ct: ct).ConfigureAwait(false);
            if (!apiSecret.IsSuccess) return Result.Failure<TypedSecrets.BinanceSpotSecrets?>(apiSecret.Reason);
            var endpoint = await manager.GetAsync(new SecretKey("Binance", "Spot", env, "EndpointUrl"), ct: ct).ConfigureAwait(false);
            if (!endpoint.IsSuccess) return Result.Failure<TypedSecrets.BinanceSpotSecrets?>(endpoint.Reason);

            if (apiKey.Data is null || apiSecret.Data is null || endpoint.Data is null)
                return Result.Success<TypedSecrets.BinanceSpotSecrets?>(null);

            return Result.Success<TypedSecrets.BinanceSpotSecrets?>(
                new TypedSecrets.BinanceSpotSecrets(apiKey.Data, apiSecret.Data, endpoint.Data));
        }

        /// <summary>Sets Binance Spot secrets.</summary>
        public static Task<Result> SetBinanceSpotAsync(SecretsManager manager, SecretEnvironment env, string apiKey, string apiSecret, string endpointUrl, CancellationToken ct = default)
            => manager.SetupProviderServiceAsync("Binance", "Spot", env, new System.Collections.Generic.Dictionary<string, string>
            {
                ["ApiKey"] = apiKey,
                ["ApiSecret"] = apiSecret,
                ["EndpointUrl"] = endpointUrl,
            }, ct);

        /// <summary>Retrieves Deribit Core secrets.</summary>
        public static async Task<Result<TypedSecrets.DeribitCoreSecrets?>> GetDeribitCoreAsync(SecretsManager manager, SecretEnvironment env, CancellationToken ct = default)
        {
            var id = await manager.GetAsync(new SecretKey("Deribit", "Core", env, "ClientId"), ct: ct).ConfigureAwait(false);
            if (!id.IsSuccess) return Result.Failure<TypedSecrets.DeribitCoreSecrets?>(id.Reason);
            var secret = await manager.GetAsync(new SecretKey("Deribit", "Core", env, "ClientSecret"), ct: ct).ConfigureAwait(false);
            if (!secret.IsSuccess) return Result.Failure<TypedSecrets.DeribitCoreSecrets?>(secret.Reason);
            var endpoint = await manager.GetAsync(new SecretKey("Deribit", "Core", env, "EndpointUrl"), ct: ct).ConfigureAwait(false);
            if (!endpoint.IsSuccess) return Result.Failure<TypedSecrets.DeribitCoreSecrets?>(endpoint.Reason);

            if (id.Data is null || secret.Data is null || endpoint.Data is null)
                return Result.Success<TypedSecrets.DeribitCoreSecrets?>(null);

            return Result.Success<TypedSecrets.DeribitCoreSecrets?>(
                new TypedSecrets.DeribitCoreSecrets(id.Data, secret.Data, endpoint.Data));
        }

        /// <summary>Sets Deribit Core secrets.</summary>
        public static Task<Result> SetDeribitCoreAsync(SecretsManager manager, SecretEnvironment env, string clientId, string clientSecret, string endpointUrl, CancellationToken ct = default)
            => manager.SetupProviderServiceAsync("Deribit", "Core", env, new System.Collections.Generic.Dictionary<string, string>
            {
                ["ClientId"] = clientId,
                ["ClientSecret"] = clientSecret,
                ["EndpointUrl"] = endpointUrl,
            }, ct);

        
        /// <summary>Gets Azure OpenAI Chat Completions deployment secrets.</summary>
        public static async Task<Result<TypedSecrets.AzureOpenAIChatCompletionsSecrets?>> GetAzureOpenAIChatAsync(SecretsManager manager, SecretEnvironment env, CancellationToken ct = default)
        {
            var key = await manager.GetAsync(new SecretKey("AzureOpenAI", "ChatCompletions", env, "ApiKey"), ct: ct).ConfigureAwait(false);
            if (!key.IsSuccess) return Result.Failure<TypedSecrets.AzureOpenAIChatCompletionsSecrets?>(key.Reason);
            var endpoint = await manager.GetAsync(new SecretKey("AzureOpenAI", "ChatCompletions", env, "EndpointUrl"), ct: ct).ConfigureAwait(false);
            if (!endpoint.IsSuccess) return Result.Failure<TypedSecrets.AzureOpenAIChatCompletionsSecrets?>(endpoint.Reason);
            var dep = await manager.GetAsync(new SecretKey("AzureOpenAI", "ChatCompletions", env, "DeploymentName"), ct: ct).ConfigureAwait(false);
            if (!dep.IsSuccess) return Result.Failure<TypedSecrets.AzureOpenAIChatCompletionsSecrets?>(dep.Reason);
            var ver = await manager.GetAsync(new SecretKey("AzureOpenAI", "ChatCompletions", env, "ApiVersion"), ct: ct).ConfigureAwait(false);
            if (!ver.IsSuccess) return Result.Failure<TypedSecrets.AzureOpenAIChatCompletionsSecrets?>(ver.Reason);

            if (key.Data is null || endpoint.Data is null || dep.Data is null || ver.Data is null)
                return Result.Success<TypedSecrets.AzureOpenAIChatCompletionsSecrets?>(null);

            return Result.Success<TypedSecrets.AzureOpenAIChatCompletionsSecrets?>(
                new TypedSecrets.AzureOpenAIChatCompletionsSecrets(key.Data, endpoint.Data, dep.Data, ver.Data));
        }

        /// <summary>Sets Azure OpenAI Chat Completions deployment secrets.</summary>
        public static Task<Result> SetAzureOpenAIChatAsync(SecretsManager manager, SecretEnvironment env, string apiKey, string endpointUrl, string deploymentName, string apiVersion, CancellationToken ct = default)
            => manager.SetupProviderServiceAsync("AzureOpenAI", "ChatCompletions", env, new System.Collections.Generic.Dictionary<string, string>
            {
                ["ApiKey"] = apiKey,
                ["EndpointUrl"] = endpointUrl,
                ["DeploymentName"] = deploymentName,
                ["ApiVersion"] = apiVersion,
            }, ct);

        /// <summary>Gets Azure OpenAI Embeddings deployment secrets.</summary>
        public static async Task<Result<TypedSecrets.AzureOpenAIEmbeddingsSecrets?>> GetAzureOpenAIEmbeddingsAsync(SecretsManager manager, SecretEnvironment env, CancellationToken ct = default)
        {
            var key = await manager.GetAsync(new SecretKey("AzureOpenAI", "Embeddings", env, "ApiKey"), ct: ct).ConfigureAwait(false);
            if (!key.IsSuccess) return Result.Failure<TypedSecrets.AzureOpenAIEmbeddingsSecrets?>(key.Reason);
            var endpoint = await manager.GetAsync(new SecretKey("AzureOpenAI", "Embeddings", env, "EndpointUrl"), ct: ct).ConfigureAwait(false);
            if (!endpoint.IsSuccess) return Result.Failure<TypedSecrets.AzureOpenAIEmbeddingsSecrets?>(endpoint.Reason);
            var dep = await manager.GetAsync(new SecretKey("AzureOpenAI", "Embeddings", env, "DeploymentName"), ct: ct).ConfigureAwait(false);
            if (!dep.IsSuccess) return Result.Failure<TypedSecrets.AzureOpenAIEmbeddingsSecrets?>(dep.Reason);
            var ver = await manager.GetAsync(new SecretKey("AzureOpenAI", "Embeddings", env, "ApiVersion"), ct: ct).ConfigureAwait(false);
            if (!ver.IsSuccess) return Result.Failure<TypedSecrets.AzureOpenAIEmbeddingsSecrets?>(ver.Reason);

            if (key.Data is null || endpoint.Data is null || dep.Data is null || ver.Data is null)
                return Result.Success<TypedSecrets.AzureOpenAIEmbeddingsSecrets?>(null);

            return Result.Success<TypedSecrets.AzureOpenAIEmbeddingsSecrets?>(
                new TypedSecrets.AzureOpenAIEmbeddingsSecrets(key.Data, endpoint.Data, dep.Data, ver.Data));
        }

        /// <summary>Sets Azure OpenAI Embeddings deployment secrets.</summary>
        public static Task<Result> SetAzureOpenAIEmbeddingsAsync(SecretsManager manager, SecretEnvironment env, string apiKey, string endpointUrl, string deploymentName, string apiVersion, CancellationToken ct = default)
            => manager.SetupProviderServiceAsync("AzureOpenAI", "Embeddings", env, new System.Collections.Generic.Dictionary<string, string>
            {
                ["ApiKey"] = apiKey,
                ["EndpointUrl"] = endpointUrl,
                ["DeploymentName"] = deploymentName,
                ["ApiVersion"] = apiVersion,
            }, ct);

        // ----------------------- Trading / REST -----------------------

        /// <summary>Gets Trading REST secrets.</summary>
        public static async Task<Result<TypedSecrets.TradingRestSecrets?>> GetTradingRestAsync(SecretsManager manager, SecretEnvironment env, CancellationToken ct = default)
        {
            var k = await manager.GetAsync(new SecretKey("Trading", "REST", env, "ApiKey"), ct: ct).ConfigureAwait(false);
            if (!k.IsSuccess) return Result.Failure<TypedSecrets.TradingRestSecrets?>(k.Reason);
            var s = await manager.GetAsync(new SecretKey("Trading", "REST", env, "ApiSecret"), ct: ct).ConfigureAwait(false);
            if (!s.IsSuccess) return Result.Failure<TypedSecrets.TradingRestSecrets?>(s.Reason);
            var e = await manager.GetAsync(new SecretKey("Trading", "REST", env, "EndpointUrl"), ct: ct).ConfigureAwait(false);
            if (!e.IsSuccess) return Result.Failure<TypedSecrets.TradingRestSecrets?>(e.Reason);

            if (k.Data is null || s.Data is null || e.Data is null)
                return Result.Success<TypedSecrets.TradingRestSecrets?>(null);

            return Result.Success<TypedSecrets.TradingRestSecrets?>(
                new TypedSecrets.TradingRestSecrets(k.Data, s.Data, e.Data));
        }

        /// <summary>Sets Trading REST secrets.</summary>
        public static Task<Result> SetTradingRestAsync(SecretsManager manager, SecretEnvironment env, string apiKey, string apiSecret, string endpointUrl, CancellationToken ct = default)
            => manager.SetupProviderServiceAsync("Trading", "REST", env, new System.Collections.Generic.Dictionary<string, string>
            {
                ["ApiKey"] = apiKey,
                ["ApiSecret"] = apiSecret,
                ["EndpointUrl"] = endpointUrl,
            }, ct);

        // ----------------------- HuggingFace -----------------------

        /// <summary>Gets HuggingFace Inference API secrets.</summary>
        public static async Task<Result<TypedSecrets.HuggingFaceInferenceSecrets?>> GetHuggingFaceInferenceAsync(SecretsManager manager, SecretEnvironment env, CancellationToken ct = default)
        {
            var t = await manager.GetAsync(new SecretKey("HuggingFace", "InferenceApi", env, "ApiToken"), ct: ct).ConfigureAwait(false);
            if (!t.IsSuccess) return Result.Failure<TypedSecrets.HuggingFaceInferenceSecrets?>(t.Reason);
            var e = await manager.GetAsync(new SecretKey("HuggingFace", "InferenceApi", env, "EndpointUrl"), ct: ct).ConfigureAwait(false);
            if (!e.IsSuccess) return Result.Failure<TypedSecrets.HuggingFaceInferenceSecrets?>(e.Reason);

            if (t.Data is null || e.Data is null)
                return Result.Success<TypedSecrets.HuggingFaceInferenceSecrets?>(null);

            return Result.Success<TypedSecrets.HuggingFaceInferenceSecrets?>(
                new TypedSecrets.HuggingFaceInferenceSecrets(t.Data, e.Data));
        }

        /// <summary>Sets HuggingFace Inference API secrets.</summary>
        public static Task<Result> SetHuggingFaceInferenceAsync(SecretsManager manager, SecretEnvironment env, string apiToken, string endpointUrl, CancellationToken ct = default)
            => manager.SetupProviderServiceAsync("HuggingFace", "InferenceApi", env, new System.Collections.Generic.Dictionary<string, string>
            {
                ["ApiToken"] = apiToken,
                ["EndpointUrl"] = endpointUrl,
            }, ct);

        /// <summary>Gets HuggingFace Spaces secrets.</summary>
        public static async Task<Result<TypedSecrets.HuggingFaceSpacesSecrets?>> GetHuggingFaceSpacesAsync(SecretsManager manager, SecretEnvironment env, CancellationToken ct = default)
        {
            var t = await manager.GetAsync(new SecretKey("HuggingFace", "Spaces", env, "ApiToken"), ct: ct).ConfigureAwait(false);
            if (!t.IsSuccess) return Result.Failure<TypedSecrets.HuggingFaceSpacesSecrets?>(t.Reason);
            var e = await manager.GetAsync(new SecretKey("HuggingFace", "Spaces", env, "EndpointUrl"), ct: ct).ConfigureAwait(false);
            if (!e.IsSuccess) return Result.Failure<TypedSecrets.HuggingFaceSpacesSecrets?>(e.Reason);

            if (t.Data is null || e.Data is null)
                return Result.Success<TypedSecrets.HuggingFaceSpacesSecrets?>(null);

            return Result.Success<TypedSecrets.HuggingFaceSpacesSecrets?>(
                new TypedSecrets.HuggingFaceSpacesSecrets(t.Data, e.Data));
        }

        /// <summary>Sets HuggingFace Spaces secrets.</summary>
        public static Task<Result> SetHuggingFaceSpacesAsync(SecretsManager manager, SecretEnvironment env, string apiToken, string endpointUrl, CancellationToken ct = default)
            => manager.SetupProviderServiceAsync("HuggingFace", "Spaces", env, new System.Collections.Generic.Dictionary<string, string>
            {
                ["ApiToken"] = apiToken,
                ["EndpointUrl"] = endpointUrl,
            }, ct);

        // ----------------------- Facebook -----------------------

        /// <summary>Gets Facebook Graph API secrets.</summary>
        public static async Task<Result<TypedSecrets.FacebookGraphSecrets?>> GetFacebookGraphAsync(SecretsManager manager, SecretEnvironment env, CancellationToken ct = default)
        {
            var id = await manager.GetAsync(new SecretKey("Facebook", "GraphApi", env, "AppId"), ct: ct).ConfigureAwait(false);
            if (!id.IsSuccess) return Result.Failure<TypedSecrets.FacebookGraphSecrets?>(id.Reason);
            var sec = await manager.GetAsync(new SecretKey("Facebook", "GraphApi", env, "AppSecret"), ct: ct).ConfigureAwait(false);
            if (!sec.IsSuccess) return Result.Failure<TypedSecrets.FacebookGraphSecrets?>(sec.Reason);
            var tok = await manager.GetAsync(new SecretKey("Facebook", "GraphApi", env, "AccessToken"), ct: ct).ConfigureAwait(false);
            if (!tok.IsSuccess) return Result.Failure<TypedSecrets.FacebookGraphSecrets?>(tok.Reason);
            var ep = await manager.GetAsync(new SecretKey("Facebook", "GraphApi", env, "EndpointUrl"), ct: ct).ConfigureAwait(false);
            if (!ep.IsSuccess) return Result.Failure<TypedSecrets.FacebookGraphSecrets?>(ep.Reason);

            if (id.Data is null || sec.Data is null || tok.Data is null || ep.Data is null)
                return Result.Success<TypedSecrets.FacebookGraphSecrets?>(null);

            return Result.Success<TypedSecrets.FacebookGraphSecrets?>(
                new TypedSecrets.FacebookGraphSecrets(id.Data, sec.Data, tok.Data, ep.Data));
        }

        /// <summary>Sets Facebook Graph API secrets.</summary>
        public static Task<Result> SetFacebookGraphAsync(SecretsManager manager, SecretEnvironment env, string appId, string appSecret, string accessToken, string endpointUrl, CancellationToken ct = default)
            => manager.SetupProviderServiceAsync("Facebook", "GraphApi", env, new System.Collections.Generic.Dictionary<string, string>
            {
                ["AppId"] = appId,
                ["AppSecret"] = appSecret,
                ["AccessToken"] = accessToken,
                ["EndpointUrl"] = endpointUrl,
            }, ct);

        // ----------------------- X (Twitter) -----------------------

        /// <summary>Gets X (Twitter) API v2 secrets.</summary>
        public static async Task<Result<TypedSecrets.XApiV2Secrets?>> GetXApiV2Async(SecretsManager manager, SecretEnvironment env, CancellationToken ct = default)
        {
            var k = await manager.GetAsync(new SecretKey("X", "ApiV2", env, "ApiKey"), ct: ct).ConfigureAwait(false);
            if (!k.IsSuccess) return Result.Failure<TypedSecrets.XApiV2Secrets?>(k.Reason);
            var s = await manager.GetAsync(new SecretKey("X", "ApiV2", env, "ApiSecret"), ct: ct).ConfigureAwait(false);
            if (!s.IsSuccess) return Result.Failure<TypedSecrets.XApiV2Secrets?>(s.Reason);
            var b = await manager.GetAsync(new SecretKey("X", "ApiV2", env, "BearerToken"), ct: ct).ConfigureAwait(false);
            if (!b.IsSuccess) return Result.Failure<TypedSecrets.XApiV2Secrets?>(b.Reason);
            var cid = await manager.GetAsync(new SecretKey("X", "ApiV2", env, "ClientId"), ct: ct).ConfigureAwait(false);
            if (!cid.IsSuccess) return Result.Failure<TypedSecrets.XApiV2Secrets?>(cid.Reason);
            var cs = await manager.GetAsync(new SecretKey("X", "ApiV2", env, "ClientSecret"), ct: ct).ConfigureAwait(false);
            if (!cs.IsSuccess) return Result.Failure<TypedSecrets.XApiV2Secrets?>(cs.Reason);
            var ep = await manager.GetAsync(new SecretKey("X", "ApiV2", env, "EndpointUrl"), ct: ct).ConfigureAwait(false);
            if (!ep.IsSuccess) return Result.Failure<TypedSecrets.XApiV2Secrets?>(ep.Reason);

            if (k.Data is null or s.Data is null or b.Data is null or cid.Data is null or cs.Data is null or ep.Data is null)
                return Result.Success<TypedSecrets.XApiV2Secrets?>(null);

            return Result.Success<TypedSecrets.XApiV2Secrets?>(
                new TypedSecrets.XApiV2Secrets(k.Data, s.Data, b.Data, cid.Data, cs.Data, ep.Data));
        }

        /// <summary>Sets X (Twitter) API v2 secrets.</summary>
        public static Task<Result> SetXApiV2Async(SecretsManager manager, SecretEnvironment env, string apiKey, string apiSecret, string bearerToken, string clientId, string clientSecret, string endpointUrl, CancellationToken ct = default)
            => manager.SetupProviderServiceAsync("X", "ApiV2", env, new System.Collections.Generic.Dictionary<string, string>
            {
                ["ApiKey"] = apiKey,
                ["ApiSecret"] = apiSecret,
                ["BearerToken"] = bearerToken,
                ["ClientId"] = clientId,
                ["ClientSecret"] = clientSecret,
                ["EndpointUrl"] = endpointUrl,
            }, ct);

        // ----------------------- Instagram -----------------------

        /// <summary>Gets Instagram Graph API secrets.</summary>
        public static async Task<Result<TypedSecrets.InstagramGraphSecrets?>> GetInstagramGraphAsync(SecretsManager manager, SecretEnvironment env, CancellationToken ct = default)
        {
            var id = await manager.GetAsync(new SecretKey("Instagram", "GraphApi", env, "AppId"), ct: ct).ConfigureAwait(false);
            if (!id.IsSuccess) return Result.Failure<TypedSecrets.InstagramGraphSecrets?>(id.Reason);
            var sec = await manager.GetAsync(new SecretKey("Instagram", "GraphApi", env, "AppSecret"), ct: ct).ConfigureAwait(false);
            if (!sec.IsSuccess) return Result.Failure<TypedSecrets.InstagramGraphSecrets?>(sec.Reason);
            var tok = await manager.GetAsync(new SecretKey("Instagram", "GraphApi", env, "AccessToken"), ct: ct).ConfigureAwait(false);
            if (!tok.IsSuccess) return Result.Failure<TypedSecrets.InstagramGraphSecrets?>(tok.Reason);
            var ep = await manager.GetAsync(new SecretKey("Instagram", "GraphApi", env, "EndpointUrl"), ct: ct).ConfigureAwait(false);
            if (!ep.IsSuccess) return Result.Failure<TypedSecrets.InstagramGraphSecrets?>(ep.Reason);

            if (id.Data is null || sec.Data is null || tok.Data is null || ep.Data is null)
                return Result.Success<TypedSecrets.InstagramGraphSecrets?>(null);

            return Result.Success<TypedSecrets.InstagramGraphSecrets?>(
                new TypedSecrets.InstagramGraphSecrets(id.Data, sec.Data, tok.Data, ep.Data));
        }

        /// <summary>Sets Instagram Graph API secrets.</summary>
        public static Task<Result> SetInstagramGraphAsync(SecretsManager manager, SecretEnvironment env, string appId, string appSecret, string accessToken, string endpointUrl, CancellationToken ct = default)
            => manager.SetupProviderServiceAsync("Instagram", "GraphApi", env, new System.Collections.Generic.Dictionary<string, string>
            {
                ["AppId"] = appId,
                ["AppSecret"] = appSecret,
                ["AccessToken"] = accessToken,
                ["EndpointUrl"] = endpointUrl,
            }, ct);

        // ----------------------- Cloudflare -----------------------

        /// <summary>Gets Cloudflare API secrets.</summary>
        public static async Task<Result<TypedSecrets.CloudflareApiSecrets?>> GetCloudflareApiAsync(SecretsManager manager, SecretEnvironment env, CancellationToken ct = default)
        {
            var t = await manager.GetAsync(new SecretKey("Cloudflare", "Api", env, "ApiToken"), ct: ct).ConfigureAwait(false);
            if (!t.IsSuccess) return Result.Failure<TypedSecrets.CloudflareApiSecrets?>(t.Reason);
            var a = await manager.GetAsync(new SecretKey("Cloudflare", "Api", env, "AccountId"), ct: ct).ConfigureAwait(false);
            if (!a.IsSuccess) return Result.Failure<TypedSecrets.CloudflareApiSecrets?>(a.Reason);
            var e = await manager.GetAsync(new SecretKey("Cloudflare", "Api", env, "EndpointUrl"), ct: ct).ConfigureAwait(false);
            if (!e.IsSuccess) return Result.Failure<TypedSecrets.CloudflareApiSecrets?>(e.Reason);

            if (t.Data is null || a.Data is null || e.Data is null)
                return Result.Success<TypedSecrets.CloudflareApiSecrets?>(null);

            return Result.Success<TypedSecrets.CloudflareApiSecrets?>(
                new TypedSecrets.CloudflareApiSecrets(t.Data, a.Data, e.Data));
        }

        /// <summary>Sets Cloudflare API secrets.</summary>
        public static Task<Result> SetCloudflareApiAsync(SecretsManager manager, SecretEnvironment env, string apiToken, string accountId, string endpointUrl, CancellationToken ct = default)
            => manager.SetupProviderServiceAsync("Cloudflare", "Api", env, new System.Collections.Generic.Dictionary<string, string>
            {
                ["ApiToken"] = apiToken,
                ["AccountId"] = accountId,
                ["EndpointUrl"] = endpointUrl,
            }, ct);

        /// <summary>Gets Cloudflare DNS secrets.</summary>
        public static async Task<Result<TypedSecrets.CloudflareDnsSecrets?>> GetCloudflareDnsAsync(SecretsManager manager, SecretEnvironment env, CancellationToken ct = default)
        {
            var t = await manager.GetAsync(new SecretKey("Cloudflare", "DNS", env, "ApiToken"), ct: ct).ConfigureAwait(false);
            if (!t.IsSuccess) return Result.Failure<TypedSecrets.CloudflareDnsSecrets?>(t.Reason);
            var a = await manager.GetAsync(new SecretKey("Cloudflare", "DNS", env, "AccountId"), ct: ct).ConfigureAwait(false);
            if (!a.IsSuccess) return Result.Failure<TypedSecrets.CloudflareDnsSecrets?>(a.Reason);
            var z = await manager.GetAsync(new SecretKey("Cloudflare", "DNS", env, "ZoneId"), ct: ct).ConfigureAwait(false);
            if (!z.IsSuccess) return Result.Failure<TypedSecrets.CloudflareDnsSecrets?>(z.Reason);
            var e = await manager.GetAsync(new SecretKey("Cloudflare", "DNS", env, "EndpointUrl"), ct: ct).ConfigureAwait(false);
            if (!e.IsSuccess) return Result.Failure<TypedSecrets.CloudflareDnsSecrets?>(e.Reason);

            if (t.Data is null || a.Data is null || z.Data is null || e.Data is null)
                return Result.Success<TypedSecrets.CloudflareDnsSecrets?>(null);

            return Result.Success<TypedSecrets.CloudflareDnsSecrets?>(
                new TypedSecrets.CloudflareDnsSecrets(t.Data, a.Data, z.Data, e.Data));
        }

        /// <summary>Sets Cloudflare DNS secrets.</summary>
        public static Task<Result> SetCloudflareDnsAsync(SecretsManager manager, SecretEnvironment env, string apiToken, string accountId, string zoneId, string endpointUrl, CancellationToken ct = default)
            => manager.SetupProviderServiceAsync("Cloudflare", "DNS", env, new System.Collections.Generic.Dictionary<string, string>
            {
                ["ApiToken"] = apiToken,
                ["AccountId"] = accountId,
                ["ZoneId"] = zoneId,
                ["EndpointUrl"] = endpointUrl,
            }, ct);

        /// <summary>Gets Cloudflare Workers secrets.</summary>
        public static async Task<Result<TypedSecrets.CloudflareWorkersSecrets?>> GetCloudflareWorkersAsync(SecretsManager manager, SecretEnvironment env, CancellationToken ct = default)
        {
            var t = await manager.GetAsync(new SecretKey("Cloudflare", "Workers", env, "ApiToken"), ct: ct).ConfigureAwait(false);
            if (!t.IsSuccess) return Result.Failure<TypedSecrets.CloudflareWorkersSecrets?>(t.Reason);
            var a = await manager.GetAsync(new SecretKey("Cloudflare", "Workers", env, "AccountId"), ct: ct).ConfigureAwait(false);
            if (!a.IsSuccess) return Result.Failure<TypedSecrets.CloudflareWorkersSecrets?>(a.Reason);
            var e = await manager.GetAsync(new SecretKey("Cloudflare", "Workers", env, "EndpointUrl"), ct: ct).ConfigureAwait(false);
            if (!e.IsSuccess) return Result.Failure<TypedSecrets.CloudflareWorkersSecrets?>(e.Reason);

            if (t.Data is null || a.Data is null || e.Data is null)
                return Result.Success<TypedSecrets.CloudflareWorkersSecrets?>(null);

            return Result.Success<TypedSecrets.CloudflareWorkersSecrets?>(
                new TypedSecrets.CloudflareWorkersSecrets(t.Data, a.Data, e.Data));
        }

        /// <summary>Sets Cloudflare Workers secrets.</summary>
        public static Task<Result> SetCloudflareWorkersAsync(SecretsManager manager, SecretEnvironment env, string apiToken, string accountId, string endpointUrl, CancellationToken ct = default)
            => manager.SetupProviderServiceAsync("Cloudflare", "Workers", env, new System.Collections.Generic.Dictionary<string, string>
            {
                ["ApiToken"] = apiToken,
                ["AccountId"] = accountId,
                ["EndpointUrl"] = endpointUrl,
            }, ct);

        /// <summary>Gets Cloudflare R2 secrets.</summary>
        public static async Task<Result<TypedSecrets.CloudflareR2Secrets?>> GetCloudflareR2Async(SecretsManager manager, SecretEnvironment env, CancellationToken ct = default)
        {
            var ak = await manager.GetAsync(new SecretKey("Cloudflare", "R2", env, "AccessKeyId"), ct: ct).ConfigureAwait(false);
            if (!ak.IsSuccess) return Result.Failure<TypedSecrets.CloudflareR2Secrets?>(ak.Reason);
            var sk = await manager.GetAsync(new SecretKey("Cloudflare", "R2", env, "SecretAccessKey"), ct: ct).ConfigureAwait(false);
            if (!sk.IsSuccess) return Result.Failure<TypedSecrets.CloudflareR2Secrets?>(sk.Reason);
            var acc = await manager.GetAsync(new SecretKey("Cloudflare", "R2", env, "AccountId"), ct: ct).ConfigureAwait(false);
            if (!acc.IsSuccess) return Result.Failure<TypedSecrets.CloudflareR2Secrets?>(acc.Reason);
            var ep = await manager.GetAsync(new SecretKey("Cloudflare", "R2", env, "EndpointUrl"), ct: ct).ConfigureAwait(false);
            if (!ep.IsSuccess) return Result.Failure<TypedSecrets.CloudflareR2Secrets?>(ep.Reason);
            var b = await manager.GetAsync(new SecretKey("Cloudflare", "R2", env, "Bucket"), ct: ct).ConfigureAwait(false);
            if (!b.IsSuccess) return Result.Failure<TypedSecrets.CloudflareR2Secrets?>(b.Reason);

            if (ak.Data is null || sk.Data is null || acc.Data is null || ep.Data is null || b.Data is null)
                return Result.Success<TypedSecrets.CloudflareR2Secrets?>(null);

            return Result.Success<TypedSecrets.CloudflareR2Secrets?>(
                new TypedSecrets.CloudflareR2Secrets(ak.Data, sk.Data, acc.Data, ep.Data, b.Data));
        }

        /// <summary>Sets Cloudflare R2 secrets.</summary>
        public static Task<Result> SetCloudflareR2Async(SecretsManager manager, SecretEnvironment env, string accessKeyId, string secretAccessKey, string accountId, string endpointUrl, string bucket, CancellationToken ct = default)
            => manager.SetupProviderServiceAsync("Cloudflare", "R2", env, new System.Collections.Generic.Dictionary<string, string>
            {
                ["AccessKeyId"] = accessKeyId,
                ["SecretAccessKey"] = secretAccessKey,
                ["AccountId"] = accountId,
                ["EndpointUrl"] = endpointUrl,
                ["Bucket"] = bucket,
            }, ct);

        // ----------------------- RapidAPI -----------------------

        /// <summary>Gets RapidAPI Core secrets.</summary>
        public static async Task<Result<TypedSecrets.RapidApiCoreSecrets?>> GetRapidApiCoreAsync(SecretsManager manager, SecretEnvironment env, CancellationToken ct = default)
        {
            var k = await manager.GetAsync(new SecretKey("RapidAPI", "Core", env, "ApiKey"), ct: ct).ConfigureAwait(false);
            if (!k.IsSuccess) return Result.Failure<TypedSecrets.RapidApiCoreSecrets?>(k.Reason);
            var e = await manager.GetAsync(new SecretKey("RapidAPI", "Core", env, "EndpointUrl"), ct: ct).ConfigureAwait(false);
            if (!e.IsSuccess) return Result.Failure<TypedSecrets.RapidApiCoreSecrets?>(e.Reason);

            if (k.Data is null || e.Data is null)
                return Result.Success<TypedSecrets.RapidApiCoreSecrets?>(null);

            return Result.Success<TypedSecrets.RapidApiCoreSecrets?>(
                new TypedSecrets.RapidApiCoreSecrets(k.Data, e.Data));
        }

        /// <summary>Sets RapidAPI Core secrets.</summary>
        public static Task<Result> SetRapidApiCoreAsync(SecretsManager manager, SecretEnvironment env, string apiKey, string endpointUrl, CancellationToken ct = default)
            => manager.SetupProviderServiceAsync("RapidAPI", "Core", env, new System.Collections.Generic.Dictionary<string, string>
            {
                ["ApiKey"] = apiKey,
                ["EndpointUrl"] = endpointUrl,
            }, ct);
    }
}
