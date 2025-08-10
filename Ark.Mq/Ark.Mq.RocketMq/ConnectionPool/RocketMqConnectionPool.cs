using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Resilience;
using Apache.RocketMQ.Client;
using Ark.Net.RocketMq.Diagnostics;

namespace Ark.Net.RocketMq;

/// <summary>
/// Provides a simple connection pool with retry policies.
/// </summary>
public class RocketMqConnectionPool : IDisposable
{
    #region Fields
    private readonly ConcurrentBag<IProducer> _producers = new();
    private readonly RocketMqSettings _options;
    private readonly ILogger<RocketMqConnectionPool> _logger;
    private readonly ResiliencePipeline<IProducer> _pipeline;
    private int _createdConnections;
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of <see cref="RocketMqConnectionPool"/>.
    /// </summary>
    public RocketMqConnectionPool(IOptions<RocketMqSettings> options, ILogger<RocketMqConnectionPool> logger, ResiliencePipelineProvider provider)
    {
        _options = options.Value;
        _logger = logger;
        _pipeline = provider.GetPipeline<IProducer>("rocketmq");
    }
    #endregion

    #region Methods (Public)
    /// <summary>
    /// Acquires an open connection from the pool.
    /// </summary>
    public async Task<IProducer> AcquireAsync()
    {
        if (_producers.TryTake(out var producer))
            return producer;

        if (Interlocked.Increment(ref _createdConnections) <= _options.MaxConnections)
            return await CreateConnectionAsync();

        while (true)
        {
            if (_producers.TryTake(out producer))
                return producer;
            await Task.Delay(50);
        }
    }

    /// <summary>
    /// Returns a connection back to the pool.
    /// </summary>
    public void Release(IProducer producer)
    {
        _producers.Add(producer);
    }
    #endregion

    #region Methods (Private)
    private async Task<IProducer> CreateConnectionAsync()
    {
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
        _logger.Log(_options.LogLevel, "RocketMQ producer started for {Server}", _options.NameServerAddress);
        RocketMqMetrics.ConnectionsOpened.Add(1);
        return producer;
    }
    #endregion

    #region IDisposable
    /// <inheritdoc />
    public void Dispose()
    {
        while (_producers.TryTake(out var producer))
        {
            producer.Dispose();
            RocketMqMetrics.ConnectionsClosed.Add(1);
        }
    }
    #endregion
}
