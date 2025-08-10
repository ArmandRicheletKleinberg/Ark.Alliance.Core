using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Resilience;
using Ark.Architecture.DDD.Domain;
using Ark.Net.RabbitMq.Diagnostics;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace Ark.Net.RabbitMq;

/// <summary>
/// Provides a singleton RabbitMQ connection.
/// </summary>
public class RabbitMqConnection : IDisposable
{
    private readonly ILogger<RabbitMqConnection> _logger;
    private readonly RabbitMqSettings _options;
    private readonly ResiliencePipeline<IConnection> _pipeline;
    private IConnection? _connection;

    /// <summary>
    /// Creates a new instance of <see cref="RabbitMqConnection"/>.
    /// </summary>
    /// <param name="options">RabbitMQ configuration options.</param>
    /// <param name="logger">Logger for diagnostics.</param>
    /// <param name="provider">Resilience pipeline provider.</param>
    public RabbitMqConnection(IOptions<RabbitMqSettings> options,
        ILogger<RabbitMqConnection> logger,
        ResiliencePipelineProvider provider)
    {
        _options = options.Value;
        _logger = logger;
        _pipeline = provider.GetPipeline<IConnection>("rabbitmq");
    }

    /// <summary>Gets an open connection instance.</summary>
    /// <returns>An open <see cref="IConnection"/>.</returns>
    public async Task<IConnection> GetConnectionAsync()
    {
        if (_connection is { IsOpen: true })
            return _connection;

        var factory = new ConnectionFactory
        {
            HostName = _options.HostName,
            Port = _options.Port,
            VirtualHost = _options.VirtualHost,
            UserName = _options.UserName,
            Password = _options.Password
        };

        if (_options.UseTls)
        {
            factory.Ssl.Enabled = true;
        }

        _connection = await _pipeline.ExecuteAsync(_ => ValueTask.FromResult(factory.CreateConnection()));
        _connection.ConnectionShutdown += (_, args) =>
        {
            _logger.LogWarning("RabbitMQ connection shutdown: {Reason}", args.ReplyText);
            RabbitMqMetrics.ConnectionsClosed.Add(1);
        };
        _logger.Log(_options.LogLevel, "RabbitMQ connection opened to {Host}", _options.HostName);
        RabbitMqMetrics.ConnectionsOpened.Add(1);
        DomainEvents.Publish(new ConnectionOpenedNotification(_options.HostName));
        return _connection;
    }

    public void Dispose()
    {
        if (_connection?.IsOpen == true)
        {
            _connection.Close();
            _logger.Log(_options.LogLevel, "RabbitMQ connection closed");
            RabbitMqMetrics.ConnectionsClosed.Add(1);
            DomainEvents.Publish(new ConnectionClosedNotification(_options.HostName));
        }
    }
}
