using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;
using System.Threading.Tasks;
using Ark;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace Ark.Data.EFCore
{
    /// <summary>
    /// Extension utilities for ADO.NET <see cref="DbDataReader"/>.
    /// <para>+ Materializes rows into strongly typed objects without custom mappers.</para>
    /// <para>- Relies on reflection which may impact performance.</para>
    /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.data.common.dbdatareader"/>
    /// </summary>
    public static class DbDataReaderExtensions
    {
        #region Fields

        /// <summary>
        /// Cache of entity properties to reduce repeated reflection lookups.
        /// <para>+ Improves performance for repeated conversions.</para>
        /// <para>- Requires manual invalidation if types change.</para>
        /// </summary>
        private static readonly Dictionary<Type, PropertyInfo[]> DataTypeProperties = new Dictionary<Type, PropertyInfo[]>();

        #endregion Fields

        #region Methods (Public)

        /// <summary>
        /// Reads all rows from a <see cref="DbDataReader"/> and converts them to strongly typed instances.
        /// <para>+ Handles DBNull values transparently.</para>
        /// <para>- Uses reflection for each row which may allocate.</para>
        /// </summary>
        /// <typeparam name="TData">Type of the objects to create.</typeparam>
        /// <param name="dbDataReader">Reader providing the data.</param>
        /// <returns>List containing the converted records.</returns>
        public static async Task<List<TData>> ToListAsync<TData>(this DbDataReader dbDataReader)
        {
            // First searches the type properties either from cache or use reflections to get them
            var dataType = typeof(TData);
            var properties = DataTypeProperties.GetValue(dataType);
            if (properties == null)
            {
                properties = dataType.GetProperties();
                DataTypeProperties.Add(dataType, properties);
            }

            // Given the properties, creates the instances list from the DB data reader records given the property names
            var list = new List<TData>();
            while (await dbDataReader.ReadAsync())
            {
                var obj = (TData)dataType.New();
                properties.ForEach(property =>
                {
                    var value = dbDataReader[property.Name];
                    if (!Equals(value, DBNull.Value))
                        property.SetValue(obj, value, null);
                });
                list.Add(obj);
            }
            return list;
        }

        #endregion Methods (Public)
    }
}