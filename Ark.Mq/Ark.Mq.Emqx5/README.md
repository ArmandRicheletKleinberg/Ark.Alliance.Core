# Ark.Mq.Emqx5

Reusable MQTT helpers targeting the EMQX 5 broker for Ark microservices.

## Index

1. [Overview](#overview)
2. [Features](#features)
3. [Architecture](#architecture)
4. [Usage](#usage)
5. [Diagnostics](#diagnostics)
6. [Testing Scenarios](#testing-scenarios)
7. [Dependencies](#dependencies)
8. [References](#references)
9. [Author](#author)

## Overview

This package exposes a thin infrastructure layer that follows a
**Clean Architecture** approach. Message publishing is exposed through
Ark.Cqrs commands so that application code remains decoupled from the
MQTT client implementation.

```mermaid
flowchart TD
    A[Application] -->|sends| B((Ark.Cqrs Command))
    B --> C[Emqx5BkrMttqPublisher]
    C --> D[(EMQX Broker)]
    D -->|messages| E[Consumers]
```

This library provides a lightweight wrapper around `MQTTnet` in order to simplify message publishing and consumption in a clean architecture style. It mirrors the structure of other `Ark.Mq` modules like `Ark.Mq.RabbitMq`.

## Features
 - Configuration via `Emqx5BkrMttqSettings` with broker and connection settings
- Connection pooling with retry policies
- `Emqx5BkrMttqPublisher` service to send strongly typed messages
- `Emqx5BkrMttqConsumer` service to receive strongly typed messages
- Implements `IBrokerProducer`/`IBrokerConsumer` for decoupled messaging
- `MessageContext<T>` model carries headers and correlation identifiers
- Background consumer service for long running processes
- Base repository helpers via `Emqx5BkrMttqRepositoryBase`
- Extension method `AddEmqx5BkrMttq` to register the services with `IServiceCollection`
- Diagnostics helpers via `DiagnosticsEmqx5BkrMttqRepository` and `Emqx5BkrMttqReportsBase`
- Optional OpenTelemetry metrics via `Emqx5BkrMttqMetrics` for Prometheus integration

## Architecture

```mermaid
classDiagram
    class Emqx5BkrMttqConnectionPool
    class Emqx5BkrMttqPublisher
    class Emqx5BkrMttqConsumer
    Emqx5BkrMttqConnectionPool --> Emqx5BkrMttqPublisher
    Emqx5BkrMttqConnectionPool --> Emqx5BkrMttqConsumer
```

## Usage
1. Reference **Ark.Mq.Emqx5** in your microservice.
2. Add configuration section `Mqtt` with host, username, etc.
3. Register the services:
   ```csharp
   services.AddEmqx5BkrMttq(Configuration);
   ```
4. Inject `Emqx5BkrMttqPublisher` or `Emqx5BkrMttqConsumer` where needed.

```mermaid
sequenceDiagram
    participant App
    participant Publisher
    participant MQ as EMQX
    participant Consumer
    App->>Publisher: PublishAsync
    Publisher->>MQ: mqtt.publish
    MQ-->>Consumer: message
    Consumer-->>App: handler invoked
```

## Diagnostics
Use `DiagnosticsEmqx5BkrMttqRepository` to publish and unsubscribe for health checks.

## Testing Scenarios
- Validate reconnection after broker restart.
- Stress test publishing large payloads.

## Dependencies
- `.NET 9`
- `MQTTnet`
- `Microsoft.Extensions.DependencyInjection`
- `Ark.App.Diagnostics`

## Advantages
- Decouples messaging logic from application code via Ark.Cqrs commands.
- Provides connection pooling with retry logic for robust producer/consumer setups.
- Minimal dependencies and easy to integrate in any service.

## Limitations
- The library focuses on basic publish/consume scenarios. Advanced features like
  transactions or complex topologies must be implemented separately.

## TODO
- Improve unit test coverage for connection failure scenarios.
 - Expand diagnostics samples using `Emqx5BkrMttqReportsBase`.

## References
- [EMQX Documentation](https://www.emqx.io/docs/en/latest/)
- Ark.Cqrs documentation (internal)


## Author

Armand Richelet-Kleinberg
