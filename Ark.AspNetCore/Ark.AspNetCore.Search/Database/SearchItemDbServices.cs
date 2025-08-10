using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Ark.Data.EFCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Ark.AspNetCore.Search
{
    /// <summary>
    /// This is the services used to make a generic search into a SQL Server database given some entities query to search into.
    /// </summary>
    internal class SearchItemDbServices
    {
        #region Methods (GetSearchItems)

        /// <summary>
        /// Makes a global application search query and returns items that matches a specific text.
        /// The SQL query is mainly a UNION of all tables to search into with a TOP clause.
        /// The result of this SQL query is a search item with the value searched (ie. UM, order number) along with a possibly different id and the code of the type to search.
        /// </summary>
        /// <param name="typesByCode">The types of the search item that will contains how to fill the items extra data ordered by code.</param>
        /// <param name="text">The text to search for all search item types.</param>
        /// <param name="take">The maximum number of items to get (important for performance in UNION).</param>
        /// <param name="typeCodes">An optional filter on the code of the types of item to search.</param>
        /// <returns>
        /// Success : The items found are returned.
        /// BadParameters : At least one type should be searched for. Please check the type configuration and the filter.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public Task<Result<SearchItemDbEntity[]>> GetSearchItems(Dictionary<string, SearchItemType> typesByCode, string text, int take, string[] typeCodes) => Result<SearchItemDbEntity[]>.SafeExecute(async () =>
        {
            await using var db = SearchController.CreateDbContextFunc();
            var parameters = new Dictionary<string, SqlParameter>();
            var typesSql = typesByCode.Values
                .Where(type => typeCodes.HasNoElements() || typeCodes.Contains(type.Code))
                .Select(type => CreateSqlFromSearchType(type, db, text, take, parameters))
                .ToArray();
            if (typesSql.HasNoElements())
                return Result<SearchItemDbEntity[]>.BadParameters.WithReason("No types were found given the type codes to search into.");

            var unionSql = string.Join($"{Environment.NewLine}UNION{Environment.NewLine}", typesSql);
            var sql = $"SELECT TOP {take} Type, Id, Value FROM ({Environment.NewLine}{unionSql}{Environment.NewLine}) AS __Union__";

            await using var connection = db.Database.GetDbConnection();
            await using var command = connection.CreateCommand();

            command.CommandText = sql;
            foreach (var p in parameters.Values)
                command.Parameters.Add(p);

            await connection.OpenAsync();
            var result = await command.ExecuteReaderAsync();
            var items = await result.ToListAsync<SearchItemDbEntity>();

            return new Result<SearchItemDbEntity[]>(items.ToArray());
        });

        /// <summary>
        /// For a specific search item type, creates the SQL to query the database.
        /// This SQL is created from the IQueryable of the search item type and is manually modified to return always the same columns for each types.
        /// Moreover, the search are grouped by the value.
        /// All this SQL will be join in an overall Union SQL statement.
        /// </summary>
        /// <param name="type">The data of the specific type to search for items.</param>
        /// <param name="db">The database context.</param>
        /// <param name="text">The text to search for in the database search item types.</param>
        /// <param name="take">The maximum number of elements to take by type. The same as overall to avoid.</param>
        /// <param name="parameters">The query parameters to add the parameters of this query.</param>
        /// <returns>The transformed SQL query.</returns>
        private static string CreateSqlFromSearchType(SearchItemType type, DbContextEx db, string text, int take, Dictionary<string, SqlParameter> parameters)
        {
            var query = type.GetEfCoreQuery(db, text, take);
            var sql = query.ToSql(true);
            var sqlParameters = query.ExtractSqlParameters();
            sqlParameters
                .Where(kvp => !(kvp.Value?.GetType().IsComplex() ?? false))
                .ForEach(kvp => parameters.AddOrUpdate(kvp.Key, new SqlParameter(kvp.Key, SqlDbType.VarChar) { Value = kvp.Value?.ToString() }));
            var selectClause = sql.SubstringFrom("SELECT ").SubstringUntil($"{Environment.NewLine}FROM");
            var columns = ExtractColumnNamesFromSelectClause(selectClause);

            selectClause = $"SELECT TOP {take} '{type.Code}' AS [Type], {columns[0]} AS [Id], MAX({columns[^1]}) AS [Value]";
            sql = $"{selectClause}{Environment.NewLine}FROM{sql.SubstringFrom($"{Environment.NewLine}FROM")}{Environment.NewLine}GROUP BY {columns[0]}";

            return sql;
        }

        /// <summary>
        /// Extracts the column names from a SELECT SQL clause.
        /// The main difficulty is to avoid to split by comma as some CONVERT/CAST or other function can be used.
        /// So it splits only by comma outside brackets.
        /// </summary>
        /// <param name="selectClause">The SQL Server SELECT clause to extract the columns.</param>
        /// <returns>The columns extracted from the SELECT clause.</returns>
        private static string[] ExtractColumnNamesFromSelectClause(string selectClause)
        {
            var commaIndexes = new List<int>();
            var indent = 0;
            selectClause.Each((chr, index) =>
            {
                switch (chr)
                {
                    case '(': indent++; break;
                    case ')': indent--; break;
                    case ',': if (indent == 0) commaIndexes.Add(index + 1); break;
                }
            });
            var columns = selectClause.SplitByIndex(commaIndexes.ToArray()).Select(c => c.Trim(',', ' ')).ToArray();
            return columns;
        }

        #endregion Methods (GetSearchItems)

        #region Methods (FillItemsWithExtraData)

        /// <summary>
        /// Fills the searched item with extra data such as date and time and summary text.
        /// </summary>
        /// <param name="typesByCode">The types of the search item that will contains how to fill the items extra data ordered by code.</param>
        /// <param name="items">The items to fill with the extra data.</param>
        /// <returns>
        /// Success : when the items have been filled successfully.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public Task<Result> FillItemsWithExtraData(Dictionary<string, SearchItemType> typesByCode, SearchItemDbEntity[] items) => Result.SafeExecute(async () =>
        {
            var itemsByType = items.ToLookup(item => item.Type);
            var fillTasks = itemsByType
                .Select(group => typesByCode.GetValue(group.Key).FillItemsWithExtraData(group.ToArray()))
                .ToArray();
            await Task.WhenAll(fillTasks);

            return Result.Success;
        });

        #endregion Methods (FillItemsWithExtraData)
    }
}