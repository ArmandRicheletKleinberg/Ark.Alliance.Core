using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ark.Data.EFCore;
using Ark.Net.Models;
using Microsoft.EntityFrameworkCore;

namespace Ark.App.Diagnostics
{
    /// <summary>
    /// Repository used to create and access a log table on a database.
    /// This repository is only for SQL SERVER.
    /// </summary>
    public class LogDbServices : DbServices<LogDbServices.DbContext, LogDbEntity>
    {
        #region Nested Class

        /// <summary>
        /// This DB context is intended to be used by the <see cref="LogDbServices"/>.
        /// </summary>
        public class DbContext : DbContextEx
        {
            #region Methods (Override)

            /// <inheritdoc />
            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                if (optionsBuilder.IsConfigured)
                    return;

                optionsBuilder.UseSqlServer(_sqlServerConnectionString);
            }

            /// <inheritdoc />
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<LogDbEntity>().ToTable(_sqlServerTableName);
            }

            #endregion Methods (Override)
        }

        #endregion Nested Class

        #region Fields

        /// <summary>
        /// The SQL SERVER connection string to connect the logs database.
        /// </summary>
        private static string _sqlServerConnectionString;

        /// <summary>
        /// The SQL SERVER logs table name.
        /// </summary>
        private static string _sqlServerTableName;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Creates a <see cref="LogDbServices"/> instance.
        /// </summary>
        /// <param name="sqlServerConnectionString">The SQL SERVER connection string to connect the logs database.</param>
        /// <param name="sqlServerTableName">The SQL SERVER logs table name.</param>
        public LogDbServices(string sqlServerConnectionString, string sqlServerTableName = "_logs")
        {
            _sqlServerConnectionString = sqlServerConnectionString;
            _sqlServerTableName = sqlServerTableName;
        }

        #endregion Constructors

        #region Properties (Public)

        /// <summary>
        /// Creates the logs table if it does not exist.
        /// It uses internally raw SQL SERVER code so use it only with this database.
        /// </summary>
        /// <returns>
        /// Success : The table already exists or has been successfully created.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public async Task<Result> CreateLogsTableIfNotExists()
        {
            try
            {
                await using var db = new DbContext();
                await using var connection = db.Database.GetDbConnection();
                await using var command = connection.CreateCommand();

                // Creates the SQL command to be executed
                command.CommandText =
                    $"IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'{_sqlServerTableName}')" +
                    "BEGIN " +
                    $"CREATE TABLE [dbo].[{_sqlServerTableName}](" +
                    "[Id] INT IDENTITY(1, 1) PRIMARY KEY NOT NULL," +
                    "[Timestamp] DATETIME2 NOT NULL DEFAULT GETUTCDATE()," +
                    "[LogLevel] INT NOT NULL, " +
                    "[Category] VARCHAR(254) NOT NULL," +
                    "[Message] VARCHAR(MAX) NULL," +
                    "[Exception] VARCHAR(MAX) NULL," +
                    "INDEX IX_Logs_Timestamp([Timestamp])," +
                    "INDEX IX_Logs_LogLevel([LogLevel])," +
                    "INDEX IX_Logs_Category([Category])" +
                    ")" +
                    "END";

                command.Connection = connection;
                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
                return Result.Success;
            }
            catch (Exception exception)
            {
                return new Result(exception);
            }
        }

        /// <summary>
        /// Insert logs in one batch into the database logs table.
        /// </summary>
        /// <param name="logs">The logs to insert into the database.</param>
        /// <returns>
        /// Success : The table already exists or has been successfully created.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public async Task<Result> InsertLogs(IEnumerable<LogDbEntity> logs)
        {
            try
            {
                await using var db = new DbContext();
                await db.AddRangeAsync(logs);
                await db.SaveChangesAsync();
                return Result.Success;
            }
            catch (Exception exception)
            {
                return new Result(exception);
            }
        }

        /// <summary>
        /// Get the logs tables which are tables prefixed by _logs.
        /// </summary>
        /// <returns>
        /// Success : The tables founds.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public Task<Result<string[]>> GetLogsTables() => Result<string[]>.SafeExecute(async () =>
        {
            await using var db = new DbContext();
            await using var connection = db.Database.GetDbConnection();
            await using var command = connection.CreateCommand();
            command.CommandText = "SELECT [name] FROM sys.[tables] WHERE [name] LIKE '_logs%'";

            command.Connection = connection;
            await connection.OpenAsync();
            var dbDataReader = await command.ExecuteReaderAsync();

            var tables = new List<string>();
            while (await dbDataReader.ReadAsync())
                tables.Add(dbDataReader[0] as string);

            return new Result<string[]>(tables.IfNotNull().ToArray());
        });

        /// <summary>
        /// Gets the categories available for the logs.
        /// </summary>
        /// <param name="tableName">The name of the table to get the categories from.</param>
        /// <returns>
        /// Success : The logs categories to filter on.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public Task<Result<string[]>> GetLogsCategories(string tableName) => Result<string[]>.SafeExecute(async () =>
        {
            await using var db = new DbContext();
            await using var connection = db.Database.GetDbConnection();
            await using var command = connection.CreateCommand();
            command.CommandText = $"SELECT DISTINCT [Category] FROM [{tableName}] ORDER BY [Category]";

            command.Connection = connection;
            await connection.OpenAsync();
            var dbDataReader = await command.ExecuteReaderAsync();

            var tables = new List<string>();
            while (await dbDataReader.ReadAsync())
                tables.Add(dbDataReader[0] as string);

            return new Result<string[]>(tables.IfNotNull().ToArray());
        });

        /// <summary>
        /// Perform a performance-optimized query (filter, order, paging) on a table or better a dedicated view.
        /// Only works with SQL Server database for single key table/view.
        /// All the names given in the <see cref="DataQueryDto"/> must be the same than this <see cref="LogDbEntity"/> name.
        /// </summary>
        /// <param name="query">The info(filter, order, paging) of the query.</param>
        /// <returns>
        /// Success : The paginated data found is returned.
        /// BadPrerequisites : the context of the database must inherit from <see cref="DbContextEx"/> and the entity must not have a composed key.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public virtual Task<Result<PaginatedDataDbEntity<LogDbEntity>>> Query(DataQueryDto query)
            => base.Query(new DataQueryDbEntity<DbContext, LogDbEntity>(query, new Dictionary<string, string>
            {
                [nameof(LogDto.Severity)] = nameof(LogDbEntity.LogLevel),
                [nameof(LogDto.CreationTime)] = nameof(LogDbEntity.Timestamp)
            }));

        #endregion Properties (Public)
    }
}