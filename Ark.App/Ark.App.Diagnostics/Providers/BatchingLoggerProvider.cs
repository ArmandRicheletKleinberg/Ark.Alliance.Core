using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Ark.App.Diagnostics
{
    /// <inheritdoc />
    /// <summary>
    /// This provider is used log batches of logs in one operation to 
    /// </summary>
    public abstract class BatchingLoggerProvider : ILoggerProvider
    {
        #region Fields

        /// <summary>
        /// The options to manage the behavior of the batch logging.
        /// </summary>
        private readonly BatchingLoggerOptions _options;

        /// <summary>
        /// The queue with all the logs to write as a batch.
        /// </summary>
        private readonly BlockingCollection<LogMessage> _logsQueue = new BlockingCollection<LogMessage>(new ConcurrentQueue<LogMessage>());

        /// <summary>
        /// The cancellation token source used to cancel 
        /// </summary>
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        #endregion Fields

        #region Constructors

        /// <inheritdoc />
        /// <summary>
        /// Creates a <see cref="BatchingLoggerProvider"/> instance.
        /// </summary>
        /// <param name="options">The options to manage the behavior of the batch logging.</param>
        protected BatchingLoggerProvider(BatchingLoggerOptions options)
        {
            _options = options;
            Task.Factory.StartNew(ProcessLogQueue, TaskCreationOptions.LongRunning);
        }

        #endregion Constructors

        #region Properties (Internal)

        /// <summary>
        /// Whether the logger should be enabled.
        /// </summary>
        internal bool IsEnabled
            => _options.IsEnabled;

        #endregion Properties (Internal)

        #region IDisposable

        /// <inheritdoc />
        public void Dispose()
        {
            _cancellationTokenSource?.TryCancel();
        }

        #endregion IDisposable

        #region ILoggerProvider

        /// <inheritdoc />
        public ILogger CreateLogger(string categoryName)
            => new BatchingLogger(this, categoryName);

        #endregion ILoggerProvider

        #region Methods (Abstract)

        /// <summary>
        /// Writes all the cached logs in one batch depending on the inherited logger provider behavior.
        /// </summary>
        /// <param name="logs">The logs to write in one batch.</param>
        /// <param name="token">A token to cancel the write of the logs.</param>
        /// <returns>Asynchronous so must return a Task.</returns>
        protected abstract Task WriteLogsAsync(IEnumerable<LogMessage> logs, CancellationToken token);

        #endregion Methods (Abstract)

        #region Methods (ProcessLogQueue)

        /// <summary>
        /// Process the log queue and writes the logs in batch.
        /// </summary>
        /// <returns>Asynchronous so must return a Task.</returns>
        private async Task ProcessLogQueue()
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                var logsBatch = new List<LogMessage>();
                while (_logsQueue.TryTake(out var log))
                    logsBatch.Add(log);

                if (logsBatch.HasAnElement())
                    await WriteLogsAsync(logsBatch, _cancellationTokenSource.Token);

                await Task.Delay(_options.FlushPeriod, _cancellationTokenSource.Token);
            }
        }

        #endregion Methods (ProcessLogQueue)

        #region Methods (AddMessage)

        /// <summary>
        /// Adds a message to the queue.
        /// Thread-safe implementation using the <see cref="BlockingCollection{LogMessage}"/>.
        /// </summary>
        /// <param name="message">The message to log.</param>
        internal void AddMessage(LogMessage message)
        {
            if (_logsQueue.IsAddingCompleted)
                return;

            _logsQueue.Add(message, _cancellationTokenSource.Token);
        }

        #endregion Methods (Public)
    }
}