namespace Ark
{
    /// <summary>
    /// Represents an immutable enumeration of items produced by a builder.
    /// <para>+ Guarantees consistent count and iteration order.</para>
    /// <para>- Items cannot be modified once materialized.</para>
    /// <para>Ref: <see href="https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable-1"/></para>
    /// </summary>
    /// <typeparam name="TItem">Type of the items.</typeparam>
    public interface IBuilderCollection<out TItem> : IEnumerable<TItem>
    {
        /// <summary>
        /// Gets the number of items in the collection.
        /// <para>+ O(1) once items are materialized.</para>
        /// <para>- May trigger enumeration on first access.</para>
        /// </summary>
        int Count { get; }
    }
}
