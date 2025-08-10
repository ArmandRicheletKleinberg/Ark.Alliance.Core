using System;
using System.Threading.Tasks;
using Ark;
using Microsoft.Extensions.Logging;
using NetMQ.Sockets;

namespace Ark.Net.ZeroMq;

/// <summary>
/// Base class providing common ZeroMQ operations using the connection pool.
/// </summary>
public abstract class ZeroMqRepositoryBase
{
    #region Fields
    private readonly ZeroMqConnectionPool _pool;
    private readonly ILogger<ZeroMqRepositoryBase> _logger;
    #endregion

    #region Constructors
    protected ZeroMqRepositoryBase(ZeroMqConnectionPool pool, ILogger<ZeroMqRepositoryBase> logger)
    {
        _pool = pool;
        _logger = logger;
    }
    #endregion

    #region Methods (Protected)
    /// <summary>
    /// Executes an action using a publisher socket from the pool.
    /// </summary>
    /// <param name="action">The action to execute with the socket.</param>
    /// <returns>
    /// Success : The execution has succeeded.
    /// Unexpected : An unexpected error occurs.
    /// </returns>
    protected async Task<Result> Execute(Func<PublisherSocket, Result> action)
    {
        var socketResult = await _pool.AcquireAsync();
        if (socketResult.IsNotSuccess)
            return new Result(socketResult);

        var socket = socketResult.Data;
        try
        {
            return action(socket);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ZeroMQ operation failed");
            return new Result(ex);
        }
        finally
        {
            _pool.Release(socket);
        }
    }

    /// <summary>
    /// Executes an action using a publisher socket from the pool and returns a value.
    /// </summary>
    /// <typeparam name="T">Type of the result data.</typeparam>
    /// <param name="action">The action to execute with the socket.</param>
    /// <returns>
    /// Success : The execution has succeeded and the value is returned.
    /// Unexpected : An unexpected error occurs.
    /// </returns>
    protected async Task<Result<T>> Execute<T>(Func<PublisherSocket, Result<T>> action)
    {
        var socketResult = await _pool.AcquireAsync();
        if (socketResult.IsNotSuccess)
            return new Result<T>(socketResult);

        var socket = socketResult.Data;
        try
        {
            return action(socket);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ZeroMQ operation failed");
            return new Result<T>(ex);
        }
        finally
        {
            _pool.Release(socket);
        }
    }
    #endregion
}
