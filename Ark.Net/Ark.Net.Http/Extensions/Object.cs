using Ark;
using System;
using System.Linq;
using System.Text;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace Ark.Net.Http
{
    /// <summary>
    /// This class is used to extend <see cref="object"/> class.
    /// </summary>
    public static class ObjectExtensions
    {
        #region Methods (ToQueryString)

        /// <summary>
        /// Converts an object to a QueryString that can be used as an URL parameters.
        /// </summary>
        /// <example>{ Id = 1, TextToDisplay = "sample" } becomes id=1&amp;texttodisplay=sample.</example>
        /// <param name="obj">The object to convert.</param>
        /// <param name="propertiesToExclude">The properties to exclude in the QueryString.</param>
        /// <returns>The converted object.</returns>
        public static string ToQueryString(this object obj, string[] propertiesToExclude = null)
        {
            propertiesToExclude ??= new string[0];
            propertiesToExclude = propertiesToExclude.Select(p => p.ToLower()).ToArray();

            var queryStringBuilder = new StringBuilder();
            obj.GetType().GetProperties().ForEach(property =>
            {
                if (propertiesToExclude.Contains(property.Name.ToLower()))
                    return;

                var name = property.Name.ToLower();
                var value = property.GetValue(obj, null)?.ToString();
                if (value == null)
                    return;

                queryStringBuilder.Append($"{Uri.EscapeDataString(name)}={Uri.EscapeDataString(value)}&");
            });

            if (queryStringBuilder.Length > 0)
                queryStringBuilder.Remove(queryStringBuilder.Length - 1, 1); // Remove the last &

            return queryStringBuilder.ToString();
        }

        #endregion Methods (ToQueryString)
    }
}