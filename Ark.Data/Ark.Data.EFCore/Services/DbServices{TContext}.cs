using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Ark;
using Microsoft.EntityFrameworkCore;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedParameter.Global
// ReSharper disable VirtualMemberNeverOverridden.Global
// ReSharper disable MemberCanBeProtected.Global

namespace Ark.Data.EFCore
{
    /// <summary>
    /// Generic database services not tied to a single entity.
    /// + Executes functions or stored procedures and multi-entity transactions.
    /// - Requires callers to manage connection strings and migrations.
    /// Ref: <see href="https://learn.microsoft.com/ef/core/"/>.
    /// </summary>
    /// <typeparam name="TContext">The type of the database context for which the entity is linked to.</typeparam>
    public class DbServices<TContext>
        where TContext : DbContextEx, new()
    {
        #region Methods (UseTransaction)

        /// <summary>
        /// Executes a bunch of entities manipulations into a single transaction.
        /// This allows to use a transaction even or not related entities.
        /// If an unexpected error occurs, the transaction is rolled back.
        /// </summary>
        /// <param name="transaction">The builder used to create the manipulations transactions.</param>
        /// <returns>
        /// Success : The entire database transaction with all entities manipulation has succeeded.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public async Task<Result> UseTransaction(Action<DbTransactionBuilder<TContext>> transaction)
        {
            var builder = new DbTransactionBuilder<TContext>();
            transaction(builder);

            var result = await BuildAndExecuteTransaction(builder.Items);

            return result;
        }

        /// <summary>
        /// Givens the transaction builder created, it builds the transaction and executes it.
        /// </summary>
        /// <param name="items">The builder items created.</param>
        /// <returns>
        /// Success : The entire database transaction with all entities manipulation has succeeded.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        private static async Task<Result> BuildAndExecuteTransaction(List<DbTransactionBuilderItem> items)
        {
            await using var db = new TContext();
            await using var transaction = await db.Database.BeginTransactionAsync();

            try
            {
                foreach (var item in items)
                {
                    foreach (var entity in item.Entities)
                    {
                        var entry = db.Entry(entity);
                        entry.State = item.State;
                        if (item.UpdateOnlyTheseProperties.HasAnElement())
                            item.UpdateOnlyTheseProperties.ForEach(propertyName => entry.Property(propertyName).IsModified = true);
                        db.Attach(entity);
                    }
                }

                await db.SaveChangesAsync();

                await transaction.CommitAsync();

                return Result.Success;
            }
            catch (Exception exception)
            {
                return new Result(exception);
            }
        }

        #endregion Methods (UseTransaction)

        #region Methods (ExecuteFunction)

        /// <summary>
        /// Executes a raw SQL scalar function (returns only one simple result) given the function name and some parameters.
        /// </summary>
        /// <typeparam name="TReturn">The expected type of the scalar return value.</typeparam>
        /// <param name="sqlFunctionName">The SQL function name to call with its schema (ie dbo.Int).</param>
        /// <param name="parameters">The parameters to give to the function (only primitive types).</param>
        /// <returns>
        /// Success : The method calls has succeeded, the data contains the scalar returned value.
        /// BadParameters : The parameters given are not valid.
        /// BadPrerequisites : The scalar value to return can not be cast to the needed type.
        /// Unexpected: An unexpected error occurs.
        /// </returns>
        public virtual Task<Result<TReturn>> ExecuteFunctionWithScalarReturn<TReturn>(string sqlFunctionName, DbParameterCollection parameters)
            => ExecuteSqlCommand(
                sqlFunctionName != null ? $"SELECT * FROM {sqlFunctionName} ({parameters?.GetSqlParameterNamesList()})" : null,
                parameters,
                async command =>
                {
                    var result = await command.ExecuteScalarAsync();
                    if (!(result is TReturn castResult))
                        return new Result<TReturn>(ResultStatus.BadPrerequisites, new Exception($"Unable to cast the scalar return value type {result?.GetType()} to {typeof(TReturn)}"));

                    return new Result<TReturn>(castResult);
                });

        /// <summary>
        /// Executes a raw SQL table value function (returns dataset) given the function name and some parameters.
        /// </summary>
        /// <typeparam name="TData">The type of expected data if different from entity.</typeparam>
        /// <param name="sqlFunctionName">The SQL function name to call with its schema (ie dbo.Int).</param>
        /// <param name="parameters">The parameters to give to the function (only primitive types).</param>
        /// <returns>
        /// Success : The data returned from the function.
        /// BadParameters : The parameters given are not valid.
        /// Unexpected: An unexpected error occurs.
        /// </returns>
        public virtual Task<Result<TData[]>> ExecuteFunctionWithTableReturn<TData>(string sqlFunctionName, DbParameterCollection parameters)
            where TData : class, new()
            => ExecuteSqlCommand(
                sqlFunctionName != null ? $"SELECT * FROM {sqlFunctionName} ({parameters?.GetSqlParameterNamesList()})" : null,
                parameters,
                async command =>
                {
                    var dbDataReader = await command.ExecuteReaderAsync();
                    var values = (await dbDataReader.ToListAsync<TData>()).ToArray();
                    return new Result<TData[]>(values);
                });

        #endregion Methods (ExecuteFunction)

        #region Methods (ExecuteStoredProcedure)

        /// <summary>
        /// Executes a stored procedure which is not a query.
        /// </summary>
        /// <param name="sqlProcedureName">The SQL stored procedure name to call with its schema (ie dbo.TestStoredProcedure).</param>
        /// <param name="parameters">The parameters to give to the stored procedure (only primitive types).</param>
        /// <param name="callerMemberName">The caller member name.</param>
        /// <returns>
        /// Success : The store procedure execution has succeeded.
        /// BadParameters : Either the SQL procedure name is null or empty or one of the parameters is not valid.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public virtual Task<Result> ExecuteStoredProcedureNonQuery(string sqlProcedureName, DbParameterCollection parameters, [CallerMemberName] string callerMemberName = null)
            => ExecuteSqlCommand(
                sqlProcedureName != null ? $"EXECUTE {sqlProcedureName} {parameters.GetSqlParameterNamesList()}" : null,
                parameters,
                async command =>
                {
                    await command.ExecuteNonQueryAsync();
                    return Result.Success;
                });

        /// <summary>
        /// Executes a stored procedure which sends some entities in return.
        /// </summary>
        /// <param name="sqlProcedureName">The SQL stored procedure name to call with its schema (ie dbo.TestStoredProcedure).</param>
        /// <param name="parameters">The parameters to give to the stored procedure (only primitive types).</param>
        /// <returns>
        /// Success : The store procedure execution has succeeded and the table data are returned.
        /// BadParameters : Either the SQL procedure name is null or empty or one of the parameters is not valid.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public virtual Task<Result<TData[]>> ExecuteStoredProcedureWithTableReturn<TData>(string sqlProcedureName, DbParameterCollection parameters)
            where TData : class, new()
            => ExecuteSqlCommand(
                sqlProcedureName != null ? $"EXECUTE {sqlProcedureName} {parameters.GetSqlParameterNamesList()}" : null,
                parameters,
                async command =>
                {
                    var dbDataReader = await command.ExecuteReaderAsync();
                    var values = (await dbDataReader.ToListAsync<TData>()).ToArray();
                    return new Result<TData[]>(values);
                });

        #endregion Methods (ExecuteStoredProcedure)

        #region Methods (ExecuteSqlCommand)

        /// <summary>
        /// Executes a raw SQL command that returns either a scalar or table value.
        /// It is the underlying method of many function and stored procedure execution.
        /// </summary>
        /// <param name="sqlQuery">The SQL query to execute.</param>
        /// <param name="parameters">The parameters to give to the function (only primitive types).</param>
        /// <param name="commandFunction">The function to execute using the built command to get some result.</param>
        /// <returns>
        /// Success : The data returned from the function.
        /// BadParameters : The parameters given are not valid.
        /// Unexpected: An unexpected error occurs.
        /// </returns>
        protected virtual async Task<Result> ExecuteSqlCommand(string sqlQuery, DbParameterCollection parameters, Func<DbCommand, Task<Result>> commandFunction)
            => await ExecuteSqlCommand(sqlQuery, parameters, async command => { await commandFunction(command); return Result<object>.Success; });

        /// <summary>
        /// Executes a raw SQL command that returns either a scalar or table value.
        /// It is the underlying method of many function and stored procedure execution.
        /// </summary>
        /// <typeparam name="TReturn">The type of expected data to return.</typeparam>
        /// <param name="sqlQuery">The SQL query to execute.</param>
        /// <param name="parameters">The parameters to give to the function (only primitive types).</param>
        /// <param name="commandFunction">The function to execute using the built command to get some result.</param>
        /// <returns>
        /// Success : The data returned from the function.
        /// BadParameters : The parameters given are not valid.
        /// Unexpected: An unexpected error occurs.
        /// </returns>
        protected virtual async Task<Result<TReturn>> ExecuteSqlCommand<TReturn>(string sqlQuery, DbParameterCollection parameters, Func<DbCommand, Task<Result<TReturn>>> commandFunction)
        {
            try
            {
                if (sqlQuery.IsNullOrEmpty())
                    return Result<TReturn>.BadParameters.WithReason("The execution object name must be provided");
                if (!parameters?.Validate() ?? true)
                    return Result<TReturn>.BadParameters.WithReason("The parameters must be provided, even an empty list");

                await using var db = new TContext();
                await using var connection = db.Database.GetDbConnection();
                await using var command = connection.CreateCommand();

                command.CommandText = sqlQuery;
                parameters.FillCommandWithParameters(command);

                await connection.OpenAsync();
                var result = await commandFunction(command);
                return result;
            }
            catch (Exception exception)
            {
                return new Result<TReturn>(exception);
            }
        }

        #endregion Methods (ExecuteSqlCommand)
    }
}