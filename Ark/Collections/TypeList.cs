namespace Ark
{
    /// <summary>
    /// Represents a list of <see cref="Type"/> constrained to <typeparamref name="TBase"/>.
    /// <para>+ Enables type registration for reflection-driven scenarios.</para>
    /// <para>- Does not prevent duplicate entries; consumers must manage uniqueness.</para>
    /// <para>Ref: <see href="https://learn.microsoft.com/dotnet/api/system.type"/></para>
    /// </summary>
    /// <typeparam name="TBase">The base type.</typeparam>
    public class TypeList<TBase>
    {
        #region Fields
        private readonly List<Type> _list = new();
        #endregion Fields

        #region Methods (Public)
        /// <summary>
        /// Registers a <typeparamref name="T"/> in the list.
        /// <para>+ Generic constraint ensures <typeparamref name="T"/> derives from <typeparamref name="TBase"/>.</para>
        /// <para>- Duplicate registrations are allowed and may impact lookup cost.</para>
        /// <para>Ref: <see href="https://learn.microsoft.com/dotnet/csharp/programming-guide/generics/generic-type-parameters"/></para>
        /// </summary>
        /// <typeparam name="T">The type to add.</typeparam>
        public void Add<T>()
            where T : TBase =>
            _list.Add(typeof(T));

        /// <summary>
        /// Determines whether a type is in the list.
        /// <para>+ Simple API for small collections.</para>
        /// <para>- Performs a linear search; O(n) for large sets.</para>
        /// <para>Ref: <see href="https://learn.microsoft.com/dotnet/api/system.collections.generic.list-1.contains"/></para>
        /// </summary>
        /// <param name="type">Type to locate.</param>
        /// <returns><c>true</c> if the type exists; otherwise <c>false</c>.</returns>
        public bool Contains(Type type) => _list.Contains(type);
        #endregion Methods (Public)
    }
}
