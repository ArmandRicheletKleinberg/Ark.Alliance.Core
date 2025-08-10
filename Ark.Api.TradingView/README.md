# Ark.Api.TradingView

`Ark.Api.TradingView` is a .NET 9 client library that wraps the public TradingView API for use by other Ark Alliance components. It offers simple methods to query symbol search results, real-time quotes and historical data.

## Architecture

- **TradingViewClient**: low level HTTP client encapsulating requests to TradingView.
- **Models**: POCOs representing search and quote responses.
- **Controllers**: this project does not expose HTTP endpoints; see `Ark.Api.TradingView.Back` for a web wrapper.

```
Ark.Api.TradingView/
├── Controllers/        # none in this project
├── Models/             # DTOs used by the client
├── TradingViewClient.cs
└── LICENSE.txt
```

## Task List
| # | Task | Prompt (System / User) | Status | Remarks & Remaining Work |
|---|------|-----------------------|--------|-------------------------|

## Usage

```csharp
var client = new TradingViewClient();
var symbols = await client.SearchAsync("BTCUSDT");
```

## Codex Prompts
```
Review this README and the solution README before modifying the TradingView client library.
Run `dotnet format`, `dotnet build` and `dotnet test` after changes and report any failures.
```

### Codex Negative Prompt
```
Do not invent TradingView endpoints or alter client behaviour beyond documented features.
Avoid committing generated artifacts or credentials and keep TODO comments until resolved.
```
