using System;
using System.Threading.Tasks;
using Ark;
using Microsoft.Extensions.Logging;
using MQTTnet;

namespace Ark.Net.Mqtt.Iot.Emqx5;

/// <summary>
/// Base class providing common MQTT operations using the connection pool.
/// </summary>
public abstract class Emqx5BkrMttqRepositoryBase
{
    #region Fields
    private readonly Emqx5BkrMttqConnectionPool _pool;
    private readonly ILogger<Emqx5BkrMttqRepositoryBase> _logger;
    #endregion

    /// <summary>
    /// Initializes a new instance of <see cref="Emqx5BkrMttqRepositoryBase"/>.
    /// </summary>
    /// <param name="pool">Connection pool.</param>
    /// <param name="logger">Logger instance.</param>
    #region Constructors
    protected Emqx5BkrMttqRepositoryBase(Emqx5BkrMttqConnectionPool pool, ILogger<Emqx5BkrMttqRepositoryBase> logger)
    {
        _pool = pool;
        _logger = logger;
    }
    #endregion

    /// <summary>
    /// Executes an action using a pooled MQTT client.
    /// </summary>
    /// <param name="action">Action to execute.</param>
    /// <returns>
    /// Success : The execution has succeeded.
    /// Unexpected : An unexpected error occurs.
    /// </returns>
    #region Methods (Protected)
    protected async Task<Result> Execute(Func<IMqttClient, Task<Result>> action)
    {
        var client = await _pool.AcquireAsync();
        try
        {
            return await action(client);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "MQTT operation failed");
            return new Result(ex);
        }
        finally
        {
            _pool.Release(client);
        }
    }
    #endregion
}
