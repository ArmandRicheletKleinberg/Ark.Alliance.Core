using System.Collections.Generic;
using System.Data;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Ark.Data
{
    /// <summary>
    /// This class extends the JToken Type.
    /// </summary>
    public static class JTokenExtensibility
    {
        /// <summary>
        /// Converts a JToken instance to a filled data table.
        /// It can either be an array as a JArray or a single row line with a single JObject.
        /// The data table columns are created given the json type and the rows are filled with the inner values of the JToken.
        /// </summary>
        /// <param name="jToken">The JToken to convert to data table.</param>
        /// <param name="dataTableName">The name of the data table to create.</param>
        /// <returns>The created data table.</returns>
        public static DataTable ToDataTable(this JToken jToken, string dataTableName)
        {
            var jTokens = jToken is JArray jArray ? jArray.Children().ToList() : new List<JToken> { jToken };

            var dataTable = new DataTable(dataTableName);
            jTokens[0].Children().OfType<JProperty>().ForEach(p => dataTable.Columns.Add(p.Name, p.Value.Type.ToPrimitiveType()));
            jTokens.ForEach(t =>
                dataTable.Rows.Add(t.Children().OfType<JProperty>().Select(v => v.Value.ToObject(v.Value.Type.ToPrimitiveType())).ToArray()));

            return dataTable;
        }
    }
}