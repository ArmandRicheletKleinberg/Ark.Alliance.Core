

namespace Ark
{
    /// <summary>
    /// + Holds a lazily built list of <see cref="IMapDefinition"/> instances.
    /// - Items are evaluated only when enumerated.
    /// Ref: <see href="https://learn.microsoft.com/dotnet/csharp/iterators"/>
    /// </summary>
    public class MapDefinitionCollection : BuilderCollectionBase<IMapDefinition>
    {
        /// <summary>
        /// + Initializes the collection with a factory that yields map definitions.
        /// - The factory may be invoked multiple times if the collection is rebuilt.
        /// </summary>
        /// <param name="items">Function returning the map definitions to include.</param>
        public MapDefinitionCollection(Func<IEnumerable<IMapDefinition>> items)
            : base(items)
        {
        }
    }
}
