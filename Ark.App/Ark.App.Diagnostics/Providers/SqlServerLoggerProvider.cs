using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Ark.App.Diagnostics
{
    /// <inheritdoc />
    /// <summary>
    /// An <see cref="T:Microsoft.Extensions.Logging.ILoggerProvider" /> that writes logs to a SQL SERVER database.
    /// </summary>
    [ProviderAlias("SqlServer")]
    public class SqlServerLoggerProvider : BatchingLoggerProvider
    {
        #region Static

        /// <summary>
        /// Whether this provider has already been initialized.
        /// It is initialized if the Logs table has been checked and maybe created in database.
        /// </summary>
        private static bool _isInitialized;

        #endregion Static

        #region Fields

        /// <summary>
        /// The mutex is needed to properly synchronize the log writing to be able to initialize correctly the table.
        /// </summary>
        private readonly MutexAsync _mutex = new MutexAsync();

        /// <summary>
        /// The log database repository is needed.
        /// </summary>
        internal LogDbServices LogDbServices;

        #endregion Fields

        #region Constructors

        /// <inheritdoc />
        /// <summary>
        /// Creates an instance of the <see cref="T:Ark.App.Diagnostics.SqlServerLoggerProvider" /> 
        /// </summary>
        /// <param name="options">The options object controlling the logger.</param>
        public SqlServerLoggerProvider(IOptionsMonitor<SqlServerLoggerOptions> options)
            : base(options.CurrentValue)
        {
            LogDbServices = new LogDbServices(options.CurrentValue.SqlServerConnectionString, options.CurrentValue.SqlServerTableName);
        }

        #endregion Constructors

        #region Methods (Override)

        /// <inheritdoc />
        protected override async Task WriteLogsAsync(IEnumerable<LogMessage> messages, CancellationToken cancellationToken)
            => await _mutex.WaitAndReleaseAsync(async () =>
        {
            if (!_isInitialized)
            {
                await LogDbServices.CreateLogsTableIfNotExists();
                _isInitialized = true;
            }

            var logs = messages.Select(message => new LogDbEntity
            {
                Timestamp = message.Timestamp.DateTime,
                LogLevel = message.LogLevel,
                Message = message.Message,
                Category = message.Category,
                Exception = message.Exception?.ToDetailedString()
            });

            await LogDbServices.InsertLogs(logs);
        }, cancellationToken);

        #endregion Methods (Override)
    }
}