using System.Collections.Generic;
using System.Linq;

namespace Ark;

/// <summary>
/// + Helper extensions for the <see cref="ILookup{TKey,TElement}"/> interface.
/// - Adds minimal overhead via method indirection.
/// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.linq.ilookup"/>
/// </summary>
// ReSharper disable once InconsistentNaming
public static class ILookupExtensions
{
    #region Methods (Public)

    /// <summary>
    /// Retrieves the collection associated with <paramref name="key"/> or returns
    /// an empty enumerable when the key is absent.
    /// </summary>
    /// <typeparam name="TK">Type of the key.</typeparam>
    /// <typeparam name="TV">Type of the values.</typeparam>
    /// <param name="lookup">Lookup instance on which the search is performed.</param>
    /// <param name="key">Key to locate.</param>
    /// <returns>A sequence of values if the key exists; otherwise an empty sequence.</returns>
    /// <remarks>
    /// Complexity: O(1) for lookups created from <see cref="ILookup{TKey,TElement}"/>.
    /// </remarks>
    public static IEnumerable<TV> GetValue<TK, TV>(this ILookup<TK, TV> lookup, TK key) =>
        lookup?.Contains(key) ?? false ? lookup[key] : Enumerable.Empty<TV>();

    #endregion Methods (Public)
}

