namespace Ark
{
    /// <summary>
    /// + Extension utilities for working with collections.
    /// - Limited to legacy helpers slated for removal.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// + Wraps an item into a new collection for APIs requiring enumerables.
        /// - Allocates a new array; prefer collection expressions in new code.
        /// </summary>
        /// <typeparam name="T">Type of the item.</typeparam>
        /// <param name="item">Element to wrap.</param>
        /// <returns>Collection containing the single item.</returns>
        [Obsolete("Please replace uses of this extension method with collection expression. This method will be removed in Ark 17.")]
        public static ICollection<T> ToSingleItemCollection<T>(this T item) =>
            new T[] { item };
    }
}
