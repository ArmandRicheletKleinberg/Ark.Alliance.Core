using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Ark;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedMember.Global

namespace Ark.Data.EFCore
{
    /// <summary>
    /// Collection of <see cref="DbParameter"/> instances for functions or stored procedures.
    /// <para>+ Provides utility helpers for validation and SQL name generation.</para>
    /// <para>- Currently tailored for SQL Server naming conventions.</para>
    /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.data.common.dbparameter"/>
    /// </summary>
    public class DbParameterCollection : List<DbParameter>
    {
        #region Constructors

        /// <inheritdoc />
        /// <summary>
        /// Creates a <see cref="DbParameterCollection"/> instance.
        /// <para>+ Accepts a variable number of parameters via params.</para>
        /// <para>- Throws <see cref="ArgumentNullException"/> when <paramref name="parameters"/> is null.</para>
        /// </summary>
        /// <param name="parameters">Parameters to include in the collection.</param>
        public DbParameterCollection(params DbParameter[] parameters)
            => AddRange(parameters);

        #endregion Constructors

        #region Methods (Public)

        /// <summary>
        /// Validates the SQL parameters given.
        /// <para>+ Ensures only mapped types are sent to the database.</para>
        /// <para>- Does not inspect parameter directions or sizes.</para>
        /// </summary>
        /// <returns>True if all parameters have a valid <see cref="DbParameter.DbType"/>.</returns>
        public bool Validate()
            => this.All(p => p.IsDbTypeValid);

        /// <summary>
        /// Builds a comma-separated list of parameter names.
        /// <para>+ Automatically generates placeholders (<c>@p#</c>) when names are missing.</para>
        /// <para>- Formatting is specific to SQL Server.</para>
        /// </summary>
        /// <returns>Comma-separated parameter names or an empty string when none exist.</returns>
        public string GetSqlParameterNamesList()
            => Count > 0 ? this.Select((p, i) => p.GetSqlName("@{0}", i)).Aggregate((p1, p2) => $"{p1},{p2}") : "";

        /// <summary>
        /// Adds parameters to an ADO.NET command.
        /// <para>+ Applies name formatting and null handling automatically.</para>
        /// <para>- Currently implements SQL Server specific conventions.</para>
        /// </summary>
        /// <param name="command">Command instance to populate with parameters.</param>
        public void FillCommandWithParameters(DbCommand command)
        {
            this.Each((p, i) =>
            {
                var dbParameter = command.CreateParameter();
                dbParameter.ParameterName = p.GetSqlName("{0}", i);
                dbParameter.DbType = p.DbType;
                dbParameter.Value = p.Value ?? DBNull.Value;
                dbParameter.Direction = p.Direction;
                command.Parameters.Add(dbParameter);
            });
        }

        #endregion Methods (Public)
    }
}