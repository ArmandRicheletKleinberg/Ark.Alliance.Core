using System;
using Microsoft.Extensions.Logging;

namespace Ark.App.Diagnostics
{
    /// <inheritdoc />
    /// <summary>
    /// This logger is used to log log messages in batch to enhance performance for some specific file writer loggers (file, database).
    /// </summary>
    public class BatchingLogger : ILogger
    {
        #region Fields

        /// <summary>
        /// The batching logger provider.
        /// </summary>
        private readonly BatchingLoggerProvider _provider;

        /// <summary>
        /// The category of the log message.
        /// </summary>
        private readonly string _category;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Creates a <see cref="BatchingLogger"/> instance.
        /// </summary>
        /// <param name="loggerProvider">The batching logger provider.</param>
        /// <param name="categoryName">The category of the log message.</param>
        public BatchingLogger(BatchingLoggerProvider loggerProvider, string categoryName)
        {
            _provider = loggerProvider;
            _category = categoryName;
        }

        #endregion Constructors

        #region ILogger

        /// <inheritdoc />
        public IDisposable BeginScope<TState>(TState state)
            => null;

        /// <inheritdoc />
        public bool IsEnabled(LogLevel logLevel)
            => _provider.IsEnabled;

        /// <inheritdoc />
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            var message = new LogMessage
            {
                Timestamp = DateTimeOffset.UtcNow,
                LogLevel = logLevel,
                Category = _category,
                Message = formatter(state, exception),
                Exception = exception
            };
            _provider.AddMessage(message);
        }

        #endregion ILogger
    }
}