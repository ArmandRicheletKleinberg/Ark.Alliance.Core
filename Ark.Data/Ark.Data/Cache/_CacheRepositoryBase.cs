using System;
using System.Collections.Generic;

namespace Ark.Data
{
    /// <summary>
    /// The abstract class contains a in memory repository where data of a single type are stored using an unique key.
    /// </summary>
    /// <typeparam name="TKey">The type of key.</typeparam>
    /// <typeparam name="TValue">The type of value to store.</typeparam>
    public abstract class CacheRepositoryBase<TKey, TValue>
    {
        #region Methods (Public)

        /// <summary>
        /// Gets a data from the store given an unique key.
        /// </summary>
        /// <param name="key">The key to search the data store.</param>
        /// <param name="valueIfNotFound">The value to return if the data stored with this key is not found or the key is null.</param>
        /// <returns>The data stored if found or valueIfNotFound if not found.</returns>
        public abstract TValue Get(TKey key, TValue valueIfNotFound = default);

        /// <summary>
        /// Gets all the data stored from the store.
        /// </summary>
        /// <returns>All the data stored from the store.</returns>
        public abstract TValue[] GetAll();

        /// <summary>
        /// Sets a data to the store given an unique key.
        /// If there is already some data stored under this key, it will be replaced.
        /// </summary>
        /// <param name="key">The key to set in the data store.</param>
        /// <param name="dataToStore">The data to store in the data store given a unique key.</param>
        /// <returns>The data value stored.</returns>
        public abstract void Set(TKey key, TValue dataToStore);

        /// <summary>
        /// Sets or resets the whole cache using an initial data store.
        /// It replaces the instance in the cache.
        /// It is better to do this to reset the data store as this is atomic.
        /// </summary>
        /// <param name="data">The data to set or reset in the data store.</param>
        public abstract void SetOrResetWhole(IDictionary<TKey, TValue> data);

        /// <summary>
        /// Removes the data given its key and returns true if removed or false if not exists.
        /// If there is already some data stored under this key, it will be replaced.
        /// </summary>
        /// <param name="key">The key to check for data in the store.</param>
        /// <returns>True if removed or false if not exists.</returns>
        public abstract bool Remove(TKey key);

        /// <summary>
        /// Removes the data that is no more valid if a validity time span has been defined.
        /// </summary>
        public abstract void RemoveInvalidData();

        /// <summary>
        /// Clears the whole data store.
        /// </summary>
        public abstract void Clear();

        /// <summary>
        /// Whether the data store is empty.
        /// </summary>
        public abstract bool IsEmpty { get; }

        /// <summary>
        /// Whether the data store contains a specific key.
        /// </summary>
        public abstract bool Contains(TKey key);

        #endregion Methods (Public)

        #region Methods (Virtual)

        /// <summary>
        /// The timespan during which a store data stays valid.
        /// The data is invalidated after this time.
        /// Null for no validity check.
        /// </summary>
        protected virtual TimeSpan? ValidityTimeSpan => null;

        #endregion Methods (Virtual)
    }
}