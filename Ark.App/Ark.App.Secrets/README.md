# Ark.App.Secrets

This delta adds **new providers and typed helpers** while keeping the existing API unchanged.

## Providers added

- **Azure OpenAI**: `ChatCompletions`, `Embeddings`
  - Secrets: `ApiKey`, `EndpointUrl`, `DeploymentName`, `ApiVersion`
- **Trading / REST**: `REST`
  - Secrets: `ApiKey`, `ApiSecret`, `EndpointUrl`
- **HuggingFace**: `InferenceApi`, `Spaces`
  - Secrets: `ApiToken`, `EndpointUrl`
- **Facebook**: `GraphApi`
  - Secrets: `AppId`, `AppSecret`, `AccessToken`, `EndpointUrl`
- **X (Twitter)**: `ApiV2`
  - Secrets: `ApiKey`, `ApiSecret`, `BearerToken`, `ClientId`, `ClientSecret`, `EndpointUrl`
- **Instagram**: `GraphApi`
  - Secrets: `AppId`, `AppSecret`, `AccessToken`, `EndpointUrl`
- **Cloudflare**: `Api`, `DNS`, `Workers`, `R2`
  - `Api`: `ApiToken`, `AccountId`, `EndpointUrl`
  - `DNS`: `ApiToken`, `AccountId`, `ZoneId`, `EndpointUrl`
  - `Workers`: `ApiToken`, `AccountId`, `EndpointUrl`
  - `R2`: `AccessKeyId`, `SecretAccessKey`, `AccountId`, `EndpointUrl`, `Bucket`
- **RapidAPI**: `Core`
  - Secrets: `ApiKey`, `EndpointUrl`

## Usage (examples)

```csharp
// Azure OpenAI (Sandbox)
await SecretsTypedHelper.SetAzureOpenAIChatAsync(mgr, SecretEnvironment.Sandbox,
    apiKey: "<KEY>", endpointUrl: "https://<resource>.openai.azure.com",
    deploymentName: "gpt-4o-mini", apiVersion: "2024-06-01");

var chat = await SecretsTypedHelper.GetAzureOpenAIChatAsync(mgr, SecretEnvironment.Sandbox);
```

```csharp
// Trading REST (Prod)
await SecretsTypedHelper.SetTradingRestAsync(mgr, SecretEnvironment.Production,
    apiKey: "<KEY>", apiSecret: "<SECRET>", endpointUrl: "https://api.broker.tld/v1");

var trading = await SecretsTypedHelper.GetTradingRestAsync(mgr, SecretEnvironment.Production);
```

```csharp
// Cloudflare R2 (Dev)
await SecretsTypedHelper.SetCloudflareR2Async(mgr, SecretEnvironment.Development,
    accessKeyId: "<AK>", secretAccessKey: "<SK>", accountId: "<ACC>",
    endpointUrl: "https://<accountid>.r2.cloudflarestorage.com", bucket: "backups");
```

## Test/Sandbox environments

This library treats environments via `SecretEnvironment` and **you choose the appropriate test endpoints**:

- **Azure OpenAI**: same endpoint; use a dedicated resource/subscription for sandbox.
- **HuggingFace**: same endpoint; use non-prod tokens.
- **Facebook / Instagram**: use **Test App** or app in **Development mode**, tokens are different from production.
- **X (Twitter)**: same base endpoint; use non-prod app credentials.
- **Cloudflare**: same endpoints; create tokens with limited scopes in a test account.
- **RapidAPI**: same endpoint; use a non-prod API key/project.
- **Binance / Deribit (already added in previous delta)**: set their official **testnet** endpoints when calling `Set*Async`.

> Nothing else to change in DI: `SecretsManager` continues to handle caching, backends, and the portable index fallback.
