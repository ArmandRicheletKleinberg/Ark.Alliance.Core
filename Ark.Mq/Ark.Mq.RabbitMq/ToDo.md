# RabbitMQ Library TODO

This document tracks planned improvements for **Ark.Mq.RabbitMq**. Each item is
marked with its current status.

| Status | Task |
|--------|------|
| Closed | Extend `RabbitMqSettings` with TLS, publisher confirms, prefetch, rate limiting and confirmation timeout. |
| Closed | Update publisher and consumers to use new settings, manual acknowledgements and rate limiting. |
| Closed | Add OpenTelemetry instrumentation for connection metrics and integrate with `Ark.App.Diagnostics`. Metrics exposed via Prometheus exporter. |
| Closed | Provide `MessageContext` model for custom headers and correlation identifiers. |
| Closed | Expand unit tests for connection failure scenarios and diagnostics helpers. |

_Last update: 2025-07-18_
| Closed | Implement `IBrokerProducer` interface for `RabbitMqPublisher` and register via DI. |
| Closed | Record metrics `messages_published_total` and `messages_failed_total`. |
| Closed | Convert `MessageContext` to immutable `record` type. |
| Closed | Provide `IBrokerConsumer` implementation wrapping `RabbitMqConsumer`. |
| Closed | Document usage of new interfaces and metrics in README. |
