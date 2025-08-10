using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;
using System.Threading.Tasks;

namespace Ark.Data
{
    /// <summary>
    /// This extension class extends the ADO .NET DbDataReader class.
    /// </summary>
    public static class DbDataReaderExtension
    {
        #region Fields

        /// <summary>
        /// This dictionary holds the result of the reflection to avoid using reflection each time.
        /// </summary>
        private static readonly Dictionary<Type, PropertyInfo[]> DataTypeProperties = new Dictionary<Type, PropertyInfo[]>();

        #endregion Fields

        /// <summary>
        /// Extracts the data from a data reader and converts it to a strongly typed instance array.
        /// </summary>
        /// <typeparam name="TData">The type of the data to create.</typeparam>
        /// <param name="dbDataReader">The data reader to extract the data from.</param>
        /// <returns></returns>
        public static async Task<List<TData>> ToListAsync<TData>(this DbDataReader dbDataReader)
            where TData : new()
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
    }
}