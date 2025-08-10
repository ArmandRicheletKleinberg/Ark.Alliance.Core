using Microsoft.Extensions.DependencyInjection;


namespace Ark
{
    /// <summary>
    /// + Builder for registering <see cref="IMapDefinition"/> instances in DI.
    /// - Only supports transient lifetimes.
    /// Ref: <see href="https://learn.microsoft.com/dotnet/core/extensions/dependency-injection"/>
    /// </summary>
    public class MapDefinitionCollectionBuilder : SetCollectionBuilderBase<MapDefinitionCollectionBuilder, MapDefinitionCollection, IMapDefinition>
    {
        /// <summary>
        /// + Provides a fluent API by returning the current builder instance.
        /// - Thread safety is not guaranteed for concurrent registrations.
        /// </summary>
        protected override MapDefinitionCollectionBuilder This => this;

        /// <summary>
        /// + Specifies the DI lifetime used for the collection.
        /// - Hard-coded to <see cref="ServiceLifetime.Transient"/>.
        /// </summary>
        protected override ServiceLifetime CollectionLifetime => ServiceLifetime.Transient;
    }
}
