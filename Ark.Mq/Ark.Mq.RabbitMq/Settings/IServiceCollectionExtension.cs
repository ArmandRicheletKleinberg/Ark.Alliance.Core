using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Resilience;
using OpenTelemetry.Metrics;
using Ark.Net.RabbitMq.Diagnostics;
using Ark;
using RabbitMQ.Client;

namespace Ark.Net.RabbitMq;

/// <summary>
/// Extensions to register RabbitMQ services.
/// </summary>
// ReSharper disable once InconsistentNaming
public static class IServiceCollectionExtension
{
    /// <summary>
    /// Adds RabbitMQ support using configuration.
    /// <code>
    /// services.AddRabbitMq(Configuration);
    /// </code>
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">Application configuration source.</param>
    /// <summary>Adds RabbitMQ support using configuration.</summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">Configuration root containing RabbitMQ settings.</param>
    /// <param name="sectionKey">Configuration section key.</param>
    public static void AddRabbitMq(this IServiceCollection services, IConfiguration configuration, string sectionKey = "RabbitMq")
    {
        var pools = configuration.GetSection(sectionKey)
            .Get<Dictionary<string, RabbitMqSettings>>();
        var section = configuration.GetSection(sectionKey);
        services.Configure<RabbitMqSettings>(section);

        var opts = section.Get<RabbitMqSettings>() ?? new RabbitMqSettings();
        services.AddResiliencePipeline<IConnection>("rabbitmq")
            .AddRetry(o => o.MaxRetryAttempts = opts.RetryCount)
            .AddCircuitBreaker();

        services.AddResiliencePipeline<Result>("rabbitmq.publish")
            .AddRetry(o => o.MaxRetryAttempts = opts.RetryCount)
            .AddCircuitBreaker();

        services.AddOpenTelemetry()
            .WithMetrics(b =>
            {
                b.AddMeter(RabbitMqMetrics.MeterName);
                b.AddPrometheusExporter();
            });

        services.AddSingleton<RabbitMqConnection>();
        services.AddSingleton<RabbitMqChannelPool>();
        services.AddTransient<RabbitMqPublisher>();
        services.AddTransient<IBrokerProducer, RabbitMqPublisher>();
        services.AddTransient<RabbitMqConsumer>();
        services.AddTransient<IBrokerConsumer, RabbitMqBrokerConsumer>();
    }
}
