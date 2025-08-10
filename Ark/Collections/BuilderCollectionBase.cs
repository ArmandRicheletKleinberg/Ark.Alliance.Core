using System.Collections;

#nullable enable

namespace Ark
{
    /// <summary>
    /// Provides a lazy, read-only wrapper for builder collections using <see cref="LazyReadOnlyCollection{TItem}"/>.
    /// <para>+ Materializes items on first access and caches results.</para>
    /// <para>- Not thread-safe; synchronize externally if modified.</para>
    /// <para>Ref: <see href="https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable-1"/></para>
    /// </summary>
    /// <typeparam name="TItem">Type of the items.</typeparam>
    public abstract class BuilderCollectionBase<TItem> : IBuilderCollection<TItem>
    {
        #region Fields
        private readonly LazyReadOnlyCollection<TItem> _items;
        #endregion Fields

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="BuilderCollectionBase{TItem}"/> class.
        /// <para>+ Defers enumeration until items are requested.</para>
        /// <para>- Throws if <paramref name="items"/> returns <see langword="null"/>.</para>
        /// </summary>
        /// <param name="items">Factory delegate providing collection items.</param>
        public BuilderCollectionBase(Func<IEnumerable<TItem>> items) =>
            _items = new LazyReadOnlyCollection<TItem>(items);
        #endregion Constructors

        #region Properties
        /// <summary>
        /// Gets the number of items in the collection.
        /// <para>+ Caches the count after first enumeration.</para>
        /// <para>- Initial access enumerates the items factory.</para>
        /// </summary>
        public int Count => _items.Count;
        #endregion Properties

        #region Methods
        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// <para>+ Subsequent iterations reuse the cached items.</para>
        /// <para>- First iteration may allocate the underlying array.</para>
        /// </summary>
        /// <returns>An enumerator over the collection.</returns>
        public IEnumerator<TItem> GetEnumerator() => _items.GetEnumerator();

        /// <summary>
        /// Returns a non-generic enumerator.
        /// <para>Ref: <see href="https://learn.microsoft.com/dotnet/api/system.collections.ienumerable.getenumerator"/></para>
        /// </summary>
        /// <returns>An <see cref="IEnumerator"/> over the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        #endregion Methods
    }
}
