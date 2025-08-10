using Ark;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ark.Net.Http
{
    /// <summary>
    /// This helper class extends the Dictionary object.
    /// </summary>
    public static class DictionaryExtensions
    {
        #region Methods (Static)

        /// <summary>
        /// Converts a dictionary to a query string or empty if no elements.
        /// </summary>
        /// <example>
        /// ["skip"] = 0, ["take"] = 100 => ?skip=0&amp;take=100.
        /// ["array"] = new { 1, 2, 3 } => ?array=1&amp;array=2&amp;array=3
        /// </example>
        /// <typeparam name="TK">The type of the dictionary key.</typeparam>
        /// <typeparam name="TV">The type of the dictionary value.</typeparam>
        /// <param name="dictionary">The dictionary to convert to a query string.</param>
        /// <returns>The query string created or string empty if no elements.</returns>
        public static string ToQueryString<TK, TV>(this IDictionary<TK, TV> dictionary)
        {
            if (dictionary.HasNoElements())
                return string.Empty;

            var items = dictionary.Where(kvp => kvp.Value != null).Select(kvp =>
            {
                var key = HttpUtility.UrlEncode(kvp.Key.ToString());
                if (kvp.Value is string)
                    return $"{key}={HttpUtility.UrlEncode(kvp.Value.ToString())}";
                if (kvp.Value is IEnumerable enumerable)
                    return string.Join("&", enumerable.Cast<object>().Select(value => $"{key}={HttpUtility.UrlEncode(value.ToString())}"));

                return $"{key}={HttpUtility.UrlEncode(kvp.Value.ToString())}";
            });

            return $"?{string.Join("&", items)}";
        }

        #endregion Methods (Static)
    }
}