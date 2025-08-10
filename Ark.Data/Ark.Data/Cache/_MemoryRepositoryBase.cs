using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Ark.Data
{
    /// <inheritdoc />
    /// <summary>
    /// The abstract class contains a in memory repository where data of a single type are stored using an unique key.
    /// </summary>
    /// <typeparam name="TKey">The type of key.</typeparam>
    /// <typeparam name="TValue">The type of value to store.</typeparam>
    public abstract class MemoryRepositoryBase<TKey, TValue> : CacheRepositoryBase<TKey, TValue>
    {
        #region Fields

        /// <summary>
        /// A dictionary used to store the data using unique keys
        /// </summary>
        protected static ConcurrentDictionary<TKey, TimedObject<TValue>> DataStore = new ConcurrentDictionary<TKey, TimedObject<TValue>>();

        #endregion Fields

        #region Methods (Public)

        /// <inheritdoc />
        public override TValue Get(TKey key, TValue valueIfNotFound = default)
        {
            DataStore.TryGetValue(key, out var timedObject);
            if (timedObject == null)
                return valueIfNotFound;

            if (ValidityTimeSpan != null && DateTime.UtcNow.Subtract(timedObject.Time) > ValidityTimeSpan.Value)
            {
                Remove(key);
                return valueIfNotFound;
            }

            return timedObject.Object;
        }

        /// <inheritdoc />
        public override TValue[] GetAll()
        {
            RemoveInvalidData();
            return DataStore.Values.Select(to => to.Object).ToArray();
        }

        /// <inheritdoc />
        public override void Set(TKey key, TValue dataToStore)
            => DataStore.AddOrUpdate(key, k => new TimedObject<TValue>(dataToStore), (k, o) => new TimedObject<TValue>(dataToStore));

        /// <inheritdoc />
        public override void SetOrResetWhole(IDictionary<TKey, TValue> data)
            => DataStore = new ConcurrentDictionary<TKey, TimedObject<TValue>>(data.ToDictionary(kvp => kvp.Key, kvp => new TimedObject<TValue>(kvp.Value)));

        /// <inheritdoc />
        public override void RemoveInvalidData()
        {
            if (ValidityTimeSpan == null)
                return;

            var keysToRemove = DataStore.Where(kvp => DateTime.UtcNow.Subtract(kvp.Value.Time) > ValidityTimeSpan.Value).Select(kvp => kvp.Key).ToArray();
            keysToRemove.ForEach(k => Remove(k));
        }

        /// <inheritdoc />
        public override bool Remove(TKey key)
            => DataStore.TryRemove(key, out _);

        /// <inheritdoc />
        public override void Clear()
            => DataStore.Clear();

        /// <inheritdoc />
        public override bool IsEmpty
            => DataStore.HasNoElements();

        /// <inheritdoc />
        public override bool Contains(TKey key)
            => DataStore.ContainsKey(key);

        #endregion Methods (Public)
    }
}