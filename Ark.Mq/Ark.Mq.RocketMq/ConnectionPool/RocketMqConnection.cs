using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Resilience;
using RocketMQ.Client;


namespace Ark.Net.RocketMq;

/// <summary>
/// Provides a singleton RocketMQ producer connection.
/// </summary>
public class RocketMqConnection : IDisposable
{
    private readonly ILogger<RocketMqConnection> _logger;
    private readonly RocketMqSettings _options;
    private readonly ResiliencePipeline<IProducer> _pipeline;
    private IProducer? _producer;

    public RocketMqConnection(IOptions<RocketMqSettings> options, ILogger<RocketMqConnection> logger, ResiliencePipelineProvider provider)
    {
        _options = options.Value;
        _logger = logger;
        _pipeline = provider.GetPipeline<IProducer>("rocketmq");
    }

    /// <summary>Gets an open connection instance.</summary>
    public async Task<IProducer> GetProducerAsync()
    {
        if (_producer is not null)
            return _producer;

        var producer = new Producer(new ProducerOptions
        {
            GroupName = _options.ProducerGroup,
            NameServerAddress = _options.NameServerAddress
        });
        await _pipeline.ExecuteAsync(async _ =>
        {
            await producer.Start();
            return producer;
        });
        _producer = producer;
        _logger.Log(_options.LogLevel, "RocketMQ producer started for {Server}", _options.NameServerAddress);
        return _producer;
    }

    public void Dispose()
    {
        if (_producer is not null)
        {
            _producer.Dispose();
            _logger.Log(_options.LogLevel, "RocketMQ producer disposed");
        }
    }
}
