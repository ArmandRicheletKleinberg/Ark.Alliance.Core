using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ark.Data.EFCore;
using Microsoft.Extensions.Logging;

namespace Ark.App.Diagnostics
{
    /// <summary>
    /// This entity contains the data of a diagnostic log.
    /// </summary>
    public class LogDbEntity : DbEntity<LogDbServices.DbContext>
    {
        #region Properties (Columns)

        /// <summary>
        /// The internal identifier of the log.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// The timestamp where the log has been created.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// The log level.
        /// </summary>
        public LogLevel LogLevel { get; set; }

        /// <summary>
        /// The category of the message to log.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// The message to log.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// The exception to log.
        /// </summary>
        public string Exception { get; set; }

        #endregion Properties (Columns)
    }
}