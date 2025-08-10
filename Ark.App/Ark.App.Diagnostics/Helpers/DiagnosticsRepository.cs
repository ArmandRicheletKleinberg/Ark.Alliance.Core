using System;
using System.Linq;
using System.Threading.Tasks;
using Ark.Net.Models;
using Ark.Net.Models.Resources;

namespace Ark.App.Diagnostics
{
    /// <summary>
    /// This repository is used to manage the Windows event log.
    /// </summary>
    public class DiagnosticsRepository
    {
        #region Properties

        /// <summary>
        /// Provides access to log table management and queries.
        /// </summary>
        public LogDbServices LogDbServices { get; }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Creates a <see cref="DiagnosticsRepository"/> instance.
        /// </summary>
        /// <param name="sqlServerConnectionString">The SQL SERVER connection string to connect the logs database.</param>
        /// <param name="sqlServerTableName">The SQL SERVER logs table name.</param>
        public DiagnosticsRepository(string sqlServerConnectionString, string sqlServerTableName = "_logs")
        {
            LogDbServices = new LogDbServices(sqlServerConnectionString, sqlServerTableName);
        }

        #endregion Constructors

        #region Methods (Public)

        /// <summary>
        /// Gets a logs list filtered.
        /// </summary>
        /// <param name="query">The query details with the order, filter and paging.</param>
        /// <returns>
        /// Success : The logs list filtered.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public Task<Result<PaginatedDataDto<LogDto>>> GetLogs(DataQueryDto query) => Result<PaginatedDataDto<LogDto>>.SafeExecute(async () =>
        {
            var getResult = await LogDbServices.Query(query);
            if (getResult.IsNotSuccess)
                return new Result<PaginatedDataDto<LogDto>>(getResult);

            var paginatedData = getResult.Data.ToDto(l => new LogDto
            {
                CreationTime = l.Timestamp,
                Category = l.Category,
                Severity = (LogSeverityEnum)Enum.Parse(typeof(LogSeverityEnum), l.LogLevel.ToString()),
                Details = l.Message,
                Exception = l.Exception
            });

            return new Result<PaginatedDataDto<LogDto>>(paginatedData);
        });

        /// <summary>
        /// Gets all the indicators defined in the app.
        /// </summary>
        /// <returns>
        /// Success : The indicators are returned.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public Task<Result<IndicatorDto[]>> GetIndicators() => Task.Run(() =>
        {
            try
            {
                var indicators = DiagBase.Indicators?.Values.Select(i => new IndicatorDto
                {
                    Key = i.Key,
                    Label = i.Label,
                    Value = i.IsValueSet ? i.Value : "Aucune valeur reçue",
                    Status = i.Status
                }).ToArray();
                return new Result<IndicatorDto[]>(indicators);
            }
            catch (Exception exception)
            {
                return new Result<IndicatorDto[]>(exception);
            }
        });

        /// <summary>
        /// Gets all the reports name defined in the app.
        /// </summary>
        /// <returns>
        /// Success : The report names are returned.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public Result<ReportDto[]> GetReports() => Result<ReportDto[]>.SafeExecute(()
            => new Result<ReportDto[]>(DiagBase.Reports?.Values.Select(r => new ReportDto { Key = r.Key, Description = r.Description }).ToArray()));


        /// <summary>
        /// Gets all the reports name defined in the app.
        /// </summary>
        /// <returns>
        /// Success : The report names are returned.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public Task<Result<DiagnosticInfoDto>> GetDiagnosticInfo() => Result<DiagnosticInfoDto>.SafeExecute(async () =>
        {
            var logTablesResult = await LogDbServices.GetLogsTables();
            if (logTablesResult.IsNotSuccess)
                return new Result<DiagnosticInfoDto>(logTablesResult.AddReason("Unable to get the _logs database tables"));
            var logTables = logTablesResult.Data.Select(table => new LogTableDto { Name = table }).ToArray();

            foreach (var table in logTables)
            {
                var categoriesResult = await LogDbServices.GetLogsCategories(table.Name);
                if (categoriesResult.IsNotSuccess)
                    return new Result<DiagnosticInfoDto>(categoriesResult.AddReason($"Unable to get the categories of the {table.Name} database table"));
                table.Categories = categoriesResult.Data;
            }

            var info = new DiagnosticInfoDto
            {
                Reports = DiagBase.Reports?.Values.Select(r => new ReportDto { Key = r.Key, Description = r.Description }).ToArray(),
                Tables = logTables
            };

            return new Result<DiagnosticInfoDto>(info);
        });

        /// <summary>
        /// Gets the raw data of a diagnostics report given its name.
        /// </summary>
        /// <returns>
        /// Success : The raw data of a diagnostics report are returned.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public async Task<Result<object>> GetReportData(string reportName)
        {
            try
            {
                var report = DiagBase.Reports.GetValue(reportName);
                if (report == null)
                    return Result<object>.NotFound.WithReason($"A report method with the name {reportName} was not found.");

                var result = await report.GetFunction();
                return result;
            }
            catch (Exception exception)
            {
                return new Result<object>(exception);
            }
        }

        #endregion Methods (Public)
    }
}