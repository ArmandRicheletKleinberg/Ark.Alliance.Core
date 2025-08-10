using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Resilience;
using Ark;
using NetMQ;
using NetMQ.Sockets;

namespace Ark.Net.ZeroMq;

/// <summary>
/// Provides a singleton publisher socket.
/// </summary>
public class ZeroMqConnection : IDisposable
{
    #region Fields
    private readonly ILogger<ZeroMqConnection> _logger;
    private readonly ZeroMqSettings _options;
    private readonly ResiliencePipeline<PublisherSocket> _pipeline;
    private PublisherSocket? _socket;
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of <see cref="ZeroMqConnection"/>.
    /// </summary>
    /// <param name="options">ZeroMQ configuration options.</param>
    /// <param name="logger">Logger used for diagnostics.</param>
    public ZeroMqConnection(IOptions<ZeroMqSettings> options, ILogger<ZeroMqConnection> logger, ResiliencePipelineProvider provider)
    {
        _options = options.Value;
        _logger = logger;
        _pipeline = provider.GetPipeline<PublisherSocket>("zeromq");
    }
    #endregion

    #region Methods (Public)
    /// <summary>
    /// Gets an open publisher socket instance.
    /// </summary>
    /// <returns>
    /// Success : The socket has been acquired.
    /// Unexpected : An unexpected error occurs.
    /// </returns>
    public async Task<Result<PublisherSocket>> GetSocketAsync()
    {
        if (_socket is { IsDisposed: false })
            return new Result<PublisherSocket>(_socket);

        try
        {
            _socket = await _pipeline.ExecuteAsync(_ => ValueTask.FromResult(new PublisherSocket()));
            _socket.Connect(_options.Endpoint);
            _logger.Log(_options.LogLevel, "ZeroMQ publisher connected to {Endpoint}", _options.Endpoint);
            return new Result<PublisherSocket>(_socket);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Creating ZeroMQ socket failed");
            return new Result<PublisherSocket>(ex);
        }
    }
    #endregion

    #region IDisposable
    /// <summary>Disposes the underlying socket.</summary>
    public void Dispose()
    {
        if (_socket != null)
        {
            _socket.Dispose();
            _logger.Log(_options.LogLevel, "ZeroMQ publisher disposed");
        }
    }
    #endregion
}
