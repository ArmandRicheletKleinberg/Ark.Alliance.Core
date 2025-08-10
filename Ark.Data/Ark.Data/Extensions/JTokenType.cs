using System;
using Newtonsoft.Json.Linq;

namespace Ark.Data
{
    /// <summary>
    /// This class extends the JTokenType Type.
    /// </summary>
    public static class JTokenTypeExtensibility
    {
        /// <summary>
        /// Converts a JTokenType to a .NET primitive type.
        /// If the JToken type can not be converted to a primite type then typeof(object) is returned.
        /// </summary>
        /// <param name="jTokenType">The JToken type to get the .NET primitive type correspondance.</param>
        /// <returns>The .NET primitive type if found, typeof(object) otherwise.</returns>
        public static Type ToPrimitiveType(this JTokenType jTokenType)
        {
            switch (jTokenType)
            {
                case JTokenType.Integer: return typeof(int);
                case JTokenType.Float: return typeof(decimal);
                case JTokenType.String: return typeof(string);
                case JTokenType.Boolean: return typeof(bool);
                case JTokenType.Date: return typeof(DateTime);
                case JTokenType.Guid: return typeof(System.Guid);
                case JTokenType.TimeSpan: return typeof(TimeSpan);
                default: return typeof(object);
            }
        }
    }
}