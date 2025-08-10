using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ark;
using Microsoft.Extensions.Logging;
using Apache.RocketMQ.Client;

namespace Ark.Net.RocketMq;

/// <summary>
/// Base class providing common RocketMQ operations using the connection pool.
/// </summary>
public abstract class RocketMqRepositoryBase
{
    private readonly RocketMqConnectionPool _pool;
    private readonly ILogger<RocketMqRepositoryBase> _logger;

    protected RocketMqRepositoryBase(RocketMqConnectionPool pool, ILogger<RocketMqRepositoryBase> logger)
    {
        _pool = pool;
        _logger = logger;
    }

    protected async Task<Result> Execute(Func<IProducer, Result> action)
    {
        var producer = await _pool.AcquireAsync();
        try
        {
            return action(producer);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RocketMQ operation failed");
            return new Result(ex);
        }
        finally
        {
            _pool.Release(producer);
        }
    }

    protected async Task<Result<T>> Execute<T>(Func<IProducer, Result<T>> action)
    {
        var producer = await _pool.AcquireAsync();
        try
        {
            return action(producer);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RocketMQ operation failed");
            return new Result<T>(ex);
        }
        finally
        {
            _pool.Release(producer);
        }
    }
}
