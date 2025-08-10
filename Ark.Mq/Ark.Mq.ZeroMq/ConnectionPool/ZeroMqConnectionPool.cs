using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Resilience;
using Ark;
using NetMQ.Sockets;
using Ark.Net.ZeroMq.Diagnostics;

namespace Ark.Net.ZeroMq;

/// <summary>
/// Provides a simple publisher socket pool with retry policies.
/// </summary>
public class ZeroMqConnectionPool : IDisposable
{
    #region Fields
    private readonly ConcurrentBag<PublisherSocket> _sockets = new();
    private readonly ZeroMqSettings _options;
    private readonly ILogger<ZeroMqConnectionPool> _logger;
    private readonly ResiliencePipeline<PublisherSocket> _pipeline;
    private int _createdSockets;
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of <see cref="ZeroMqConnectionPool"/>.
    /// </summary>
    /// <param name="options">ZeroMQ configuration options.</param>
    /// <param name="logger">Logger used for diagnostics.</param>
    public ZeroMqConnectionPool(IOptions<ZeroMqSettings> options, ILogger<ZeroMqConnectionPool> logger, ResiliencePipelineProvider provider)
    {
        _options = options.Value;
        _logger = logger;
        _pipeline = provider.GetPipeline<PublisherSocket>("zeromq");
    }
    #endregion

    #region Methods (Public)
    /// <summary>
    /// Acquires an open socket from the pool.
    /// </summary>
    /// <returns>
    /// Success : The socket has been acquired.
    /// Unexpected : An unexpected error occurs.
    /// </returns>
    public async Task<Result<PublisherSocket>> AcquireAsync()
    {
        if (_sockets.TryTake(out var socket) && !socket.IsDisposed)
            return new Result<PublisherSocket>(socket);

        if (Interlocked.Increment(ref _createdSockets) <= _options.MaxConnections)
            return await CreateSocketAsync();

        while (true)
        {
            if (_sockets.TryTake(out socket) && !socket.IsDisposed)
                return new Result<PublisherSocket>(socket);
            await Task.Delay(50);
        }
    }

    /// <summary>
    /// Returns a socket back to the pool.
    /// </summary>
    /// <param name="socket">The socket to return.</param>
    public void Release(PublisherSocket socket)
    {
        if (!socket.IsDisposed)
            _sockets.Add(socket);
        else
        {
            socket.Dispose();
            ZeroMqMetrics.ConnectionsClosed.Add(1);
        }
    }
    #endregion

    #region Methods (Private)
    /// <summary>
    /// Creates and connects a new publisher socket.
    /// </summary>
    /// <returns>
    /// Success : The socket has been created.
    /// Unexpected : An unexpected error occurs.
    /// </returns>
    private async Task<Result<PublisherSocket>> CreateSocketAsync()
    {
        try
        {
            var socket = await _pipeline.ExecuteAsync(_ => ValueTask.FromResult(new PublisherSocket()));
            socket.Connect(_options.Endpoint);
            _logger.Log(_options.LogLevel, "ZeroMQ publisher connected to {Endpoint}", _options.Endpoint);
            ZeroMqMetrics.ConnectionsOpened.Add(1);
            return new Result<PublisherSocket>(socket);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while creating ZeroMQ socket");
            return new Result<PublisherSocket>(ex);
        }
    }
    #endregion

    #region IDisposable
    /// <summary>Disposes all sockets in the pool.</summary>
    public void Dispose()
    {
        while (_sockets.TryTake(out var socket))
        {
            socket.Dispose();
            ZeroMqMetrics.ConnectionsClosed.Add(1);
        }
    }
    #endregion
}
