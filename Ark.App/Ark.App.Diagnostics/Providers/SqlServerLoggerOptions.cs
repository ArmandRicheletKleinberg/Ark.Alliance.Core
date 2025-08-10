namespace Ark.App.Diagnostics
{
    /// <summary>
    /// Options that describe the behavior for SQL SERVER database logging.
    /// </summary>
    public class SqlServerLoggerOptions : BatchingLoggerOptions
    {
        #region Properties (Public)

        /// <summary>
        /// The connection string used to connect the SQL SERVER.
        /// </summary>
        public string SqlServerConnectionString { get; set; }

        /// <summary>
        /// The name of the table with the logs in the SQL SERVER.
        /// </summary>
        public string SqlServerTableName { get; set; }

        #endregion Properties (Public)
    }
}