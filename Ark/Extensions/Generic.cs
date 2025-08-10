using Newtonsoft.Json;

namespace Ark
{
    /// <summary>
    /// This class extends any object as it takes only a generic type to work.
    /// </summary>
    public static class GenericExtensibility
    {
        /// <summary>
        /// Iterates a tree hierarchical data and gets the flat list of all the children.
        /// </summary>
        /// <typeparam name="T">The type of the tree.</typeparam>
        /// <param name="root">The root of the tree.</param>
        /// <param name="childrenFunction">The function to get the children back.</param>
        /// <returns>An enumeration with the flat list of children.</returns>
        public static IEnumerable<T> IterateTree<T>(this T root, Func<T, IEnumerable<T>> childrenFunction)
        {
            var list = new List<T> { root };
            while (list.Any())
            {
                var child = list[0];
                list.RemoveAt(0);
                list.AddRange(childrenFunction(child) ?? Enumerable.Empty<T>());
                yield return child;
            }
        }

        /// <summary>
        /// Perform a deep Copy of the object, using Json as a serialisation method.
        /// </summary>
        /// <typeparam name="T">The type of object being copied.</typeparam>
        /// <param name="source">The object instance to copy.</param>
        /// <returns>The copied object.</returns>
        public static T CloneJson<T>(this T source)
        {
            // Don't serialize a null object, simply return the default for that object
            if (ReferenceEquals(source, null)) return default(T);

            var serializerSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            var serialized = JsonConvert.SerializeObject(source, serializerSettings);

            // initialize inner objects individually
            // for example in default constructor some list property initialized with some values, but in 'source' these items are cleaned 
            // without ObjectCreationHandling.Replace default constructor values will be added to result
            var deserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace, TypeNameHandling = TypeNameHandling.All };
            return JsonConvert.DeserializeObject<T>(serialized, deserializeSettings);
        }
    }
}