using System.Collections;
using System.Collections.Concurrent;

namespace Ark
{
    /// <summary>
    /// Thread-safe set implementation backed by <see cref="ConcurrentDictionary{TKey,TValue}"/>.
    /// </summary>
    /// <typeparam name="T">Type of items stored in the set.</typeparam>
    public class ConcurrentSet<T> : ISet<T>
    {
        private readonly ConcurrentDictionary<T, byte> _dictionary = new();

        /// <summary>
        /// Attempts to add an item to the set.
        /// </summary>
        public bool TryAdd(T item) => _dictionary.TryAdd(item, default);

        /// <summary>
        /// Attempts to remove the given item from the set.
        /// </summary>
        public bool TryRemove(T item) => _dictionary.TryRemove(item, out _);

        /// <summary>Values contained in the set.</summary>
        public ICollection<T> Values => _dictionary.Keys;

        /// <summary>True when the set contains no items.</summary>
        public bool IsEmpty => _dictionary.IsEmpty;

        /// <summary>Returns an enumerator over the set.</summary>
        public IEnumerator<T> GetEnumerator() => _dictionary.Keys.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>Copies items to a new array.</summary>
        public T[] ToArray() => _dictionary.Keys.ToArray();

        /// <inheritdoc />
        public int Count => _dictionary.Count;

        /// <inheritdoc />
        public bool IsReadOnly => false;

        void ICollection<T>.Add(T item)
        {
            if (item == null) throw new ArgumentException("Item is null.");
            if (!Add(item)) throw new ArgumentException("Item already exists in set.");
        }

        /// <inheritdoc />
        public void Clear() => _dictionary.Clear();

        /// <inheritdoc />
        public bool Contains(T item) => item != null && _dictionary.ContainsKey(item);

        /// <inheritdoc />
        public void CopyTo(T[] array, int arrayIndex) => Values.CopyTo(array, arrayIndex);

        /// <inheritdoc />
        public bool Remove(T item) => TryRemove(item);

        /// <inheritdoc />
        public bool Add(T item) => TryAdd(item);

        /// <inheritdoc />
        public void ExceptWith(IEnumerable<T> other)
        {
            foreach (var item in other)
                TryRemove(item);
        }

        /// <inheritdoc />
        public void IntersectWith(IEnumerable<T> other)
        {
            var enumerable = other as IList<T> ?? other.ToArray();
            foreach (var item in this)
                if (!enumerable.Contains(item))
                    TryRemove(item);
        }

        /// <inheritdoc />
        public bool IsSupersetOf(IEnumerable<T> other) => other.AsParallel().All(Contains);

        /// <inheritdoc />
        public bool IsSubsetOf(IEnumerable<T> other)
        {
            var enumerable = other as IList<T> ?? other.ToArray();
            return this.AsParallel().All(enumerable.Contains);
        }

        /// <inheritdoc />
        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            var enumerable = other as IList<T> ?? other.ToArray();
            return Count != enumerable.Count && IsSubsetOf(enumerable);
        }

        /// <inheritdoc />
        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            var enumerable = other as IList<T> ?? other.ToArray();
            return Count != enumerable.Count && IsSupersetOf(enumerable);
        }

        /// <inheritdoc />
        public bool Overlaps(IEnumerable<T> other) => other.AsParallel().Any(Contains);

        /// <inheritdoc />
        public bool SetEquals(IEnumerable<T> other)
        {
            var enumerable = other as IList<T> ?? other.ToArray();
            return Count == enumerable.Count && enumerable.AsParallel().All(Contains);
        }

        /// <inheritdoc />
        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            foreach (var item in other)
            {
                if (!TryRemove(item))
                    TryAdd(item);
            }
        }

        /// <inheritdoc />
        public void UnionWith(IEnumerable<T> other)
        {
            foreach (var item in other)
                TryAdd(item);
        }
    }
}
