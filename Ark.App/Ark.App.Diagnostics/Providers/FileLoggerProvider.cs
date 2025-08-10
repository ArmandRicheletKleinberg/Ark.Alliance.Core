using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Ark.App.Diagnostics
{
    /// <inheritdoc />
    /// <summary>
    /// An <see cref="T:Microsoft.Extensions.Logging.ILoggerProvider" /> that writes logs to a file.
    /// </summary>
    [ProviderAlias("File")]
    public class FileLoggerProvider : BatchingLoggerProvider
    {
        #region Fields

        /// <summary>
        /// The options that describe the behavior to log into a file.
        /// </summary>
        private readonly FileLoggerOptions _options;

        #endregion Fields

        #region Constructors

        /// <inheritdoc />
        /// <summary>
        /// Creates an instance of the <see cref="T:Ark.App.Diagnostics.FileLoggerProvider" /> 
        /// </summary>
        /// <param name="options">The options object controlling the logger.</param>
        public FileLoggerProvider(IOptionsMonitor<FileLoggerOptions> options)
            : base(options.CurrentValue)
        {
            _options = options.CurrentValue;
        }

        #endregion Constructors

        #region Methods (Override)

        /// <inheritdoc />
        protected override async Task WriteLogsAsync(IEnumerable<LogMessage> messages, CancellationToken cancellationToken)
        {
            Directory.CreateDirectory(_options.LogDirectory);
            var filePath = GetFilePath(DateTime.Now);
            var fileInfo = new FileInfo(filePath);

            if (_options.FileSizeLimit > 0 && fileInfo.Exists && fileInfo.Length > _options.FileSizeLimit)
                return;

            using (var streamWriter = File.AppendText(filePath))
                await messages.ForEachAsync(async message =>
                {
                    var messageString = $"{message.Timestamp.ToLocalTime():G}    " +
                                        $"{message.LogLevel.ToString().PadRight(15, ' ')}" +
                                        $"{message.Category.PadRight(20, ' ')}" +
                                        $"{message.Message}{(message.Exception != null ? $"{Environment.NewLine}{message.Exception.ToDetailedString()}" : "")}";
                    await streamWriter.WriteLineAsync(messageString);
                });

            RollFiles();
        }

        #endregion Methods (Override)

        #region Methods (Helpers)

        /// <summary>
        /// Gets the log file path given a timestamp and the periodicity options.
        /// </summary>
        /// <param name="timestamp">The timestamp to use to create the file name.</param>
        /// <returns>The created file path.</returns>
        private string GetFilePath(DateTime timestamp)
        {
            string fileSuffix;
            switch (_options.Periodicity)
            {
                case FileLoggerPeriodicityEnum.Minutely: fileSuffix = timestamp.ToString("yyyyMMddHHmm"); break;
                case FileLoggerPeriodicityEnum.Hourly: fileSuffix = timestamp.ToString("yyyyMMddHH"); break;
                case FileLoggerPeriodicityEnum.Daily: fileSuffix = timestamp.ToString("yyyyMMdd"); break;
                case FileLoggerPeriodicityEnum.Monthly: fileSuffix = timestamp.ToString("yyyyMM"); break;
                default: throw new InvalidDataException("Invalid periodicity");
            }

            var filePath = Path.Combine(_options.LogDirectory, $"{_options.FilePrefixName}{fileSuffix}.{_options.FileExtension}");
            return filePath;
        }

        /// <summary>
        /// Deletes old log files, keeping a number of files defined by <see cref="FileLoggerOptions.RetainedFileCountLimit" />
        /// </summary>
        protected void RollFiles()
        {
            if (_options.RetainedFileCountLimit <= 0)
                return;

            new DirectoryInfo(_options.LogDirectory)
                .GetFiles(_options.FilePrefixName + "*")
                .OrderByDescending(f => f.Name)
                .Skip(_options.RetainedFileCountLimit)
                .ForEach(f => f.Delete());
        }

        #endregion Methods (Helpers)
    }
}