using System;
using System.Collections.Generic;
using System.Linq;

namespace Ark.Data
{
    /// <inheritdoc />
    /// <summary>
    /// The abstract class contains a in memory repository where data of a single type are stored using an unique key which is defined in an enumeration.
    /// This is done for better lookup performance when no a lot of enumeration items. The array direct index is better than dictionary lookup.
    /// For better memory allocation, the enum should not manually set the integer value.
    /// Beware, this cache is not thread safe.
    /// UNLESS YOU NEED EXTREME PERFORMANCE, ALWAYS PREFER THE NORMAL MEMORYREPOSITORYBASE.
    /// </summary>
    /// <typeparam name="TKey">The type of key.</typeparam>
    /// <typeparam name="TValue">The type of value to store.</typeparam>
    public abstract class MemoryWithEnumKeyRepositoryBase<TKey, TValue> : CacheRepositoryBase<TKey, TValue>
        where TKey : struct, IComparable
        where TValue : class
    {
        #region Fields

        /// <summary>
        /// A dictionary used to store the data using unique keys
        /// </summary>
        protected static TimedObject<TValue>[] DataStore;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Creates a <see cref="MemoryWithEnumKeyRepositoryBase{Tkey, Tvalue}"/> instance.
        /// </summary>
        protected MemoryWithEnumKeyRepositoryBase()
        {
            if (DataStore != null)
                return;

            var maxValue = Enum.GetValues(typeof(TKey)).Cast<int>().Max();
            DataStore = new TimedObject<TValue>[maxValue + 1];
        }

        #endregion Constructors

        #region Methods (Public)

        /// <inheritdoc />
        public override TValue Get(TKey key, TValue valueIfNotFound = default)
        {
            var timedObject = DataStore[(int)(object)key];
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
            return DataStore.Select(to => to.Object).IfNotNull().ToArray();
        }

        /// <inheritdoc />
        public override void Set(TKey key, TValue dataToStore)
            => DataStore[(int)(object)key] = new TimedObject<TValue>(dataToStore);

        /// <inheritdoc />
        public override void SetOrResetWhole(IDictionary<TKey, TValue> data)
            => throw new NotImplementedException();

        /// <inheritdoc />
        public override void RemoveInvalidData()
        {
            for (var counter = 0; counter < DataStore.Length; counter++)
            {
                var timedObject = DataStore[counter];
                if (ValidityTimeSpan != null && DateTime.UtcNow.Subtract(timedObject.Time) > ValidityTimeSpan.Value)
                    DataStore[counter] = null;
            }
        }

        /// <inheritdoc />
        public override bool Remove(TKey key)
        {
            DataStore[(int)(object)key] = null;
            return true;
        }

        /// <inheritdoc />
        public override void Clear()
        {
            var maxValue = Enum.GetValues(typeof(TKey)).Cast<int>().Max();
            DataStore = new TimedObject<TValue>[maxValue + 1];
        }

        /// <inheritdoc />
        public override bool IsEmpty
            => DataStore.HasNoElements();

        /// <inheritdoc />
        public override bool Contains(TKey key)
            => DataStore[(int)(object)key] != null;

        #endregion Methods (Public)
    }
}