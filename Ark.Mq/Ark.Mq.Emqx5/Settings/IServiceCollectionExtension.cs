using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Resilience;
using Ark.Cqrs.Messaging.Abstractions;
using OpenTelemetry.Metrics;
using Ark.Net.Mqtt.Iot.Emqx5.Diagnostics;

namespace Ark.Net.Mqtt.Iot.Emqx5;

/// <summary>
/// Extensions to register MQTT services.
/// </summary>
// ReSharper disable once InconsistentNaming
public static class IServiceCollectionExtension
{
    #region Methods (Public)
    /// <summary>Adds MQTT support using configuration.</summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration">Source configuration.</param>
    /// <param name="sectionKey">Configuration section key.</param>
    public static void AddEmqx5BkrMttq(this IServiceCollection services, IConfiguration configuration, string sectionKey = "Mqtt")
    {
        var section = configuration.GetSection(sectionKey);
        services.Configure<Emqx5BkrMttqSettings>(section);

        var opts = section.Get<Emqx5BkrMttqSettings>() ?? new Emqx5BkrMttqSettings();

        services.AddResiliencePipeline("emqx5mqtt")
            .AddRetry(o => o.MaxRetryAttempts = opts.RetryCount);

        services.AddOpenTelemetry()
            .WithMetrics(b =>
            {
                b.AddMeter(Emqx5BkrMttqMetrics.MeterName);
                b.AddPrometheusExporter();
            });

        services.AddSingleton<Emqx5BkrMttqConnectionPool>();
        services.AddTransient<Emqx5BkrMttqPublisher>();
        services.AddTransient<IBrokerProducer, Emqx5BkrMttqPublisher>();
        services.AddTransient<Emqx5BkrMttqConsumer>();
        services.AddTransient<IBrokerConsumer, Emqx5BkrMttqBrokerConsumer>();
    }
    #endregion
}
