using System;
using Microsoft.Extensions.Logging;

#nullable enable

namespace Ark.Api.Binance
{ 
    /// <summary>
    /// Wraps an <see cref="ILogger"/> and filters messages below <see cref="Diag.MinimumLevel"/>.
    /// </summary>
    internal class LevelFilteredLogger : ILogger
    {
        private readonly ILogger _inner;

        public LevelFilteredLogger(ILogger inner) => _inner = inner;

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
            => _inner.BeginScope(state);

        public bool IsEnabled(LogLevel logLevel) => _inner.IsEnabled(logLevel) && logLevel >= Diag.MinimumLevel;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (IsEnabled(logLevel))
                _inner.Log(logLevel, eventId, state, exception, formatter);
        }
    }
}
