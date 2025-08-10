using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentFTP;

namespace Ark.Net.Ftp
{
    /// <summary>
    /// This repository is used to manage a FTP.
    /// </summary>
    public class FtpRepository
    {
        #region Fields

        /// <summary>
        /// The settings for the connection to the FTP.
        /// </summary>
        private FtpConnectionSettings _settings;

        #endregion Fields

        #region Methods (Public)

        /// <summary>
        /// Initialize the settings for the connection of a FTP.
        /// </summary>
        /// <param name="settings">The settings for the connection to the FTP.</param>
        public void Initialize(FtpConnectionSettings settings)
        {
            _settings = settings;
        }


        /// <summary>
        /// Download a file on a FTP as an array of bytes.
        /// </summary>
        /// <param name="filePath">The path to the file to download.</param>
        /// Success : Return the file as an array of bytes.
        /// BadPrerequisites : The FTP settings are not set.
        /// NotFound : The file was not found on the FTP
        /// BadParameters : The settings are not well formated.
        /// Unexpected : An unexpected error occurs.
        public async Task<Result<byte[]>> DownLoadFileAsBytesByFtp(string filePath)
        {
            return await DownLoadFileByFtp(filePath);
        }

        /// <summary>
        /// Download a file on a FTP as a String.
        /// </summary>
        /// <param name="filePath">The path to the file to download.</param>
        /// Success : Return the file as a string.
        /// BadPrerequisites : The FTP settings are not set.
        /// NotFound : The file was not found on the FTP
        /// BadParameters : The settings are not well formated.
        /// Unexpected : An unexpected error occurs.
        public async Task<Result<string>> DownLoadFileAsStringByFtp(string filePath)
        {
            var downloadResult = await DownLoadFileByFtp(filePath, true);

            if (downloadResult.IsNotSuccess)
                return new Result<string>(downloadResult);

            return new Result<string>(Encoding.UTF8.GetString(downloadResult.Data));
        }

        #endregion Methods (Public)

        #region Methods (Helpers)

        /// <summary>
        /// This method downloads a file on a FTP.
        /// </summary>
        /// <param name="filePath">The path to the file to download.</param>
        /// /// <param name="isTextFile">The data type of the file to return is a string or a binary.</param>
        /// <returns>
        /// Success : Return the file as an array of bytes.
        /// BadPrerequisites : The FTP settings are not set.
        /// NotFound : The file was not found on the FTP
        /// BadParameters : The settings are not well formated.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        private async Task<Result<byte[]>> DownLoadFileByFtp(string filePath, bool isTextFile = false)
        {
            try
            {
                if (_settings == null)
                    return new Result<byte[]>(ResultStatus.BadPrerequisites).WithReason("The FTP settings are not set.");
                if (_settings.Host.IsNullOrEmpty())
                    return new Result<byte[]>(ResultStatus.BadParameters).WithReason("You need to provide a host for the ftp connection");
                if (_settings.UserName.IsNullOrEmpty())
                    return new Result<byte[]>(ResultStatus.BadParameters).WithReason("You need to provide a login for the ftp connection");
                if (_settings.Password.IsNullOrEmpty())
                    return new Result<byte[]>(ResultStatus.BadParameters).WithReason("You need to provide a password for the ftp connection");
                if (filePath.IsNullOrEmpty())
                    return new Result<byte[]>(ResultStatus.BadParameters).WithReason("You need to provide a file path to know which file to download");

                using var client = new FtpClient
                {
                    Host = _settings.Host,
                    Port = _settings.Port != 0 ? _settings.Port : 21,
                    DataConnectionType = FtpDataConnectionType.PASV,
                    Credentials = new NetworkCredential(_settings.UserName, _settings.Password),
                    DownloadDataType = isTextFile ? FtpDataType.ASCII : FtpDataType.Binary
                };

                try
                {
                    client.Connect();
                    var fileStream = await client.DownloadAsync($"'{filePath}'", new CancellationToken());
                    return new Result<byte[]>(fileStream);
                }
                catch (Exception exception)
                {
                    if (exception.GetBaseException().Message.Contains("not found"))
                        return new Result<byte[]>(ResultStatus.NotFound, exception).WithReason(exception.GetBaseException().Message);
                    return new Result<byte[]>(exception).WithReason(exception.Message);
                }
                finally
                {
                    client.Disconnect();
                }
            }
            catch (Exception exception)
            {
                return new Result<byte[]>(exception).WithReason(exception.Message);
            }
        }

        #endregion Methods (Helpers)

    }
}
