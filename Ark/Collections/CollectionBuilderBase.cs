using Microsoft.Extensions.DependencyInjection;

#nullable enable

namespace Ark
{
    /// <summary>
    /// Builds and registers <see cref="IBuilderCollection{TItem}"/> implementations with dependency injection.
    /// <para>+ Ensures collection and items share the same <see cref="ServiceLifetime"/>.</para>
    /// <para>- Single-use; repeated registrations throw <see cref="InvalidOperationException"/>.</para>
    /// <para>Ref: <see href="https://learn.microsoft.com/dotnet/core/extensions/dependency-injection"/></para>
    /// </summary>
    /// <typeparam name="TBuilder">Type of the builder.</typeparam>
    /// <typeparam name="TCollection">Type of the collection.</typeparam>
    /// <typeparam name="TItem">Type of the items.</typeparam>
    public abstract class CollectionBuilderBase<TBuilder, TCollection, TItem> : ICollectionBuilder<TCollection, TItem>
        where TBuilder : CollectionBuilderBase<TBuilder, TCollection, TItem>
        where TCollection : class, IBuilderCollection<TItem>
    {
        #region Fields
        private readonly Lock _locker = new();
        private readonly List<Type> _types = new();
        private Type[]? _registeredTypes;
        #endregion Fields

        #region Properties
        /// <summary>
        /// Gets the lifetime applied to the collection and its items.
        /// <para>+ Defaults to <see cref="ServiceLifetime.Singleton"/>.</para>
        /// <para>- Override to use scoped or transient lifetimes.</para>
        /// </summary>
        protected virtual ServiceLifetime CollectionLifetime => ServiceLifetime.Singleton;
        #endregion Properties

        #region Methods
        /// <summary>
        /// Registers the builder with the specified <see cref="IServiceCollection"/>.
        /// <para>+ Adds both the collection and its item types to the container.</para>
        /// <para>- Throws if invoked more than once.</para>
        /// </summary>
        /// <param name="services">DI service collection.</param>
        public virtual void RegisterWith(IServiceCollection services)
        {
            if (_registeredTypes != null)
            {
                throw new InvalidOperationException("This builder has already been registered.");
            }

            // register the collection
            services.Add(new ServiceDescriptor(typeof(TCollection), CreateCollection, CollectionLifetime));

            // register the types
            RegisterTypes(services);
        }

        /// <summary>
        /// Creates a collection instance.
        /// <para>+ Respects registration order of items.</para>
        /// <para>- Each call resolves all items, which may be costly.</para>
        /// </summary>
        /// <param name="factory">Service provider used to resolve dependencies.</param>
        /// <returns>The constructed collection. JSON: serialized object.</returns>
        /// <remarks>Creates a new collection each time it is invoked.</remarks>
        public virtual TCollection CreateCollection(IServiceProvider factory) =>
            factory.CreateInstance<TCollection>(CreateItemsFactory(factory));

        /// <summary>
        /// Exposes the registered item types as an immutable sequence.
        /// <para>+ Useful for diagnostics or custom ordering.</para>
        /// <para>- Modifying the returned list has no effect.</para>
        /// </summary>
        /// <returns>Sequence of registered types.</returns>
        public IEnumerable<Type> GetTypes() => _types;

        /// <summary>
        /// Determines whether the collection contains the specified type.
        /// <para>+ Generic overload avoids runtime <see cref="Type"/> instances.</para>
        /// <para>- Requires the type to be known at compile time.</para>
        /// </summary>
        /// <typeparam name="T">Type to search for.</typeparam>
        /// <returns><see langword="true"/> if present; otherwise <see langword="false"/>. JSON: <c>true</c>/<c>false</c>.</returns>
        /// <remarks>
        /// Some builders may surface this as a public <c>Has&lt;T&gt;()</c> method when it is meaningful.
        /// </remarks>
        public virtual bool Has<T>()
            where T : TItem =>
            _types.Contains(typeof(T));

        /// <summary>
        /// Determines whether the collection contains the specified <see cref="Type"/>.
        /// <para>+ Validates compatibility with <typeparamref name="TItem"/>.</para>
        /// <para>- Throws if <paramref name="type"/> is not assignable.</para>
        /// </summary>
        /// <param name="type">Type to search for.</param>
        /// <returns><see langword="true"/> if present; otherwise <see langword="false"/>. JSON: <c>true</c>/<c>false</c>.</returns>
        /// <remarks>
        /// Some builders may expose this as a public <c>Has(Type)</c> helper when appropriate.
        /// </remarks>
        public virtual bool Has(Type type)
        {
            EnsureType(type, "find");
            return _types.Contains(type);
        }

        /// <summary>
        /// Configures the internal list of types.
        /// <para>+ Allows fluent registration before DI integration.</para>
        /// <para>- Throws if called after <see cref="RegisterWith"/>.</para>
        /// </summary>
        /// <param name="action">Action manipulating the underlying list.</param>
        /// <remarks>Throws if the types have already been registered.</remarks>
        protected void Configure(Action<List<Type>> action)
        {
            lock (_locker)
            {
                if (_registeredTypes != null)
                {
                    throw new InvalidOperationException(
                        "Cannot configure a collection builder after it has been registered.");
                }

                action(_types);
            }
        }

        /// <summary>
        /// Provides the list of types to register with the container.
        /// <para>+ Enables filtering or ordering before registration.</para>
        /// <para>- Default implementation returns the input unchanged.</para>
        /// </summary>
        /// <param name="types">Internal list of types.</param>
        /// <returns>Enumeration of types to register.</returns>
        /// <remarks>Used by implementations to add or reorder types.</remarks>
        protected virtual IEnumerable<Type> GetRegisteringTypes(IEnumerable<Type> types) => types;

        /// <summary>
        /// Creates the collection items using the registered types.
        /// <para>+ Respects the registration order.</para>
        /// <para>- Throws if <see cref="_registeredTypes"/> is <see langword="null"/>.</para>
        /// </summary>
        /// <param name="factory">Service provider used to resolve items.</param>
        /// <returns>Collection items.</returns>
        protected virtual IEnumerable<TItem> CreateItems(IServiceProvider factory)
        {
            if (_registeredTypes == null)
            {
                throw new InvalidOperationException(
                    "Cannot create items before the collection builder has been registered.");
            }

            return _registeredTypes // respect order
                .Select(x => CreateItem(factory, x))
                .ToArray(); // safe
        }

        /// <summary>
        /// Creates a single collection item.
        /// <para>+ Uses <see cref="ServiceProviderServiceExtensions.GetRequiredService(IServiceProvider, Type)"/> for resolution.</para>
        /// <para>- Resolving transient dependencies on each call may allocate.</para>
        /// </summary>
        /// <param name="factory">Service provider.</param>
        /// <param name="itemType">Type to resolve.</param>
        /// <returns>The resolved item.</returns>
        protected virtual TItem CreateItem(IServiceProvider factory, Type itemType) =>
            (TItem)factory.GetRequiredService(itemType);

        /// <summary>
        /// Ensures the specified <see cref="Type"/> derives from <typeparamref name="TItem"/>.
        /// <para>+ Prevents invalid registrations at runtime.</para>
        /// <para>- Reflection checks may incur minor overhead.</para>
        /// </summary>
        /// <param name="type">Type to validate.</param>
        /// <param name="action">Contextual verb for exception messages.</param>
        /// <returns>The validated type.</returns>
        protected Type EnsureType(Type type, string action)
        {
            if (typeof(TItem).IsAssignableFrom(type) == false)
            {
                throw new InvalidOperationException(
                    $"Cannot {action} type {type.FullName} as it does not inherit from/implement {typeof(TItem).FullName}.");
            }

            return type;
        }

        private void RegisterTypes(IServiceCollection services)
        {
            lock (_locker)
            {
                if (_registeredTypes != null)
                {
                    return;
                }

                Type[] types = GetRegisteringTypes(_types).ToArray();

                // ensure they are safe
                foreach (Type type in types)
                {
                    EnsureType(type, "register");
                }

                // register them - ensuring that each item is registered with the same lifetime as the collection.
                // NOTE: Previously each one was not registered with the same lifetime which would mean that if there
                // was a dependency on an individual item, it would resolve a brand new transient instance which isn't what
                // we would expect to happen. The same item should be resolved from the container as the collection.
                foreach (Type type in types)
                {
                    services.Add(new ServiceDescriptor(type, type, CollectionLifetime));
                }

                _registeredTypes = types;
            }
        }

        // used to resolve a Func<IEnumerable<TItem>> parameter
        private Func<IEnumerable<TItem>> CreateItemsFactory(IServiceProvider factory) => () => CreateItems(factory);
        #endregion Methods
    }
}
