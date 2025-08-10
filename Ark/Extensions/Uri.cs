namespace Ark
{
    /// <summary>
    /// This class extends the Uri class.
    /// </summary>
    public static class UriExtensions
    {
        #region Methods (Static)

        /// <summary>
        /// Appends some relative paths to an existing Uri.
        /// It basically removes slashes et adds a single slash between each paths.
        /// </summary>
        /// <param name="uri">The Uri to append.</param>
        /// <param name="paths">The paths to add to the Uri.</param>
        /// <returns>The appended Uri.</returns>
        public static Uri Append(this Uri uri, params string[] paths)
            => new(paths.Aggregate(uri.AbsoluteUri, (current, path) => $"{current.TrimEnd('/')}/{path.TrimStart('/')}"));

        #endregion Methods (Static)
    }
}