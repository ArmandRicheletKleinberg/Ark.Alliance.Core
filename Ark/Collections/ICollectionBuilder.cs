using Microsoft.Extensions.DependencyInjection;

namespace Ark
{
    /// <summary>
    /// Represents a DI-aware builder that registers collections and their item types.
    /// <para>+ Centralizes registration logic.</para>
    /// <para>- Must be called before resolving the collection.</para>
    /// <para>Ref: <see href="https://learn.microsoft.com/dotnet/core/extensions/dependency-injection"/></para>
    /// </summary>
    public interface ICollectionBuilder
    {
        /// <summary>
        /// Registers the builder so it can create collections by registering the collection and its item types.
        /// <para>+ Ensures consistent <see cref="ServiceLifetime"/>.</para>
        /// <para>- Throws <see cref="InvalidOperationException"/> on duplicate registration.</para>
        /// </summary>
        /// <param name="services"><see cref="IServiceCollection"/> to register with.</param>
        void RegisterWith(IServiceCollection services);
    }

    /// <summary>
    /// Represents a typed collection builder.
    /// <para>+ Produces <typeparamref name="TCollection"/> instances on demand.</para>
    /// <para>- Each call may resolve multiple services.</para>
    /// </summary>
    /// <typeparam name="TCollection">Type of the collection.</typeparam>
    /// <typeparam name="TItem">Type of the items.</typeparam>
    public interface ICollectionBuilder<out TCollection, TItem> : ICollectionBuilder
        where TCollection : IBuilderCollection<TItem>
    {
        /// <summary>
        /// Creates a collection instance.
        /// <para>+ Respects item registration order.</para>
        /// <para>- Returns a new collection on each call.</para>
        /// </summary>
        /// <param name="factory"><see cref="IServiceProvider"/> used to resolve items.</param>
        /// <returns>The constructed collection.</returns>
        /// <remarks>Creates a new collection each time it is invoked.</remarks>
        TCollection CreateCollection(IServiceProvider factory);
    }
}
