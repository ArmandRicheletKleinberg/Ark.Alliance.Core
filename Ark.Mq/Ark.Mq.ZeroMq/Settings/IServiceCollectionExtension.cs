using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Resilience;
using NetMQ.Sockets;
using Ark.Cqrs.Messaging.Abstractions;
using OpenTelemetry.Metrics;
using Ark.Net.ZeroMq.Diagnostics;

namespace Ark.Net.ZeroMq;

/// <summary>
/// Extensions to register ZeroMQ services.
/// </summary>
// ReSharper disable once InconsistentNaming
public static class IServiceCollectionExtension
{
    /// <summary>Adds ZeroMQ support using configuration.</summary>
    /// <param name="services">Service collection to register into.</param>
    /// <param name="configuration">Application configuration source.</param>
    /// <param name="sectionKey">Configuration section key.</param>
    public static void AddZeroMq(this IServiceCollection services, IConfiguration configuration, string sectionKey = "ZeroMq")
    {
        var section = configuration.GetSection(sectionKey);
        services.Configure<ZeroMqSettings>(section);

        var opts = section.Get<ZeroMqSettings>() ?? new ZeroMqSettings();

        services.AddResiliencePipeline<PublisherSocket>("zeromq")
            .AddRetry(o => o.MaxRetryAttempts = opts.RetryCount);

        services.AddOpenTelemetry()
            .WithMetrics(b =>
            {
                b.AddMeter(ZeroMqMetrics.MeterName);
                b.AddPrometheusExporter();
            });

        services.AddSingleton<ZeroMqConnectionPool>();
        services.AddTransient<ZeroMqPublisher>();
        services.AddTransient<IBrokerProducer, ZeroMqPublisher>();
        services.AddTransient<ZeroMqConsumer>();
        services.AddTransient<IBrokerConsumer, ZeroMqBrokerConsumer>();
    }
}
