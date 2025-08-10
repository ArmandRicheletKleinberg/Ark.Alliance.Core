using Ark;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace Ark.Net.Http
{
    /// <summary>
    /// This class is used to extend <see cref="string"/> class.
    /// </summary>
    public static class StringExtensions
    {
        #region Methods (Public)

        /// <summary>
        /// Creates a complex object instance from a HTML query string.
        /// </summary>
        /// <typeparam name="TObj">The complex type of the object to create.</typeparam>
        /// <param name="queryString">The query string to get the data from.</param>
        /// <returns>The created object.</returns>
        public static TObj GetFromQueryString<TObj>(this string queryString)
        {
            var nameValueCollection = HttpUtility.ParseQueryString(queryString.TrimStart('?'));
            var dictionary = nameValueCollection.Cast<string>().ToDictionary(k => k.ToLower(), v => nameValueCollection[v]);
            var obj = (TObj)typeof(TObj).New();
            var properties = typeof(TObj).GetProperties();
            foreach (var property in properties)
            {
                var stringValue = dictionary.GetValue(property.Name.ToLower());
                if (stringValue == null)
                    continue;
                var value = Convert.ChangeType(stringValue, property.PropertyType);
                if (value == null)
                    continue;

                property.SetValue(obj, value, null);
            }
            return obj;
        }

        /// <summary>
        /// Convert Html String to plain text
        /// </summary>
        /// <param name="str">The full string to convert</param>
        /// <returns></returns>
        public static string HtmlToPlainText(this string str)
            => Regex.Replace(str, "(&lt;(((?!/&gt;).)*)/&gt;)|(&lt;(((?!&gt;).)*)&gt;)|(<(((?!/>).)*)/>)|(<(((?!>).)*)>)", string.Empty);

        #endregion Methods (Public)
    }
}