
# Complete Guide: ZeroMQ in C# .NET (Broker‑less Messaging, .NET 8/9/10)

## Introduction
**ZeroMQ** (ØMQ) is a high‑performance asynchronous messaging library that removes the need for a dedicated broker. Applications communicate **peer‑to‑peer** over in‑proc, IPC, TCP, or multicast transports using a simple socket‑like API. Its *black‑box* concurrency model scales from threads to clusters, making ZeroMQ a pragmatic choice when you need raw speed, low latency, and flexible topologies without the operational burden of a centralized broker.

---

## 1  Current Versions & Releases — July 14 2025
| Component | Latest Release | Date | Notes |
|-----------|----------------|------|-------|
| **libzmq (C/C++)** | **4.3.5** | 2023‑10‑09 | Still the latest stable tag. |
| **NetMQ (C#)** | **4.0.2.1** | 2025‑06‑16 | Targets .NET 8/9/10; actively maintained. |
| **License** | MPL‑2.0 | 2023‑10‑09 | Relicensing from LGPL completed. |
| **.NET 8** | LTS | 2023‑11‑14 | Production baseline. |
| **.NET 9** | STS | 2024‑11‑12 | Performance upgrade path. |
| **.NET 10** | Preview 5 (LTS) | 2025‑06‑09 | C# 14, NativeAOT gains. |

---

## 2  ZeroMQ Essentials
### 2.1  Socket Patterns
- **REQ / REP** — RPC‑style request/response.
- **DEALER / ROUTER** — High‑throughput async RPC, manual envelopes.
- **PUB / SUB** — Broadcast & fan‑out with topic prefix filtering.
- **PUSH / PULL** — Pipeline task distribution with fair‑queuing.
- **PAIR** — Exclusive 1‑to‑1, mainly for in‑proc tests.

### 2.2  Transports
`inproc://`, `ipc://`, `tcp://`, `pgm://` / `epgm://` (reliable multicast).

### 2.3  Security – CURVE (Curve25519)
Built‑in authentication and encryption (ZAP v2). Provides perfect‑forward secrecy per connection.

---

## 3  Best‑Practice Updates for 2025
| Area | Recommendation | Why |
|------|---------------|-----|
| Threading | One socket per thread; bridge with `inproc`. | libzmq sockets are not thread‑safe. |
| High‑Water‑Mark | Start with `SndHwm`=1000, `RcvHwm`=1000. | Prevent unbounded queues. |
| Reconnect | `ReconnectInterval` 100 ms, `ReconnectIntervalMax` 5 s. | Matches NetMQ defaults. |
| Busy‑Poll | Enable `ZMQ_BUSY_POLL` only when sub‑50 µs latency is required. | Avoids context‑switch delays. |
| License Compliance | MPL‑2.0 is file‑level copyleft; prefer dynamic plugin models. | Keeps proprietary hosts clean. |

---

## 4  Modern C# Binding — NetMQ 4.0.2.1
```bash
dotnet add package NetMQ --version 4.0.2.1
```
Highlights:
* Async `SendAsync` / `ReceiveAsync` API.
* Targets **net8.0**, compatible up to **net10.0**.
* Maintains MPL‑2.0 parity with upstream.

### Minimal Example
```csharp
using NetMQ;
using NetMQ.Sockets;

// Publisher
using var pub = new PublisherSocket("@tcp://*:5556");
pub.Options.SendHighWatermark = 1000;

// Subscriber
using var sub = new SubscriberSocket(">tcp://localhost:5556");
sub.Subscribe("price");

// Publish loop
_ = Task.Run(async () =>
{
    while (true)
    {
        pub.SendMoreFrame("price").SendFrame(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString());
        await Task.Delay(50);
    }
});

// Consume
while (true)
{
    var topic = sub.ReceiveFrameString();
    var payload = sub.ReceiveFrameString();
    Console.WriteLine($"{topic}: {payload}");
}
```

---

## 5  Choosing .NET 8 vs 9 vs 10
| Feature | .NET 8 (LTS) | .NET 9 (STS) | .NET 10 Preview (LTS) |
|---------|--------------|--------------|-----------------------|
| Support Window | 3 yrs → Nov 2026 | 18 mo → May 2026 | 3 yrs post‑RTM (est. Nov 2028) |
| NativeAOT | Mature baseline | Smaller trims | Faster code‑gen |
| GC / PGO | Dynamic PGO | More SIMD ops | AVX10.2 vectorization |

---

## 6  Sample `appsettings.json`
```jsonc
{
  "ZeroMQ": {
    "Endpoints": {
      "PublisherBind":  "tcp://*:5556",
      "SubscriberConnect": ["tcp://localhost:5556"]
    },
    "HighWaterMarks": { "Send": 1000, "Receive": 1000 },
    "Reconnect": { "MinMs": 100, "MaxMs": 5000 },
    "Security": {
      "Curve": true,
      "ServerPublicKey": "ngZ8c…",
      "ClientPublicKey": "7VmP…",
      "ClientSecretKey": "Xw3J…"
    }
  }
}
```

---

## 7  Observability & Resilience
* **Microsoft.Extensions.Resilience** policies for retry/circuit‑breaker on transient `AgainException`.
* **OpenTelemetry** spans (`messaging.zeromq`) exported to Prometheus/Grafana.
* Scrape socket metrics via `zmq_socket_monitor`.

---

## 8  Testing Matrix
| Layer | Tooling | Goal |
|-------|---------|------|
| Unit | xUnit + `PairSocket` | Frame integrity, envelope parsing |
| Integration | TestContainers‑dotnet + NetMQ | Multi‑container PUB/SUB latency |
| Load | k6 or wrk | Throughput vs HWM & busy‑poll |

---

## Sources
1. libzmq 4.3.5 release notes – GitHub  
2. ZeroMQ license page  
3. NetMQ NuGet package page  
4. Microsoft .NET support lifecycle  
5. .NET 10 Preview 5 blog post  
6. ZMQ `ZMQ_BUSY_POLL` option – API docs  
7. ZeroMQ download matrix  
8. CURVEZMQ specification  
9. ZeroMQ encryption guide  
10. NetMQ async API documentation  