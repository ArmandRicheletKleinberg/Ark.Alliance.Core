# Code Structure Analysis

**Generated on:** 2025-08-09 20:49:54
**Root Directory:** `C:\Users\ARKle\source\repos\Ark.Alliance.Trading\Ark.Alliance.Trading.Shared`
**Comment Processing:** Comments Preserved
**Markdown Included:** README.md only
**JavaScript/JSX:** Included
**Analysis Tool:** Code Structure and Content Analyzer (v3)

---

## 📊 Project Statistics

- **Total Directories (with files):** 6
- **Total Included Files:** 22
- **Total Size:** 22.7 KB

### File Type Breakdown

- **.cs**: 20 files
- **.csproj**: 1 files
- **.md**: 1 files

---

## 📁 Directory Structure (Included Files)

```
Ark.Alliance.Trading.Shared/
├── Enums/
│   ├── AnalysisRecommendation.cs (803 B)
│   ├── BotStatus.cs (677 B)
│   ├── CircuitState.cs (609 B)
│   └── RiskLevel.cs (444 B)
├── Models/
│   ├── Dtos/
│   │   ├── AnalysisSummaryDto.cs (1.2 KB)
│   │   ├── BotOptimizationStatsDto.cs (1.0 KB)
│   │   ├── CircuitBreakerStatsDto.cs (1.8 KB)
│   │   ├── RegisterRequestDto.cs (810 B)
│   │   ├── RiskAssessmentDto.cs (1.8 KB)
│   │   └── SessionInfoDto.cs (1.3 KB)
│   ├── Requests/
│   │   ├── ApiKeyLoginRequest.cs (504 B)
│   │   ├── LoginRequest.cs (615 B)
│   │   ├── MarginRequirementRequest.cs (1.4 KB)
│   │   ├── StartBotRequest.cs (922 B)
│   │   └── StopBotRequest.cs (615 B)
│   ├── Responses/
│   │   ├── LoginResponse.cs (806 B)
│   │   ├── RegisterResponse.cs (840 B)
│   │   └── StartBotResponse.cs (1.0 KB)
│   ├── TickerHistoryDto.cs (1.0 KB)
│   └── TradingSettings.cs (4.0 KB)
├── Ark.Alliance.Trading.Shared.csproj (320 B)
└── README.md (464 B)
```

---

## 💻 File Contents

Complete content of all included files (with comments):

### Root Directory

**Full Path:** `C:\Users\ARKle\source\repos\Ark.Alliance.Trading\Ark.Alliance.Trading.Shared`
**Files:** 2

#### 💾 Ark.Alliance.Trading.Shared.csproj

**File Path:** `Ark.Alliance.Trading.Shared.csproj`
**File Type:** .CSPROJ (xml)

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="..\Ark.Api.Binance\Ark.Api.Binance.csproj" />
  </ItemGroup>
</Project>

```

---

#### 💾 README.md

**File Path:** `README.md`
**File Type:** .MD (markdown)

```markdown
# Ark.Alliance.Trading.Shared

Common models and enums shared between the backend and frontend projects.

## Task List
| # | Task | Prompt (System / User) | Status | Remarks & Remaining Work |
|---|------|-----------------------|--------|-------------------------|
|1|Add authentication request/response models|User|Closed|Introduced login and registration DTOs|
|2|Document shared models and enums|User|Closed|Added XML comments and reference links|



```

---

### Directory: Enums

**Full Path:** `C:\Users\ARKle\source\repos\Ark.Alliance.Trading\Ark.Alliance.Trading.Shared\Enums`
**Files:** 4

#### 💾 AnalysisRecommendation.cs

**File Path:** `Enums\AnalysisRecommendation.cs`
**File Type:** .CS (csharp)

```csharp
namespace Ark.Alliance.Trading.Shared.Enums;

/// <summary>
/// Overall technical recommendation from TradingView.
/// + Enables consistent Buy/Sell indicators across backend and frontend.
/// - Represents external analysis; verify before trading.
/// Ref: <see href="https://www.tradingview.com/support/solutions/43000534721-technical-analysis-overview/" />
/// </summary>
public enum AnalysisRecommendation
{
    /// <summary>Indicators strongly advise selling.</summary>
    StrongSell,

    /// <summary>Most indicators suggest a sell.</summary>
    Sell,

    /// <summary>Indicators are inconclusive.</summary>
    Neutral,

    /// <summary>Most indicators suggest buying.</summary>
    Buy,

    /// <summary>Indicators strongly advise buying.</summary>
    StrongBuy
}

```

---

#### 💾 BotStatus.cs

**File Path:** `Enums\BotStatus.cs`
**File Type:** .CS (csharp)

```csharp
namespace Ark.Alliance.Trading.Shared.Enums;

/// <summary>
/// Indicates the current lifecycle state of the trading bot.
/// + Allows UI components to reflect running status consistently.
/// - Does not convey internal health of sub-services.
/// Ref: <see href="https://learn.microsoft.com/dotnet/api/microsoft.extensions.hosting.backgroundservice" />
/// </summary>
public enum BotStatus
{
    /// <summary>Background service is not running.</summary>
    Stopped,
    /// <summary>Background service is executing the trading strategy.</summary>
    Running,
    /// <summary>Trading is temporarily paused due to external conditions.</summary>
    Paused
}

```

---

#### 💾 CircuitState.cs

**File Path:** `Enums\CircuitState.cs`
**File Type:** .CS (csharp)

```csharp
namespace Ark.Alliance.Trading.Shared.Enums;

/// <summary>
/// Represents the state of a circuit breaker.
/// + Mirrors <c>Ark.Api.Binance.Helpers.CircuitState</c>.
/// + Used by clients to display circuit breaker status.
/// - Aggregates all endpoints; individual circuit details are unavailable.
/// </summary>
public enum CircuitState
{
    /// <summary>Normal operation - requests allowed.</summary>
    Closed,

    /// <summary>Failure threshold exceeded - requests blocked.</summary>
    Open,

    /// <summary>Testing recovery - limited requests allowed.</summary>
    HalfOpen
}

```

---

#### 💾 RiskLevel.cs

**File Path:** `Enums\RiskLevel.cs`
**File Type:** .CS (csharp)

```csharp
using System;

namespace Ark.Alliance.Trading.Shared.Enums;

/// <summary>
/// Defines overall risk exposure levels.
/// + Shared between backend risk management and frontend UI indicators.
/// - Does not quantify precise exposure; consult backend logs for details.
/// Ref: <see href="https://www.investopedia.com/terms/r/risk-level.asp" />
/// </summary>
public enum RiskLevel
{
    Low,
    Medium,
    High,
    Critical
}

```

---

### Directory: Models

**Full Path:** `C:\Users\ARKle\source\repos\Ark.Alliance.Trading\Ark.Alliance.Trading.Shared\Models`
**Files:** 2

#### 💾 TickerHistoryDto.cs

**File Path:** `Models\TickerHistoryDto.cs`
**File Type:** .CS (csharp)

```csharp
namespace Ark.Alliance.Trading.Shared.Models;

using Ark.Api.Binance;
using System.Collections.Generic;

/// <summary>
/// Snapshot of collected ticker data for multiple symbols.
/// + Enables correlation of historical quotes across providers.
/// - Requires external pruning to avoid unbounded memory growth.
/// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.collections.generic.dictionary-2" />
/// </summary>
public class TickerHistoryDto
{
    #region Properties

    /// <summary>
    /// Historical ticks keyed by trading symbol.
    /// + Provides ordered <see cref="TickerDto" /> sequences per symbol.
    /// - Symbols with heavy activity may cause high memory usage.
    /// </summary>
    /// <example>
    /// {
    ///   "BTCUSDT": [
    ///     { "Symbol": "BTCUSDT", "Price": 30000.0, "Timestamp": "2024-01-01T00:00:00Z" }
    ///   ]
    /// }
    /// </example>
    public Dictionary<string, TickerDto[]> History { get; set; } = new();

    #endregion Properties
}

```

---

#### 💾 TradingSettings.cs

**File Path:** `Models\TradingSettings.cs`
**File Type:** .CS (csharp)

```csharp
namespace Ark.Alliance.Trading.Shared.Models;

/// <summary>
/// Application settings controlling trading rules.
/// + Centralizes configurable parameters for the bot.
/// - Default values are indicative and may not suit production.
/// Ref: <see href="https://binance-docs.github.io/apidocs/futures/en/" />
/// </summary>
public class TradingSettings
{
    /// <summary>
    /// Trading symbol traded by the bot.
    /// + Defaults to BTCUSDT for quick testing.
    /// - Must be a valid Binance Futures symbol.
    /// Ref: <see href="https://binance-docs.github.io/apidocs/futures/en/#public-endpoints-info" />
    /// </summary>
    public string Symbol { get; set; } = "BTCUSDT";
    /// <summary>
    /// Initial protective order distance (R2).
    /// + Sets the first stop-loss distance.
    /// - Excessive values reduce profitability.
    /// </summary>
    public decimal InitialProtectionPct { get; set; } = 0.05m;
    /// <summary>
    /// Percentage part of minimal net gain (R1).
    /// + Ensures trades meet yield expectations.
    /// - Ignores absolute fees; see <see cref="MinNetGainAbsolute"/>.
    /// </summary>
    public decimal MinNetYieldPct { get; set; } = 0.01m;
    /// <summary>
    /// Absolute part of minimal net gain in USDT (R1).
    /// + Protects against tiny profit trades.
    /// - Assumes quote asset is USDT.
    /// </summary>
    public decimal MinNetGainAbsolute { get; set; } = 1m;
    /// <summary>
    /// Estimated fees for round trip.
    /// + Influences net gain calculation.
    /// - Actual exchange fees may vary.
    /// </summary>
    public decimal FeeRate { get; set; } = 0.0004m;
    /// <summary>
    /// Estimated funding rate.
    /// + Accounts for periodic funding payments.
    /// - Only an estimate; check live rate.
    /// </summary>
    public decimal FundingRateEst { get; set; } = 0.0001m;
    /// <summary>
    /// Leverage used when opening the initial position.
    /// + Higher leverage reduces capital requirement.
    /// - Increases liquidation risk.
    /// </summary>
    public int InitialLeverage { get; set; } = 20;
    /// <summary>
    /// Transfer threshold for profits (R5).
    /// + Automates profit sweeping.
    /// - Frequent transfers may incur fees.
    /// </summary>
    public decimal TransferThresholdPct { get; set; } = 0.05m;
    /// <summary>
    /// Timeout before cancelling partial fills (R6).
    /// + Prevents hanging orders.
    /// - Too low may cancel valid fills.
    /// </summary>
    public int PartialFillTimeoutSec { get; set; } = 3;
    /// <summary>
    /// Safety margin applied when sizing positions.
    /// + Provides buffer against rapid moves.
    /// - Reduces potential profit.
    /// </summary>
    public decimal SafetyMarginFactor { get; set; } = 0.8m;
    /// <summary>
    /// Pause trading when rate limit usage exceeds this ratio.
    /// + Avoids hitting Binance hard limits.
    /// - May delay trades during spikes.
    /// </summary>
    public decimal RateLimitThresholdPct { get; set; } = 0.7m;
    /// <summary>
    /// Resume trading when usage falls below this ratio.
    /// + Restores activity once safe.
    /// - Requires accurate limiter stats.
    /// </summary>
    public decimal RateLimitRecoveryPct { get; set; } = 0.3m;
    /// <summary>
    /// Time window in seconds to compute the average spread for entry.
    /// + Smooths spread fluctuations.
    /// - Longer windows reduce reactivity.
    /// </summary>
    public int AvgSpreadWindowSec { get; set; } = 30;
    /// <summary>
    /// Critical latency threshold in milliseconds that may trigger emergency liquidation.
    /// </summary>
    /// <remarks>
    /// + Increase to tolerate slower networks.
    /// - Lower values can cause premature liquidation.
    /// Reference: <see cref="Ark.Api.Binance.Services.LatencyOptions.CriticalLatencyThresholdMs"/>.
    /// </remarks>
    public decimal MaxLatencyThresholdMs { get; set; } = 3000m;
}

```

---

### Directory: Models\Dtos

**Full Path:** `C:\Users\ARKle\source\repos\Ark.Alliance.Trading\Ark.Alliance.Trading.Shared\Models\Dtos`
**Files:** 6

#### 💾 AnalysisSummaryDto.cs

**File Path:** `Models\Dtos\AnalysisSummaryDto.cs`
**File Type:** .CS (csharp)

```csharp
using System;
using Ark.Alliance.Trading.Shared.Enums;

namespace Ark.Alliance.Trading.Shared.Models.Dtos;

/// <summary>
/// Technical analysis summary returned by TradingView.
/// + Provides high-level market signal for UI and strategy tuning.
/// - Relies on third-party data; confirm before executing trades.
/// Ref: <see href="https://www.tradingview.com/support/solutions/43000534721-technical-analysis-overview/" />
/// </summary>
public class AnalysisSummaryDto
{
    /// <summary>
    /// Ticker symbol.
    /// + Identifies the market pair analysed.
    /// - Not validated against exchange listings.
    /// </summary>
    public string Symbol { get; set; } = string.Empty;

    /// <summary>
    /// Overall recommendation.
    /// + Facilitates unified "Buy"/"Sell" semantics.
    /// - Does not replace full technical analysis.
    /// </summary>
    public AnalysisRecommendation Recommendation { get; set; } = AnalysisRecommendation.Neutral;

    /// <summary>
    /// Timestamp of the analysis.
    /// + Allows sorting and staleness checks.
    /// - Based on client clock of provider.
    /// </summary>
    public DateTime Timestamp { get; set; }
}

```

---

#### 💾 BotOptimizationStatsDto.cs

**File Path:** `Models\Dtos\BotOptimizationStatsDto.cs`
**File Type:** .CS (csharp)

```csharp
namespace Ark.Alliance.Trading.Shared.Models.Dtos;

/// <summary>
/// Summary statistics for bot optimisation metrics.
/// + Aggregates performance indicators for dashboards.
/// - Snapshot values; historical trend requires external storage.
/// Ref: <see href="https://en.wikipedia.org/wiki/Sharpe_ratio" />
/// </summary>
public class BotOptimizationStatsDto
{
    /// <summary>
    /// Peak-to-trough equity decline.
    /// + Highlights worst recent performance.
    /// - Does not include open positions.
    /// </summary>
    public decimal CurrentDrawdown { get; set; }

    /// <summary>
    /// Ratio of winning trades to total trades.
    /// + Quick gauge of strategy success.
    /// - Ignores profit magnitude.
    /// </summary>
    public double WinRate { get; set; }

    /// <summary>
    /// Number of trades executed in the analysed period.
    /// + Useful for throughput analysis.
    /// - High counts may hide overtrading.
    /// </summary>
    public int TradesExecuted { get; set; }
}

```

---

#### 💾 CircuitBreakerStatsDto.cs

**File Path:** `Models\Dtos\CircuitBreakerStatsDto.cs`
**File Type:** .CS (csharp)

```csharp
using Ark.Alliance.Trading.Shared.Enums;

namespace Ark.Alliance.Trading.Shared.Models.Dtos;

/// <summary>
/// Circuit breaker statistics exposed by the backend.
/// + Provides insight into failure counts and state transitions.
/// - Values may be stale between polls.
/// Ref: <see href="https://learn.microsoft.com/dotnet/core/extensions/circuit-breaker" />
/// </summary>
public class CircuitBreakerStatsDto
{
    /// <summary>
    /// Current state of the circuit.
    /// + Indicates whether requests are permitted.
    /// - Global value; not per-endpoint.
    /// </summary>
    public CircuitState State { get; set; }

    /// <summary>
    /// Number of failures recorded in the current window.
    /// + Helps diagnose instability.
    /// - Counter resets after state transitions.
    /// </summary>
    public int FailureCount { get; set; }

    /// <summary>
    /// Threshold of failures required to open the circuit.
    /// + Mirrors backend configuration.
    /// - Changing runtime values may desync.
    /// </summary>
    public int FailureThreshold { get; set; }

    /// <summary>
    /// Last time an operation failed.
    /// + Useful for troubleshooting.
    /// - Client clock differences may mislead.
    /// </summary>
    public DateTime LastFailureTime { get; set; }

    /// <summary>
    /// Next time the circuit will attempt to close.
    /// + Allows frontends to show countdowns.
    /// - Only approximate; backend may attempt sooner.
    /// </summary>
    public DateTime NextAttemptTime { get; set; }

    /// <summary>
    /// Duration the circuit remains open before retrying.
    /// + Communicates resilience policy.
    /// - Actual retry timing may vary.
    /// </summary>
    public TimeSpan RecoveryTimeout { get; set; }
}

```

---

#### 💾 RegisterRequestDto.cs

**File Path:** `Models\Dtos\RegisterRequestDto.cs`
**File Type:** .CS (csharp)

```csharp
using System.ComponentModel.DataAnnotations;

namespace Ark.Alliance.Trading.Shared.Models.Dtos;

/// <summary>
/// Request payload for user registration.
/// + Used to create an initial profile and API key.
/// - Password is hashed with a demo algorithm.
/// Ref: <see href="https://datatracker.ietf.org/doc/html/rfc7231#section-4.3.3" />
/// </summary>
public class RegisterRequestDto
{
    /// <summary>Desired username.</summary>
    [Required]
    public string Username { get; set; } = string.Empty;

    /// <summary>User e-mail address.</summary>
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>Account password.</summary>
    [Required]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;
}


```

---

#### 💾 RiskAssessmentDto.cs

**File Path:** `Models\Dtos\RiskAssessmentDto.cs`
**File Type:** .CS (csharp)

```csharp
using System;
using System.Collections.Generic;
using Ark.Alliance.Trading.Shared.Enums;

namespace Ark.Alliance.Trading.Shared.Models.Dtos;

/// <summary>
/// Represents the current risk evaluation for a trading symbol.
/// + Returned by backend risk endpoints and consumed by the dashboard.
/// - Factors provide descriptive hints without weighting details.
/// Ref: <see href="https://www.investopedia.com/terms/r/riskmanagement.asp" />
/// </summary>
public class RiskAssessmentDto
{
    /// <summary>
    /// Trading symbol assessed.
    /// + Identifies which market pair was evaluated.
    /// - Not validated against live exchange listings.
    /// </summary>
    public string Symbol { get; set; } = string.Empty;

    /// <summary>
    /// Time when the assessment was produced.
    /// + Allows clients to gauge staleness.
    /// - Depends on server clock accuracy.
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Computed risk level.
    /// + Simplifies display logic.
    /// - Coarse-grained; see <see cref="RiskScore"/> for nuance.
    /// </summary>
    public RiskLevel RiskLevel { get; set; }

    /// <summary>
    /// Aggregated risk score (implementation specific).
    /// + Enables fine-tuned thresholds.
    /// - Scale varies by backend implementation.
    /// </summary>
    public int RiskScore { get; set; }

    /// <summary>
    /// Primary reason for the risk level.
    /// + Useful for quick diagnosis.
    /// - Free-form text; not localised.
    /// </summary>
    public string Reason { get; set; } = string.Empty;

    /// <summary>
    /// List of contributing factors.
    /// + Provides deeper context for analysts.
    /// - Ordered arbitrarily; no weighting.
    /// </summary>
    public List<string> Factors { get; set; } = new();
}

```

---

#### 💾 SessionInfoDto.cs

**File Path:** `Models\Dtos\SessionInfoDto.cs`
**File Type:** .CS (csharp)

```csharp
using Ark.Api.Binance;

namespace Ark.Alliance.Trading.Shared.Models.Dtos;

/// <summary>
/// Basic information about a Binance session.
/// + Used to track server-side sessions from the UI.
/// - Does not expose security-sensitive details.
/// Ref: <see href="https://binance-docs.github.io/apidocs/futures/en/#user-data-streams" />
/// </summary>
public class SessionInfoDto
{
    /// <summary>
    /// Session identifier.
    /// + Correlates backend logs and user actions.
    /// - Unique only within the current database.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Creation timestamp of the session.
    /// + Helps compute session lifetime.
    /// - Uses server local time.
    /// </summary>
    public DateTime Created { get; set; }

    /// <summary>
    /// Identifier of the user owning the session.
    /// + Enables multi-user dashboards.
    /// - No role or permission information.
    /// </summary>
    public string OwnerId { get; set; } = string.Empty;

    /// <summary>
    /// Binance environment used for this session.
    /// + Distinguishes live vs testnet usage.
    /// - Enum may not include future environments.
    /// </summary>
    public BinanceEnvironment Environment { get; set; }
}

```

---

### Directory: Models\Requests

**Full Path:** `C:\Users\ARKle\source\repos\Ark.Alliance.Trading\Ark.Alliance.Trading.Shared\Models\Requests`
**Files:** 5

#### 💾 ApiKeyLoginRequest.cs

**File Path:** `Models\Requests\ApiKeyLoginRequest.cs`
**File Type:** .CS (csharp)

```csharp
using System.ComponentModel.DataAnnotations;

namespace Ark.Alliance.Trading.Shared.Models.Requests;

/// <summary>
/// Authentication payload using a previously issued API key.
/// + Enables headless clients.
/// - API key must be kept secret.
/// Ref: <see href="https://owasp.org/www-community/controls/Key_Management" />
/// </summary>
public class ApiKeyLoginRequest
{
    /// <summary>Raw API key.</summary>
    [Required]
    public string ApiKey { get; set; } = string.Empty;
}


```

---

#### 💾 LoginRequest.cs

**File Path:** `Models\Requests\LoginRequest.cs`
**File Type:** .CS (csharp)

```csharp
using System.ComponentModel.DataAnnotations;

namespace Ark.Alliance.Trading.Shared.Models.Requests;

/// <summary>
/// Credentials for username/password authentication.
/// + Used for interactive sign-in.
/// - Not suitable for API key login.
/// Ref: <see href="https://datatracker.ietf.org/doc/html/rfc6749" />
/// </summary>
public class LoginRequest
{
    /// <summary>User login name.</summary>
    [Required]
    public string Username { get; set; } = string.Empty;

    /// <summary>Account password.</summary>
    [Required]
    public string Password { get; set; } = string.Empty;
}


```

---

#### 💾 MarginRequirementRequest.cs

**File Path:** `Models\Requests\MarginRequirementRequest.cs`
**File Type:** .CS (csharp)

```csharp
using System.ComponentModel.DataAnnotations;

namespace Ark.Alliance.Trading.Shared.Models.Requests;

/// <summary>
/// Request to project margin requirements for a prospective trade.
/// + Supplies trade parameters for projection.
/// - Does not validate symbol-specific leverage caps.
/// Ref: <see href="https://www.binance.com/en/futures/fee" />
/// </summary>
public class MarginRequirementRequest
{
    /// <summary>
    /// Futures contract symbol.
    /// + Determines contract size and tick value.
    /// - Case-sensitive; mismatches cause rejection.
    /// </summary>
    [Required]
    public string Symbol { get; set; } = string.Empty;

    /// <summary>
    /// Number of contracts to open.
    /// + Drives margin calculation.
    /// - Must respect exchange minimums.
    /// </summary>
    [Range(0.0001, double.MaxValue)]
    public decimal Quantity { get; set; }

    /// <summary>
    /// Entry price in quote asset.
    /// + Used to compute notional value.
    /// - High precision may be truncated by exchange.
    /// </summary>
    [Range(0.0001, double.MaxValue)]
    public decimal Price { get; set; }

    /// <summary>
    /// Applied leverage multiplier.
    /// + Allows margin reduction for lower capital usage.
    /// - Excess leverage increases liquidation risk.
    /// </summary>
    [Range(1, 125)]
    public int Leverage { get; set; }
}

```

---

#### 💾 StartBotRequest.cs

**File Path:** `Models\Requests\StartBotRequest.cs`
**File Type:** .CS (csharp)

```csharp
using Ark.Api.Binance;
using Ark.Alliance.Trading.Shared.Models;

namespace Ark.Alliance.Trading.Shared.Models.Requests;

/// <summary>
/// Request payload used to start the trading bot.
/// + Bundles Binance credentials and trading parameters.
/// - Does not validate configuration coherence.
/// Ref: <see href="https://binance-docs.github.io/apidocs/futures/en/#change-log" />
/// </summary>
public class StartBotRequest
{
    /// <summary>
    /// Binance connection options.
    /// + Supplies API keys and environment info.
    /// - Secrets must be secured externally.
    /// </summary>
    public BinanceOptions BinanceOptions { get; set; } = new();

    /// <summary>
    /// Initial trading settings.
    /// + Defines symbol and strategy thresholds.
    /// - Subsequent changes require restart.
    /// </summary>
    public TradingSettings TradingSettings { get; set; } = new();
}

```

---

#### 💾 StopBotRequest.cs

**File Path:** `Models\Requests\StopBotRequest.cs`
**File Type:** .CS (csharp)

```csharp
namespace Ark.Alliance.Trading.Shared.Models.Requests;

/// <summary>
/// Optional parameters for stopping the trading bot.
/// + Allows graceful or immediate shutdown.
/// - Does not persist state for later resume.
/// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken" />
/// </summary>
public class StopBotRequest
{
    /// <summary>
    /// When true, open positions are liquidated at market.
    /// + Ensures no exposure after stop.
    /// - Market orders may incur slippage.
    /// </summary>
    public bool LiquidateMarket { get; set; } = true;
}

```

---

### Directory: Models\Responses

**Full Path:** `C:\Users\ARKle\source\repos\Ark.Alliance.Trading\Ark.Alliance.Trading.Shared\Models\Responses`
**Files:** 3

#### 💾 LoginResponse.cs

**File Path:** `Models\Responses\LoginResponse.cs`
**File Type:** .CS (csharp)

```csharp
using System;

namespace Ark.Alliance.Trading.Shared.Models.Responses;

/// <summary>
/// Response returned after a successful login.
/// + Includes session token for subsequent requests.
/// - Token is temporary and non-JWT.
/// Ref: <see href="https://datatracker.ietf.org/doc/html/rfc7231#section-6.3.1" />
/// </summary>
public class LoginResponse
{
    /// <summary>Identifier of the authenticated user.</summary>
    public Guid UserId { get; set; }

    /// <summary>Authenticated username.</summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>Issued session token.</summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>Expiration timestamp of the session.</summary>
    public DateTime ExpiresAt { get; set; }
}


```

---

#### 💾 RegisterResponse.cs

**File Path:** `Models\Responses\RegisterResponse.cs`
**File Type:** .CS (csharp)

```csharp
using System;

namespace Ark.Alliance.Trading.Shared.Models.Responses;

/// <summary>
/// Response returned after a successful user registration.
/// + Provides one-time API key to authenticate.
/// - API key is shown only once.
/// Ref: <see href="https://datatracker.ietf.org/doc/html/rfc7231#section-6.3.1" />
/// </summary>
public class RegisterResponse
{
    /// <summary>Identifier of the created user profile.</summary>
    public Guid UserId { get; set; }

    /// <summary>Registered username.</summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>One-time API key issued to the user.</summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>Informational message for the client.</summary>
    public string Message { get; set; } = string.Empty;
}


```

---

#### 💾 StartBotResponse.cs

**File Path:** `Models\Responses\StartBotResponse.cs`
**File Type:** .CS (csharp)

```csharp
using System;

namespace Ark.Alliance.Trading.Shared.Models.Responses;

/// <summary>
/// Result returned after attempting to start the trading bot.
/// + Provides session identifier on success.
/// - Does not include detailed failure diagnostics.
/// Ref: <see href="https://datatracker.ietf.org/doc/html/rfc7231#section-6" />
/// </summary>
public class StartBotResponse
{
    /// <summary>
    /// Identifier of the created session.
    /// + Use to query session status.
    /// - Null when <see cref="Success"/> is false.
    /// </summary>
    public Guid SessionId { get; set; }

    /// <summary>
    /// Indicates whether the start request succeeded.
    /// + Simplifies client branching logic.
    /// - True even if warnings occurred.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Error message when <see cref="Success"/> is false.
    /// + Helps troubleshoot configuration issues.
    /// - Not localised.
    /// </summary>
    public string? Error { get; set; }
}

```


---

**Analysis completed on 2025-08-09 at 20:49:55**
**Comments:** Preserved as written
**JavaScript/JSX:** Included

*Generated by Code Structure and Content Analyzer (v3)*