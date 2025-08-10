namespace Ark
{
    /// <summary>
    /// This class helps playing with query string.
    /// It mainly converts a query string to dictionary and opposite.
    /// </summary>
    public static class QueryString
    {

        #region Methods

        /// <summary>
        /// Parses a query encoded string to a dictionary owning all the query string parameters.
        /// </summary>
        /// <param name="query">The query encoded string to parse.</param>
        /// <returns>The dictionary of the query string parameters.</returns>
        public static Dictionary<string, string> Parse(string query)
        {
            if (string.IsNullOrEmpty(query)) return new Dictionary<string, string>();

            return query.Split('?').Last().Split('&').Where(p => p.Contains("="))
                .ToDictionary(p => p.Substring(0, p.IndexOf('=')), p => p.Substring(p.IndexOf('=') + 1));
        }

        /// <summary>
        /// Converts a parameters dictionary to a query encoded string.
        /// </summary>
        /// <param name="parameters">The parameters to convert in query encoded string.</param>
        /// <returns>The dictionary of the query string parameters.</returns>
        public static string ToString(Dictionary<string, string> parameters)
        {
            return parameters?.Select(p => $"{p.Key}={p.Value}").Aggregate((p1, p2) => $"{p1}&{p2}");
        }

        #endregion Methods

    }
}
