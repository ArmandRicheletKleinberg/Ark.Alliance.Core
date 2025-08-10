using System;
using System.Collections.Generic;
using System.Data;
using Ark;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedType.Global

namespace Ark.Data.EFCore
{
    /// <summary>
    /// Represents a parameter used with SQL functions and stored procedures.
    /// <para>+ Maps CLR types to provider-specific <see cref="DbType"/> values.</para>
    /// <para>- Misconfigured values may cause runtime casting errors.</para>
    /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.data.common.dbparameter"/>
    /// </summary>
    public abstract class DbParameter
    {
        #region Fields (Static)

        /// <summary>
        /// Mapping between common CLR types and database types.
        /// <para>+ Caches lookups to avoid repeated reflection.</para>
        /// <para>- New types must be added manually to the dictionary.</para>
        /// </summary>
        protected static readonly Dictionary<Type, DbType> DbTypesMap = new Dictionary<Type, DbType>
        {
            [typeof(byte)] = DbType.Byte,
            [typeof(sbyte)] = DbType.SByte,
            [typeof(short)] = DbType.Int16,
            [typeof(ushort)] = DbType.UInt16,
            [typeof(int)] = DbType.Int32,
            [typeof(uint)] = DbType.UInt32,
            [typeof(long)] = DbType.Int64,
            [typeof(ulong)] = DbType.UInt64,
            [typeof(float)] = DbType.Single,
            [typeof(double)] = DbType.Double,
            [typeof(decimal)] = DbType.Decimal,
            [typeof(bool)] = DbType.Boolean,
            [typeof(string)] = DbType.String,
            [typeof(char)] = DbType.StringFixedLength,
            [typeof(System.Guid)] = DbType.Guid,
            [typeof(DateTime)] = DbType.DateTime,
            [typeof(DateTimeOffset)] = DbType.DateTimeOffset,
            [typeof(byte[])] = DbType.Binary,
            [typeof(byte?)] = DbType.Byte,
            [typeof(sbyte?)] = DbType.SByte,
            [typeof(short?)] = DbType.Int16,
            [typeof(ushort?)] = DbType.UInt16,
            [typeof(int?)] = DbType.Int32,
            [typeof(uint?)] = DbType.UInt32,
            [typeof(long?)] = DbType.Int64,
            [typeof(ulong?)] = DbType.UInt64,
            [typeof(float?)] = DbType.Single,
            [typeof(double?)] = DbType.Double,
            [typeof(decimal?)] = DbType.Decimal,
            [typeof(bool?)] = DbType.Boolean,
            [typeof(char?)] = DbType.StringFixedLength,
            [typeof(System.Guid?)] = DbType.Guid,
            [typeof(DateTime?)] = DbType.DateTime,
            [typeof(DateTimeOffset?)] = DbType.DateTimeOffset
        };

        #endregion Fields (Static)

        #region Constructors

        /// <summary>
        /// Creates a <see cref="DbParameter"/> instance.
        /// <para>+ Infers <see cref="DbType"/> using <see cref="DbTypesMap"/>.</para>
        /// <para>- Defaults to <see cref="DbType.Object"/> when type is unknown.</para>
        /// </summary>
        /// <param name="type">The .NET type of the parameter.</param>
        /// <param name="value">The value to store in the parameter.</param>
        /// <param name="name">The name of the parameter (optional).</param>
        /// <param name="direction">The direction of the parameter (optional).</param>
        protected DbParameter(Type type, object value, string name = null, ParameterDirection direction = ParameterDirection.Input)
        {
            Type = type;
            DbType = DbTypesMap.GetValue(type, DbType.Object);
            Value = value;
            Name = name;
            Direction = direction;
        }

        #endregion Constructors

        #region Properties (Public)

        /// <summary>
        /// Name of the parameter as referenced in SQL.
        /// <para>+ Enables positional independence.</para>
        /// <para>- Null names fall back to index-based placeholders.</para>
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// CLR type of the parameter value.
        /// <para>+ Guides conversion and validation.</para>
        /// <para>- Incorrect types may produce invalid casts.</para>
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Database type inferred from <see cref="Type"/>.
        /// <para>+ Used by ADO.NET when sending values.</para>
        /// <para>- <see cref="DbType.Object"/> indicates unsupported mapping.</para>
        /// </summary>
        public DbType DbType { get; }

        /// <summary>
        /// Value assigned to the parameter.
        /// <para>+ Supports nullable primitives and strings.</para>
        /// <para>- Complex objects require manual serialization.</para>
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Direction of the parameter (input, output, or both).
        /// <para>+ Matches <see cref="ParameterDirection"/> semantics.</para>
        /// <para>- Output parameters demand provider-specific handling.</para>
        /// </summary>
        public ParameterDirection Direction { get; set; }


        #endregion Properties (Public)

        #region Properties (Computed)

        /// <summary>
        /// Indicates whether <see cref="DbType"/> was resolved.
        /// <para>+ Prevents executing commands with unsupported types.</para>
        /// <para>- False positives occur for custom provider mappings.</para>
        /// </summary>
        public bool IsDbTypeValid
            => DbType != DbType.Object;

        #endregion Properties (Computed)

        #region Methods (Public)

        /// <summary>
        /// Gets the SQL-safe name of the parameter for raw queries.
        /// <para>+ Falls back to positional <c>p#</c> placeholders when unnamed.</para>
        /// <para>- Caller must supply the correct format for each provider.</para>
        /// </summary>
        /// <param name="format">Format string for provider-specific parameter naming.</param>
        /// <param name="index">Index used when <see cref="Name"/> is not provided.</param>
        /// <returns>Resolved parameter name suitable for SQL text.</returns>
        public string GetSqlName(string format, int index = 0)
            => string.Format(format, Name ?? $"p{index}");

        #endregion Methods (Public)
    }

    /// <inheritdoc />
    /// <summary>
    /// Strongly typed parameter wrapper.
    /// <para>+ Provides compile-time safety for parameter values.</para>
    /// <para>- Generic type must match <see cref="DbParameter.Value"/> at runtime.</para>
    /// </summary>
    /// <typeparam name="TParam">.NET type of the parameter.</typeparam>
    public class DbParameter<TParam> : DbParameter
    {
        #region Constructors

        /// <inheritdoc />
        /// <summary>
        /// Creates a <see cref="T:DbParameter" /> instance.
        /// </summary>
        /// <param name="value">The value of the parameter.</param>
        /// <param name="name">The name of the parameter (optional).</param>
        /// <param name="direction">The direction of the parameter (optional).</param>

        public DbParameter(TParam value, string name = null, ParameterDirection direction = ParameterDirection.Input)
            : base(typeof(TParam), value, name, direction)
        { }

        #endregion Constructors
    }
}