using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Resilience;
using Apache.RocketMQ.Client;
using Ark.Cqrs.Messaging.Abstractions;
using OpenTelemetry.Metrics;
using Ark.Net.RocketMq.Diagnostics;

namespace Ark.Net.RocketMq;

/// <summary>
/// Extensions to register RocketMQ services.
/// </summary>
// ReSharper disable once InconsistentNaming
public static class IServiceCollectionExtension
{
    /// <summary>Adds RocketMQ support using configuration.</summary>
    public static void AddRocketMq(this IServiceCollection services, IConfiguration configuration, string sectionKey = "RocketMq")
    {
        var section = configuration.GetSection(sectionKey);
        services.Configure<RocketMqSettings>(section);

        var opts = section.Get<RocketMqSettings>() ?? new RocketMqSettings();

        services.AddResiliencePipeline<IProducer>("rocketmq")
            .AddRetry(o => o.MaxRetryAttempts = opts.RetryCount);

        services.AddOpenTelemetry()
            .WithMetrics(b =>
            {
                b.AddMeter(RocketMqMetrics.MeterName);
                b.AddPrometheusExporter();
            });

        services.AddSingleton<RocketMqConnectionPool>();
        services.AddTransient<RocketMqPublisher>();
        services.AddTransient<IBrokerProducer, RocketMqPublisher>();
        services.AddTransient<RocketMqConsumer>();
        services.AddTransient<IBrokerConsumer, RocketMqBrokerConsumer>();
    }
}
