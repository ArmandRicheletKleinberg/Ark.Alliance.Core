using System;
using System.IO;
using System.Threading.Tasks;
using Ark.Data;
using Ark.App.Diagnostics;
using Ark.Net.Ftp;
using Microsoft.Extensions.Logging;

namespace Ark.Net.MqSeries
{
    /// <summary>
    /// Repository to manage file on the ftp mainframe by receiving order of the mainframe.
    /// </summary>
    public class MainFrameFtpFileRepository
    {
        #region Fields

        /// <summary>
        /// The mainframe repository base is needed to get the message.
        /// </summary>
        internal FtpMqSeriesRepository FtpMqSeriesRepository;

        /// <summary>
        /// The repository to manage a FTP. 
        /// </summary>
        internal FtpRepository FtpRepository = new FtpRepository();

        #endregion Fields

        #region Methods (Public)

        /// <summary>
        /// This method check a queue of the mainframe to know if we need to download a file on the mainframe and return it or not. 
        /// </summary>
        /// <param name="connectionPoolName">The name of the MQ connection pool to use with this repository.</param>
        /// <param name="queueKey">The key of the queue with is linked to the queue name in the settings.</param>
        /// <param name="ftpSettings">The settings for the connection to the FTP.</param>
        /// <param name="action">The asynchronous action to execute which returns a result which will be tested to remove the messages from the queue.</param>
        /// <param name="log">Logger.</param>
        /// <returns>
        /// Success : The execution has succeeded and the mainframe file is returned.
        /// BadPrerequisites : The connection pool name has not been found in the pre initialized settings.
        /// NotFound : No message was found in the queue event with the optional wait time.
        /// Failure : The file on the FTP is not found or empty.
        /// Timeout : No available connection could be used to open the queue in the given laps.
        /// NoConnection : No connection can be made to the MQ Series Server.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public async Task<Result<FtpFile>> GetFileOnMainFrame(string connectionPoolName, string queueKey, FtpConnectionSettings ftpSettings, Func<FtpFile, Task<Result>> action = null, ILogger log = null)
        {
            FtpMqSeriesRepository ??= new FtpMqSeriesRepository(connectionPoolName);
            FtpRepository.Initialize(ftpSettings);

            var ftpFile = new FtpFile();

            var browseResult = await FtpMqSeriesRepository.FtpBrowseFirstMainFrameObjectAndRemoveOnActionSuccess(queueKey,async entity => 
            {
                var downloadFileResult = await FtpRepository.DownLoadFileAsStringByFtp(entity.CheminFtp);
                if (downloadFileResult.IsNotFound)
                {
                    log?.Log(LogLevel.Error, $"The {entity.CheminFtp} was not found on the FTP {ftpSettings.Host}");
                    return Result.Success;
                }
                if (downloadFileResult.IsNotSuccess)
                    return new Result<FtpFile>(downloadFileResult);

                ftpFile = new FtpFile
                {
                    Content = downloadFileResult.Data,
                    Length = downloadFileResult.Data.Length,
                    FileName = entity.CheminDestination.IsNotNullOrEmpty() ? Path.GetFileName(entity.CheminDestination) : string.Empty
                };

                if (action != null) 
                {
                    var actionResult = await action(ftpFile);

                    if (actionResult.IsNotSuccess)
                        log?.LogResult(actionResult);

                    return actionResult;
                }
                    
                return Result.Success;
            });

            if (browseResult.IsNotSuccess)
                return new Result<FtpFile>(browseResult);

            if (ftpFile.Content.IsNullOrEmpty())
                return new Result<FtpFile>(ResultStatus.Failure).WithReason("No file was found on the FTP.");

            return new Result<FtpFile>(ftpFile);
        }


        #endregion Methods (Public)


    }
}
