namespace Ark
{
    #nullable enable

    /// <summary>
    /// Provides helpers for creating and working with topological graphs.
    /// <para>+ Supplies shared constants and factory methods.</para>
    /// <para>- Not intended for direct instantiation.</para>
    /// </summary>
    public class TopoGraph
    {
        internal const string CycleDependencyError = "Cyclic dependency.";
        internal const string MissingDependencyError = "Missing dependency.";

        /// <summary>
        /// Creates a new <see cref="Node{TKey, TItem}"/> instance.
        /// </summary>
        /// <typeparam name="TKey">Key type used to identify nodes.</typeparam>
        /// <typeparam name="TItem">Payload stored in the node.</typeparam>
        /// <param name="key">Identifier of the node.</param>
        /// <param name="item">Value carried by the node.</param>
        /// <param name="dependencies">Keys that must precede this node.</param>
        public static Node<TKey, TItem> CreateNode<TKey, TItem>(TKey key, TItem item, IEnumerable<TKey> dependencies) =>
            new(key, item, dependencies);

        /// <summary>
        /// Represents a node within the <see cref="TopoGraph"/>.
        /// <para>+ Stores its dependencies for sorting.</para>
        /// <para>- Immutable once created.</para>
        /// </summary>
        /// <typeparam name="TKey">Key type.</typeparam>
        /// <typeparam name="TItem">Item type.</typeparam>
        public class Node<TKey, TItem>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Node{TKey, TItem}"/> class.
            /// </summary>
            /// <param name="key">Identifier of this node.</param>
            /// <param name="item">Value carried by the node.</param>
            /// <param name="dependencies">Keys that must be processed before this node.</param>
            public Node(TKey key, TItem item, IEnumerable<TKey> dependencies)
            {
                Key = key;
                Item = item;
                Dependencies = dependencies;
            }

            /// <summary>Gets the node identifier.</summary>
            public TKey Key { get; }

            /// <summary>Gets the value associated with the node.</summary>
            public TItem Item { get; }

            /// <summary>Gets the keys this node depends on.</summary>
            public IEnumerable<TKey> Dependencies { get; }
        }
    }

    /// <summary>
    ///     Represents a generic DAG that can be topologically sorted.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys.</typeparam>
    /// <typeparam name="TItem">The type of the items.</typeparam>
    public class TopoGraph<TKey, TItem> : TopoGraph
        where TKey : notnull
    {
        private readonly Func<TItem, IEnumerable<TKey>?> _getDependencies;
        private readonly Func<TItem, TKey> _getKey;
        private readonly Dictionary<TKey, TItem> _items = new();

        /// <summary>
        ///     Initializes a new instance of the <see cref="TopoGraph{TKey, TItem}" /> class.
        /// </summary>
        /// <param name="getKey">A method that returns the key of an item.</param>
        /// <param name="getDependencies">A method that returns the dependency keys of an item.</param>
        public TopoGraph(Func<TItem, TKey> getKey, Func<TItem, IEnumerable<TKey>?> getDependencies)
        {
            _getKey = getKey;
            _getDependencies = getDependencies;
        }

        /// <summary>
        ///     Adds an item to the graph.
        /// </summary>
        /// <param name="item">The item.</param>
        public void AddItem(TItem item)
        {
            TKey key = _getKey(item);
            _items[key] = item;
        }

        /// <summary>
        ///     Adds items to the graph.
        /// </summary>
        /// <param name="items">The items.</param>
        public void AddItems(IEnumerable<TItem> items)
        {
            foreach (TItem item in items)
            {
                AddItem(item);
            }
        }

        /// <summary>
        ///     Gets the sorted items.
        /// </summary>
        /// <param name="throwOnCycle">A value indicating whether to throw on cycles, or just ignore the branch.</param>
        /// <param name="throwOnMissing">A value indicating whether to throw on missing dependency, or just ignore the dependency.</param>
        /// <param name="reverse">A value indicating whether to reverse the order.</param>
        /// <returns>The (topologically) sorted items.</returns>
        public IEnumerable<TItem> GetSortedItems(bool throwOnCycle = true, bool throwOnMissing = true, bool reverse = false)
        {
            var sorted = new TItem[_items.Count];
            var visited = new HashSet<TItem>();
            var index = reverse ? _items.Count - 1 : 0;
            var incr = reverse ? -1 : +1;

            foreach (TItem item in _items.Values)
            {
                Visit(item, visited, sorted, ref index, incr, throwOnCycle, throwOnMissing);
            }

            return sorted;
        }

        private static bool Contains(TItem[] items, TItem item, int start, int count) =>
            Array.IndexOf(items, item, start, count) >= 0;

        private void Visit(TItem item, ISet<TItem> visited, TItem[] sorted, ref int index, int incr, bool throwOnCycle, bool throwOnMissing)
        {
            if (visited.Contains(item))
            {
                // visited but not sorted yet = cycle
                var start = incr > 0 ? 0 : index;
                var count = incr > 0 ? index : sorted.Length - index;
                if (throwOnCycle && Contains(sorted, item, start, count) == false)
                {
                    throw new Exception(CycleDependencyError + ": " + item);
                }

                return;
            }

            visited.Add(item);

            IEnumerable<TKey>? keys = _getDependencies(item);
            IEnumerable<TItem>? dependencies = keys == null ? null : FindDependencies(keys, throwOnMissing);

            if (dependencies != null)
            {
                foreach (TItem dep in dependencies)
                {
                    Visit(dep, visited, sorted, ref index, incr, throwOnCycle, throwOnMissing);
                }
            }

            sorted[index] = item;
            index += incr;
        }

        private IEnumerable<TItem> FindDependencies(IEnumerable<TKey> keys, bool throwOnMissing)
        {
            foreach (TKey key in keys)
            {
                if (_items.TryGetValue(key, out TItem? value))
                {
                    yield return value;
                }
                else if (throwOnMissing)
                {
                    throw new Exception($"{MissingDependencyError} Error in type {typeof(TItem).Name}, with key {key}");
                }
            }
        }
    }
}
