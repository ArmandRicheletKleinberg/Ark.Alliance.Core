# Ark Alliance Core

Ark Alliance Core hosts a suite of reusable .NET 9 libraries that follow Domain‑Driven Design, event‑driven patterns and Clean Architecture. The collection ranges from low-level utilities to complete API wrappers and messaging adapters, enabling scalable and maintainable services across the ecosystem.

## Libraries

| Library | Summary | README |
| --- | --- | --- |
| Ark | Core collections, extensions and patterns for building applications. | [README](Ark/README.md) |
| Ark.App | Application bootstrap helpers for Ark-based services. | *(pending)* |
| Ark.App.Diagnostics | Structured logging, health indicators and diagnostics. | [README](Ark.App/Ark.App.Diagnostics/README.md) |
| Ark.Data | Caching, file repositories and settings management. | [README](Ark.Data/Ark.Data/README.md) |
| Ark.Data.EFCore | Base Entity Framework Core repository helpers. | *(pending)* |
| Ark.Data.EFCore.SqlServer | SQL Server provider for Ark.Data.EFCore. | *(pending)* |
| Ark.Data.Excel | Excel and CSV export utilities. | [README](Ark.Data/Ark.Data.Excel/README.md) |
| Ark.Api.TradingView | Client for TradingView search and quote APIs. | [README](Ark.Api.TradingView/README.md) |
| Ark.Api.Binance | High level Binance Futures REST/WebSocket wrapper with rate limiting. | [README](Ark.Api.Binance/README.md) |
| Ark.Net.Ftp | FTP repository built on FluentFTP. | [README](Ark.Net/Ark.Net.Ftp/README.md) |
| Ark.Net.Http | HTTP client helpers with Result pattern. | [README](Ark.Net/Ark.Net.Http/README.md) |
| Ark.Net.Models | Shared DTOs for queries, logs, emails and users. | [README](Ark.Net/Ark.Net.Models/README.md) |
| Ark.Net.CrossCutting | Unified access to cross-cutting services (users, email, archive). | [README](Ark.Net/Ark.Net.CrossCutting/README.md) |
| Ark.Net.Ssh | SSH.NET extensions for SFTP and command execution. | [README](Ark.Net/Ark.Net.Ssh/README.md) |
| Ark.AspNetCore | ASP.NET Core controller bases, DI extensions and Windows service helpers. | [README](Ark.AspNetCore/Ark.AspNetCore/README.md) |
| Ark.AspNetCore.Search | Search helpers for ASP.NET Core endpoints. | *(pending)* |
| Ark.AspNetCore.Search.Models | Embedded search item type dataset. | [README](Ark.AspNetCore/Ark.AspNetCore.Search.Models/README.md) |
| Ark.System.Info | Cross-platform system, network and storage inspection utilities. | [README](Ark.System/Ark.Core.Systeminfo/README.md) |
| Ark.Mq.Emqx5 | MQTT (EMQX 5) messaging helpers. | [README](Ark.Mq/Ark.Mq.Emqx5/README.md) |
| Ark.Mq.RabbitMq | RabbitMQ messaging helpers with resilience and diagnostics. | [README](Ark.Mq/Ark.Mq.RabbitMq/README.md) |
| Ark.Mq.RocketMq | RocketMQ messaging helpers. | [README](Ark.Mq/Ark.Mq.RocketMq/README.md) |
| Ark.Mq.MqSeries | IBM MQSeries messaging helpers. | *(pending)* |
| Ark.Mq.ZeroMq | ZeroMQ messaging helpers. | [README](Ark.Mq/Ark.Mq.ZeroMq/README.md) |

## Highlights

- **.NET 9 everywhere** for access to the latest C# features and runtime improvements.
- **Complete Binance library** with session management and proactive rate limiting.
- **Multiple messaging adapters** (RabbitMQ, RocketMQ, EMQX, ZeroMQ, MQSeries) built on Ark.Cqrs for broker‑agnostic code.
- **Rich diagnostics** via Ark.App.Diagnostics and OpenTelemetry integrations.

## Dependencies and Thanks

The ecosystem relies on many excellent open source packages:

| Package | Version | Repository |
| --- | --- | --- |
| JetBrains.Annotations | 2025.2.0 | https://github.com/JetBrains/JetBrains.Annotations |
| Newtonsoft.Json | 13.0.3 | https://github.com/JamesNK/Newtonsoft.Json |
| System.Text.Json | 9.0.8 | https://github.com/dotnet/runtime |
| Microsoft.Extensions.* | 9.0.8 | https://github.com/dotnet/runtime |
| Microsoft.EntityFrameworkCore.SqlServer | 9.0.8 | https://github.com/dotnet/efcore |
| EPPlus | 8.0.8 | https://github.com/EPPlusSoftware/EPPlus |
| FluentValidation | 12.0.0 | https://github.com/FluentValidation/FluentValidation |
| FluentFTP | 33.0.3 | https://github.com/robinrodricks/FluentFTP |
| SSH.NET | 2025.0.0 | https://github.com/sshnet/SSH.NET |
| Binance.Net | 11.4.0 | https://github.com/JKorf/Binance.Net |
| RabbitMQ.Client | 6.7.0 | https://github.com/rabbitmq/rabbitmq-dotnet-client |
| PenGen.RocketMQ.Client | 1.1.0 | https://github.com/pen-gen/RocketMQ.Client |
| MQTTnet.AspNetCore | 5.0.1.1416 | https://github.com/dotnet/MQTTnet |
| NetMQ | 4.0.0.1 | https://github.com/zeromq/netmq |
| OpenTelemetry & Extensions.Hosting | 1.7.0 / 1.3.2 | https://github.com/open-telemetry/opentelemetry-dotnet |
| AgileObjects.ReadableExpressions | 4.1.3 | https://github.com/agileobjects/ReadableExpressions |
| LibGit2Sharp | 0.31.0 | https://github.com/libgit2/libgit2sharp |
| iTextSharp-LGPL-Core | 1.2.0 | https://github.com/itext/itextsharp |

Thank you to all maintainers and contributors of these projects.

## License

This repository is licensed under the MIT License. See [LICENSE.txt](LICENSE.txt) for details.

---
Armand Richelet-Kleinberg
