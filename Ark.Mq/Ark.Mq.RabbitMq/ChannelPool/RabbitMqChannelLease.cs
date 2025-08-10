using System;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace Ark.Net.RabbitMq;

/// <summary>
/// Represents a leased RabbitMQ channel returned from <see cref="RabbitMqChannelPool"/>.
/// Disposing the lease returns the channel to the pool.
/// </summary>
public sealed class RabbitMqChannelLease : IAsyncDisposable
{
    internal RabbitMqChannelLease(IModel channel, RabbitMqChannelPool pool)
    {
        Channel = channel;
        _pool = pool;
    }

    private readonly RabbitMqChannelPool _pool;

    /// <summary>The leased channel instance.</summary>
    public IModel Channel { get; }

    /// <inheritdoc />
    public ValueTask DisposeAsync()
    {
        _pool.Release(Channel);
        return ValueTask.CompletedTask;
    }
}
