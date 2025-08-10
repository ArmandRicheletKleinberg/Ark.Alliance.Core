using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Ark.Net.RabbitMq;

/// <summary>
/// Provides a thread-safe pool of RabbitMQ channels for publishing and consuming.
/// </summary>
public class RabbitMqChannelPool
{
    private readonly ConcurrentBag<IModel> _channels = new();
    private readonly RabbitMqConnection _connectionProvider;
    private readonly ILogger<RabbitMqChannelPool> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="RabbitMqChannelPool"/>.
    /// </summary>
    public RabbitMqChannelPool(RabbitMqConnection connectionProvider, ILogger<RabbitMqChannelPool> logger)
    {
        _connectionProvider = connectionProvider;
        _logger = logger;
    }

    /// <summary>
    /// Acquires a channel lease from the pool. A new channel is created using the
    /// shared connection if none are available.
    /// </summary>
    public async Task<RabbitMqChannelLease> AcquireAsync()
    {
        if (_channels.TryTake(out var channel) && channel.IsOpen)
            return new RabbitMqChannelLease(channel, this);

        var connection = await _connectionProvider.GetConnectionAsync();
        channel = connection.CreateModel();
        _logger.LogDebug("RabbitMQ channel opened");
        return new RabbitMqChannelLease(channel, this);
    }

    internal void Release(IModel channel)
    {
        if (channel.IsOpen)
        {
            _channels.Add(channel);
        }
        else
        {
            channel.Dispose();
        }
    }
}
