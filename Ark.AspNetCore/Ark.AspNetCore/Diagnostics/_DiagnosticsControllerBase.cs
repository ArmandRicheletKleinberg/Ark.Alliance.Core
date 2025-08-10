using System.Threading.Tasks;
using Ark.App.Diagnostics;
using Ark.Data;
using Ark.Data.EFCore;
using Ark.Net.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ark.AspNetCore
{
    /// <inheritdoc />
    /// <summary>
    /// This is the base class used to give diagnostics details to a client app.
    /// This class should be overriden in another project controller to allow access to the diagnostics data.
    /// Security must be set to allow only super admin to connect this controller.
    /// </summary>
    [Authorize(nameof(UserCommonPermissionEnum.SeeDiagnostics))]
    [ApiExplorerSettings(GroupName = "ο Diagnostics")]
    public abstract class DiagnosticsControllerBase<TUserProfileData> : ControllerBase<TUserProfileData>
        where TUserProfileData : new()
    {
        #region Methods (Public)

        /// <summary>
        /// Gets the diagnostics info that is the reports available the categories of the log and some other module diagnostics API.
        /// </summary>
        /// <remarks>
        /// ## Permissions ##
        /// Allowed super admin user.
        /// ## Description ##
        /// This generic service is used in diagnostic page.
        /// ## Example ##
        /// ```
        /// GET api/diagnostics/info
        /// ```
        /// Gets the diagnostic info that is the reports available the categories of the log and some other module diagnostics API.
        /// </remarks>
        /// <response code="200">
        /// **Success** - The diagnostic info found are returned.
        /// **BadParameters** - Either the source or the log name.
        /// **Unexpected** - An unexpected error occurs.
        /// </response>
        /// <response code="401">User not authenticated.</response>
        /// <returns></returns>
        [HttpGet("diagnostics/info")]
        public Task<ResultDto<DiagnosticInfoDto>> GetDiagnosticInfo()
            => ExecuteBlAsync(() => new DiagnosticsRepository(SqlServerConnectionString, SqlServerLogsTableName).GetDiagnosticInfo());

        /// <summary>
        /// Returns all the filtered application logs.
        /// </summary>
        /// <param name="query">The query with the paging/filter/order to apply.</param>
        /// <param name="tableName">The name of the table to query.</param>
        /// <remarks>
        /// ## Permissions ##
        /// Allowed technical user of another app.
        /// ## Description ##
        /// This generic service is use in diagnostic page.
        /// ## Example ##
        /// ```
        /// GET api/diag/logs?filterTimeFrom=2019-03-05T11:01:28.365Z&amp;filterLevel=Information
        /// ```
        /// Gets the application logs crated after the 05/03/2018 11:01:28 with the level Information.
        /// </remarks>
        /// <response code="200">
        /// **Success** - The filtered logs found are returned.
        /// **BadParameters** - Either the source or the log name.
        /// **Unexpected** - An unexpected error occurs.
        /// </response>
        /// <response code="401">User not authenticated.</response>
        /// <returns></returns>
        [HttpPost("diagnostics/logs")]
        public Task<ResultDto<PaginatedDataDto<LogDto>>> GetLogs([FromBody] DataQueryDto query, [FromQuery] string tableName = null)
            => ExecuteBlAsync(() => new DiagnosticsRepository(SqlServerConnectionString, tableName ?? SqlServerLogsTableName).GetLogs(query));

        /// <summary>
        /// Returns all the app diagnostics indicators.
        /// </summary>
        /// <remarks>
        /// ## Permissions ##
        /// User with super admin permission.
        /// ## Description ##
        /// This generic service is use in diagnostic page.
        /// ## Example ##
        /// ```
        /// GET api/diagnostics/indicators
        /// ```
        /// Gets the application indicators.
        /// </remarks>
        /// <response code="200">
        /// **Success** - The indicators found are returned.
        /// **Unexpected** - An unexpected error occurs.
        /// </response>
        /// <response code="401">User not authenticated.</response>
        /// <returns></returns>
        [HttpGet("diagnostics/indicators")]
        public Task<ResultDto<IndicatorDto[]>> GetIndicators()
            => ExecuteBlAsync(() => new DiagnosticsRepository(SqlServerConnectionString, SqlServerLogsTableName).GetIndicators());

        /// <summary>
        /// Finds a report data given its name.
        /// </summary>
        /// <param name="reportName">The name of the report to request.</param>
        /// <remarks>
        /// ## Permissions ##
        /// User with super admin permission.
        /// ## Description ##
        /// This generic service is use in diagnostic page.
        /// ## Example ##
        /// ```
        /// GET api/reports/queueCertificatsInput
        /// ```
        /// Gets the report "queueCertificatsInput" with returns the first 10 messages of the queue input for Certificats.
        /// </remarks>
        /// <response code="200">
        /// **Success** - The raw report data is returned.
        /// **Unexpected** - An unexpected error occurs.
        /// </response>
        /// <response code="401">User not authenticated.</response>
        /// <returns></returns>
        [HttpGet("diagnostics/reports/{reportName}")]
        public virtual Task<ResultDto<object>> FindReport(string reportName)
            => ExecuteBlAsync(() => new DiagnosticsRepository(SqlServerConnectionString, SqlServerLogsTableName).GetReportData(reportName));

        #endregion Methods (Public)

        #region Properties (Abstract)

        /// <summary>
        /// The SQL SERVER connection string to connect the logs database.
        /// Gets from the Logging Configuration first from the connection string then from the database shortcut if any.
        /// </summary>
        protected virtual string SqlServerConnectionString
        {
            get
            {
                var configuration = HttpContext.RequestServices.GetRequiredService<IConfiguration>();
                var connectionString = configuration.GetSection($"Logging:SqlServer:{nameof(SqlServerLoggerOptions.SqlServerConnectionString)}").Get<string>();
                if (connectionString != null)
                    return connectionString;

                var database = configuration.GetSection($"Logging:SqlServer:Database").Get<string>();
                if (database == null)
                    return null;

                var sqlServerConnectionString = configuration.GetSection($"Databases:{database}:{nameof(DatabaseOptions.ConnectionString)}").Get<string>();
                return sqlServerConnectionString;
            }
        }

        /// <summary>
        /// The SQL SERVER logs database table name.
        /// </summary>
        protected virtual string SqlServerLogsTableName
            => HttpContext.RequestServices.GetRequiredService<IConfiguration>().GetSection($"Logging:SqlServer:{nameof(SqlServerLoggerOptions.SqlServerTableName)}").Get<string>();

        #endregion Properties (Abstract)
    }
}