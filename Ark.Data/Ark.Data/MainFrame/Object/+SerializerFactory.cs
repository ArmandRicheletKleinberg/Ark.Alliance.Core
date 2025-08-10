using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace Ark.Data
{
    /// <summary>
    /// This factory is used to create a serializer used to serialize mainframe objects.
    /// </summary>
    internal static class MainFrameObjectSerializerFactory
    {
        #region Fields (Static)

        /// <summary>
        /// The already created serializers are kept in memory to save performance.
        /// It will not change at the runtime.
        /// </summary>
        private static readonly ConcurrentDictionary<Type, MainFrameObjectSerializer> Serializers = new ConcurrentDictionary<Type, MainFrameObjectSerializer>();

        #endregion Fields (Static)

        #region Methods (Static)

        /// <summary>
        /// Creates a serializer used to serialize mainframe objects.
        /// It uses a static cache to avoid to create a new serializer each call.
        /// </summary>
        /// <typeparam name="TMfo">The type of the mainframe object on which to create the serializer.</typeparam>
        /// <returns>The created serializer.</returns>
        internal static MainFrameObjectSerializer<TMfo> Create<TMfo>()
            where TMfo : class, new()
            => (MainFrameObjectSerializer<TMfo>)Create(typeof(TMfo));

        /// <summary>
        /// Creates a serializer used to serialize mainframe objects.
        /// It uses a static cache to avoid to create a new serializer each call.
        /// </summary>
        /// <param name="mainFrameObjectType">The type of the mainframe object on which to create the serializer.</param>
        /// <returns>The created serializer.</returns>
        internal static MainFrameObjectSerializer Create(Type mainFrameObjectType)
        {
            // Checks that the mainframe object has been well decorated using the MainFrameObject attribute
            if (mainFrameObjectType.GetCustomAttribute<MainFrameObjectAttribute>() == null)
                throw new Exception($"The mainframe object {mainFrameObjectType.Name} class must be decorated with the MainFrameObject attribute.");

            // Gets the serialize or creates it if not found
            var serializer = Serializers.GetOrAdd(mainFrameObjectType, type => (MainFrameObjectSerializer)typeof(MainFrameObjectSerializer<>).MakeGenericType(mainFrameObjectType).New());
            return serializer;
        }

        #endregion Methods (Static)
    }
}