namespace Ark.Net.Ftp
{
    /// <summary>
    /// Represents basic configuration to connect to an FTP server.
    /// + Stores host, port, and user credentials.
    /// - Does not include advanced options like SSL or passive mode settings.
    /// </summary>
    public class FtpConnectionSettings
    {
        /// <summary>
        /// FTP server address.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// User name.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// FTP port.
        /// </summary>
        public int Port { get; set; }

    }
}
