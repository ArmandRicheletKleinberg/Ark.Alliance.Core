using System.Collections.ObjectModel;
using System.Collections.Specialized;

#nullable enable
namespace Ark;

/// <summary>
/// Provides a dictionary-like <see cref="ObservableCollection{T}"/> where items are indexed by a key
/// generated from their value.
/// <para>+ Raises change notifications while enabling O(1) lookups by key.</para>
/// <para>- Keys are derived from values and must remain stable for the lifetime of the item.</para>
/// <para>Ref: <see href="https://learn.microsoft.com/dotnet/api/system.collections.specialized.inotifycollectionchanged"/></para>
/// </summary>
/// <typeparam name="TKey">Type used as the key for lookup operations.</typeparam>
/// <typeparam name="TValue">Type of elements stored in the collection.</typeparam>
/// <example>
/// <code language="csharp">
/// var dict = new ObservableDictionary&lt;int, string&gt;(s => s.Length);
/// dict.Add("ark");
/// bool exists = dict.ContainsKey(3); // true
/// </code>
/// </example>
public class ObservableDictionary<TKey, TValue> : ObservableCollection<TValue>, IReadOnlyDictionary<TKey, TValue>,
    IDictionary<TKey, TValue>, INotifyCollectionChanged
    where TKey : notnull
{
        // need to explicitly implement with event accessor syntax in order to override in order to to clear
        // c# events are weird, they do not behave the same way as other c# things that are 'virtual',
        // a good article is here: https://medium.com/@unicorn_dev/virtual-events-in-c-something-went-wrong-c6f6f5fbe252
        // and https://stackoverflow.com/questions/2268065/c-sharp-language-design-explicit-interface-implementation-of-an-event
        private NotifyCollectionChangedEventHandler? _changed;

        /// <summary>
        /// Creates a new instance of the <see cref="ObservableDictionary{TKey, TValue}"/> class.
        /// <para>+ Allows custom key derivation via <paramref name="keySelector"/>.</para>
        /// <para>- Assumes keys remain stable after insertion.</para>
        /// </summary>
        /// <param name="keySelector">Function generating a key from each value.</param>
        /// <param name="equalityComparer">Comparer used for key equality; <c>null</c> uses the default.</param>
        public ObservableDictionary(Func<TValue, TKey> keySelector, IEqualityComparer<TKey>? equalityComparer = null)
        {
            KeySelector = keySelector ?? throw new ArgumentException(nameof(keySelector));
            Indecies = new Dictionary<TKey, int>(equalityComparer);
        }

        /// <summary>
        /// Index mapping between keys and their positions in the underlying list.
        /// <para>+ Enables constant-time lookups.</para>
        /// <para>- Exposed as <see langword="protected"/> for extension only.</para>
        /// </summary>
        protected Dictionary<TKey, int> Indecies { get; }

        /// <summary>
        /// Delegate used to extract a key from a value.
        /// <para>+ Centralized key generation logic.</para>
        /// <para>- Must be side-effect free to avoid inconsistent keys.</para>
        /// </summary>
        protected Func<TValue, TKey> KeySelector { get; }

        /// <summary>
        /// Removes the item associated with the specified key.
        /// <para>+ Updates the internal index to maintain consistency.</para>
        /// <para>- Returns <c>false</c> if the key is absent.</para>
        /// </summary>
        /// <param name="key">Key of the item to remove.</param>
        /// <returns><c>true</c> if the item was found and removed; otherwise <c>false</c>.</returns>
        public bool Remove(TKey key)
        {
            if (!Indecies.TryGetValue(key, out int index))
            {
                return false;
            }

            RemoveAt(index);
            return true;
        }

        event NotifyCollectionChangedEventHandler? INotifyCollectionChanged.CollectionChanged
        {
            add => _changed += value;
            remove => _changed -= value;
        }

        /// <summary>
        /// Determines whether the collection contains the specified key.
        /// <para>+ O(1) lookup using the internal index map.</para>
        /// </summary>
        /// <param name="key">Key to locate.</param>
        /// <returns><c>true</c> if the key exists; otherwise <c>false</c>.</returns>
        public bool ContainsKey(TKey key) => Indecies.ContainsKey(key);

        /// <summary>
        /// Gets or sets the element with the specified key.
        /// <para>+ Setting a value updates existing entries or adds new ones.</para>
        /// <para>- Setting a value with a mismatched key throws <see cref="InvalidOperationException"/>.</para>
        /// </summary>
        /// <param name="key">Key of element to access.</param>
        /// <returns>The value associated with the key.</returns>
        public TValue this[TKey key]
        {
            get => this[Indecies[key]];
            set
            {
                // confirm key matches
                if (!KeySelector(value)!.Equals(key))
                {
                    throw new InvalidOperationException("Key of new value does not match.");
                }

                if (!Indecies.TryGetValue(key, out int index))
                {
                    Add(value);
                }
                else
                {
                    this[index] = value;
                }
            }
        }

        /// <summary>
        /// Clears all <see cref="INotifyCollectionChanged.CollectionChanged"/> event handlers.
        /// <para>+ Releases references to subscribers.</para>
        /// <para>- Subsequent changes will not notify previous listeners.</para>
        /// </summary>
        public void ClearCollectionChangedEvents() => _changed = null;

        /// <summary>
        /// Replaces the element associated with the specified key.
        /// <para>+ Keeps ordering intact by updating in place.</para>
        /// <para>- Throws if the new value produces a different key.</para>
        /// </summary>
        /// <param name="key">Key of element to replace.</param>
        /// <param name="value">New value.</param>
        /// <returns><c>true</c> if replaced; <c>false</c> if key not found.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the new value's key differs.</exception>
        public bool Replace(TKey key, TValue value)
        {
            if (!Indecies.TryGetValue(key, out int index))
            {
                return false;
            }

            // confirm key matches
            if (!KeySelector(value)!.Equals(key))
            {
                throw new InvalidOperationException("Key of new value does not match.");
            }

            this[index] = value;
            return true;
        }

        /// <summary>
        /// Replaces the entire contents of the dictionary with the provided values.
        /// <para>+ Ensures keys stay consistent by using the <c>Add</c> method.</para>
        /// <para>- Throws <see cref="ArgumentNullException"/> when <paramref name="values"/> is <c>null</c>.</para>
        /// </summary>
        /// <param name="values">Sequence of values to load into the collection.</param>
        public void ReplaceAll(IEnumerable<TValue> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            Clear();

            foreach (TValue value in values)
            {
                Add(value);
            }
        }

        /// <summary>
        /// Changes the key associated with an existing item.
        /// <para>+ Useful when the key needs to be updated without recreating the item.</para>
        /// <para>- Throws if the new key already exists.</para>
        /// </summary>
        /// <param name="currentKey">Existing key of the item.</param>
        /// <param name="newKey">New key to associate with the item.</param>
        public void ChangeKey(TKey currentKey, TKey newKey)
        {
            if (!Indecies.TryGetValue(currentKey, out int currentIndex))
            {
                throw new InvalidOperationException($"No item with the key '{currentKey}' was found in the dictionary.");
            }

            if (ContainsKey(newKey))
            {
                throw new ArgumentException($"An element with the same key '{newKey}' already exists in the dictionary.", nameof(newKey));
            }

            Indecies.Remove(currentKey);
            Indecies.Add(newKey, currentIndex);
        }

        #region Protected Methods

        /// <summary>
        /// Inserts an item into the collection at the specified index.
        /// <para>+ Updates internal index positions for subsequent items.</para>
        /// <para>- Throws if an item with the same key already exists.</para>
        /// </summary>
        /// <param name="index">Zero-based index at which the item should be inserted.</param>
        /// <param name="item">Item to insert.</param>
        protected override void InsertItem(int index, TValue item)
        {
            TKey key = KeySelector(item);
            if (Indecies.ContainsKey(key))
            {
                throw new ArgumentException($"An element with the same key '{key}' already exists in the dictionary.", nameof(item));
            }

            if (index != Count)
            {
                foreach (KeyValuePair<TKey, int> largerOrEqualToIndex in Indecies.Where(kvp => kvp.Value >= index))
                {
                    Indecies[largerOrEqualToIndex.Key] = largerOrEqualToIndex.Value + 1;
                }
            }

            base.InsertItem(index, item);
            Indecies[key] = index;
        }

        /// <summary>
        /// Clears the collection and the associated key index map.
        /// <para>+ Ensures no stale key mappings remain.</para>
        /// </summary>
        protected override void ClearItems()
        {
            base.ClearItems();
            Indecies.Clear();
        }

        /// <summary>
        /// Removes the item at the specified index and updates key mappings.
        /// <para>+ Maintains correct indices for remaining items.</para>
        /// </summary>
        /// <param name="index">Index of the item to remove.</param>
        protected override void RemoveItem(int index)
        {
            TValue item = this[index];
            TKey key = KeySelector(item);

            base.RemoveItem(index);

            Indecies.Remove(key);

            foreach (KeyValuePair<TKey, int> largerThanIndex in Indecies.Where(kvp => kvp.Value > index))
            {
                Indecies[largerThanIndex.Key] = largerThanIndex.Value - 1;
            }
        }

        #endregion

        #region IDictionary and IReadOnlyDictionary implementation

        /// <summary>
        /// Attempts to get the value associated with the specified key.
        /// <para>+ Avoids exceptions when keys are missing.</para>
        /// </summary>
        /// <param name="key">Key to locate.</param>
        /// <param name="val">When this method returns, contains the value associated with the specified key, if the key is found; otherwise, the default value for the type of the value parameter.</param>
        /// <returns><c>true</c> if the key was found; otherwise <c>false</c>.</returns>
        public bool TryGetValue(TKey key, out TValue val)
        {
            if (Indecies.TryGetValue(key, out var index))
            {
                val = this[index];
                return true;
            }

            val = default!;
            return false;
        }

        /// <summary>
        /// Returns an enumerable view of all keys in the dictionary.
        /// <para>+ Reflects current ordering of items.</para>
        /// </summary>
        public IEnumerable<TKey> Keys => Indecies.Keys;

        /// <summary>
        /// Returns an enumerable view of all values in the dictionary.
        /// <para>+ Delegates to the underlying <see cref="ObservableCollection{T}"/>.</para>
        /// </summary>
        public IEnumerable<TValue> Values => Items;

        ICollection<TKey> IDictionary<TKey, TValue>.Keys => Indecies.Keys;

        // this will never be used
        ICollection<TValue> IDictionary<TKey, TValue>.Values => Values.ToList();

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            foreach (TValue i in Values)
            {
                TKey key = KeySelector(i);
                yield return new KeyValuePair<TKey, TValue>(key, i);
            }
        }

        void IDictionary<TKey, TValue>.Add(TKey key, TValue value) => Add(value);

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item) => Add(item.Value);

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item) => ContainsKey(item.Key);

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) =>
            throw new NotImplementedException();

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item) => Remove(item.Key);

        #endregion
    }

