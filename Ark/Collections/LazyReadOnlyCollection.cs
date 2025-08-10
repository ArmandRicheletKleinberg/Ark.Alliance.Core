using System.Collections;

namespace Ark;

/// <summary>
/// Provides an <see cref="IReadOnlyCollection{T}"/> whose items are lazily materialized.
/// <para>+ Defers expensive enumeration until needed.</para>
/// <para>- Requires thread-safe lazy sources for concurrent access.</para>
/// <para>Ref: <see href="https://learn.microsoft.com/dotnet/api/system.lazy-1"/></para>
/// </summary>
/// <typeparam name="T">Type of items produced on demand.</typeparam>
/// <example>
/// <code language="csharp">
/// var lazy = new LazyReadOnlyCollection&lt;int&gt;(() =&gt; Enumerable.Range(0, 3));
/// var count = lazy.Count; // triggers enumeration once
/// </code>
/// </example>
public sealed class LazyReadOnlyCollection<T> : IReadOnlyCollection<T>
{
    #region Fields
    private readonly Lazy<IEnumerable<T>> _lazyCollection;
    private int? _count;
    #endregion Fields

    #region Constructors
    /// <summary>
    /// Initializes with an existing <see cref="Lazy{T}"/> sequence.
    /// <para>+ Reuses preconfigured lazy sources.</para>
    /// <para>- Null delegates lead to runtime errors.</para>
    /// </summary>
    /// <param name="lazyCollection">Lazy source providing the elements.</param>
    public LazyReadOnlyCollection(Lazy<IEnumerable<T>> lazyCollection) => _lazyCollection = lazyCollection;

    /// <summary>
    /// Initializes with a function executed lazily on first access.
    /// <para>+ Simplifies inline definitions.</para>
    /// <para>- Exceptions thrown by <paramref name="lazyCollection"/> are cached.</para>
    /// </summary>
    /// <param name="lazyCollection">Function generating the collection.</param>
    public LazyReadOnlyCollection(Func<IEnumerable<T>> lazyCollection) =>
        _lazyCollection = new Lazy<IEnumerable<T>>(lazyCollection);
    #endregion Constructors

    #region Properties
    /// <summary>
    /// Gets the underlying collection, materializing it if necessary.
    /// <para>+ Enumeration occurs at most once.</para>
    /// <para>- Subsequent mutations on source are ignored.</para>
    /// </summary>
    public IEnumerable<T> Value => EnsureCollection();

    /// <summary>
    /// Gets the number of items after materialization.
    /// <para>+ Caches the count to avoid multiple enumerations.</para>
    /// <para>- First access may incur enumeration cost.</para>
    /// </summary>
    public int Count
    {
        get
        {
            EnsureCollection();
            return _count.GetValueOrDefault();
        }
    }
    #endregion Properties

    #region Methods
    /// <summary>
    /// Returns an enumerator over the lazily materialized items.
    /// <para>+ Deferred execution until enumeration.</para>
    /// <para>- Enumeration uses cached collection and doesn't refresh.</para>
    /// </summary>
    /// <returns>An <see cref="IEnumerator{T}"/> over the collection.</returns>
    public IEnumerator<T> GetEnumerator() => Value.GetEnumerator();

    /// <summary>
    /// Returns a non-generic enumerator.
    /// <para>Ref: <see href="https://learn.microsoft.com/dotnet/api/system.collections.ienumerable.getenumerator"/></para>
    /// </summary>
    /// <returns>An <see cref="IEnumerator"/> for the collection.</returns>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    #endregion Methods

    #region Helpers
    private IEnumerable<T> EnsureCollection()
    {
        if (_lazyCollection == null)
        {
            _count = 0;
            return Enumerable.Empty<T>();
        }

        IEnumerable<T> val = _lazyCollection.Value;
        if (_count == null)
        {
            _count = val.Count();
        }

        return val;
    }
    #endregion Helpers
}
