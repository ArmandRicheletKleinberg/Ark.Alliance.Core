using System.Collections;

#nullable enable

namespace Ark;

/// <summary>
/// Provides a thread-safe wrapper around <see cref="HashSet{T}"/> using <see cref="ReaderWriterLockSlim"/> locks.
/// <para>+ Safe concurrent reads and writes without external synchronization.</para>
/// <para>- Enumeration clones the set and may allocate; avoid on hot paths.</para>
/// <para>Ref: <see href="https://learn.microsoft.com/dotnet/api/system.collections.generic.hashset-1"/></para>
/// </summary>
/// <typeparam name="T">Type of elements stored in the set.</typeparam>
[Serializable]
public class ConcurrentHashSet<T> : ICollection<T>
{
    #region Fields
    private readonly HashSet<T> _innerSet = new();
    private readonly ReaderWriterLockSlim _instanceLocker = new(LockRecursionPolicy.NoRecursion);
    #endregion Fields

    #region Properties
    /// <summary>
    /// Gets the number of elements contained in the collection.
    /// <para>+ O(1) lookup guarded by a read lock.</para>
    /// <para>- Count may change immediately after the lock is released.</para>
    /// </summary>
    public int Count
    {
        get
        {
            try
            {
                _instanceLocker.EnterReadLock();
                return _innerSet.Count;
            }
            finally
            {
                if (_instanceLocker.IsReadLockHeld)
                {
                    _instanceLocker.ExitReadLock();
                }
            }
        }
    }

    /// <summary>
    /// Indicates whether the set is read-only.
    /// <para>+ Always <see langword="false"/> enabling modifications.</para>
    /// <para>- Cannot be toggled to immutable at runtime.</para>
    /// </summary>
    public bool IsReadOnly => false;
    #endregion Properties

    #region Methods
    /// <summary>
    /// Adds an item to the set.
    /// <para>+ Write lock ensures exclusive access.</para>
    /// <para>- No feedback when an item already exists; use <see cref="TryAdd"/> to know.</para>
    /// </summary>
    /// <param name="item">Item to insert.</param>
    public void Add(T item)
    {
        try
        {
            _instanceLocker.EnterWriteLock();
            _innerSet.Add(item);
        }
        finally
        {
            if (_instanceLocker.IsWriteLockHeld)
            {
                _instanceLocker.ExitWriteLock();
            }
        }
    }

    /// <summary>
    /// Attempts to add an item only if it does not already exist.
    /// <para>+ Prevents duplicates in concurrent scenarios.</para>
    /// <para>- Performs two lookups which may impact large sets.</para>
    /// </summary>
    /// <param name="item">Item to add.</param>
    /// <returns><see langword="true"/> if the item was added; otherwise <see langword="false"/>. JSON: <c>true</c>/<c>false</c>.</returns>
    public bool TryAdd(T item)
    {
        if (Contains(item))
        {
            return false;
        }

        try
        {
            _instanceLocker.EnterWriteLock();

            if (_innerSet.Contains(item))
            {
                return false;
            }

            _innerSet.Add(item);
            return true;
        }
        finally
        {
            if (_instanceLocker.IsWriteLockHeld)
            {
                _instanceLocker.ExitWriteLock();
            }
        }
    }

    /// <summary>
    /// Removes all items from the set.
    /// <para>+ Leaves the instance reusable without allocating a new set.</para>
    /// <para>- Releases references held by contained elements.</para>
    /// </summary>
    public void Clear()
    {
        try
        {
            _instanceLocker.EnterWriteLock();
            _innerSet.Clear();
        }
        finally
        {
            if (_instanceLocker.IsWriteLockHeld)
            {
                _instanceLocker.ExitWriteLock();
            }
        }
    }

    /// <summary>
    /// Determines whether the set contains a specific value.
    /// <para>+ Read lock ensures a consistent view.</para>
    /// <para>- May block writers while the read lock is held.</para>
    /// </summary>
    /// <param name="item">Item to locate.</param>
    /// <returns><see langword="true"/> if found; otherwise <see langword="false"/>. JSON: <c>true</c>/<c>false</c>.</returns>
    public bool Contains(T item)
    {
        try
        {
            _instanceLocker.EnterReadLock();
            return _innerSet.Contains(item);
        }
        finally
        {
            if (_instanceLocker.IsReadLockHeld)
            {
                _instanceLocker.ExitReadLock();
            }
        }
    }

    /// <summary>
    /// Removes the first occurrence of an item from the set.
    /// <para>+ Write lock guarantees exclusive access.</para>
    /// <para>- Returns <see langword="false"/> when the item is absent.</para>
    /// </summary>
    /// <param name="item">Item to remove.</param>
    /// <returns><see langword="true"/> if the item was removed; otherwise <see langword="false"/>. JSON: <c>true</c>/<c>false</c>.</returns>
    public bool Remove(T item)
    {
        try
        {
            _instanceLocker.EnterWriteLock();
            return _innerSet.Remove(item);
        }
        finally
        {
            if (_instanceLocker.IsWriteLockHeld)
            {
                _instanceLocker.ExitWriteLock();
            }
        }
    }

    /// <summary>
    /// Returns an enumerator that iterates over a thread-safe snapshot of the set.
    /// <para>+ Enumeration is unaffected by concurrent modifications.</para>
    /// <para>- Snapshot allocation occurs on each call.</para>
    /// </summary>
    /// <returns>An enumerator over a cloned set.</returns>
    public IEnumerator<T> GetEnumerator() => GetThreadSafeClone().GetEnumerator();

    /// <summary>
    /// Returns a non-generic enumerator for the snapshot.
    /// <para>Ref: <see href="https://learn.microsoft.com/dotnet/api/system.collections.ienumerable.getenumerator"/></para>
    /// </summary>
    /// <returns>An <see cref="IEnumerator"/> over a cloned set.</returns>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Copies the elements of the set to an array starting at the specified index.
    /// <para>+ Uses a snapshot for thread safety.</para>
    /// <para>- Snapshot creation may allocate.</para>
    /// </summary>
    /// <param name="array">Destination array.</param>
    /// <param name="arrayIndex">Zero-based index in <paramref name="array"/> where copying begins.</param>
    public void CopyTo(T[] array, int arrayIndex)
    {
        var clone = GetThreadSafeClone();
        clone.CopyTo(array, arrayIndex);
    }

    /// <summary>
    /// Copies the elements of the set to a non-generic <see cref="Array"/>.
    /// <para>+ Compatible with legacy APIs.</para>
    /// <para>- Requires boxing for value types.</para>
    /// </summary>
    /// <param name="array">Destination array.</param>
    /// <param name="index">Zero-based starting index.</param>
    public void CopyTo(Array array, int index)
    {
        var clone = GetThreadSafeClone();
        Array.Copy(clone.ToArray(), 0, array, index, clone.Count);
    }
    #endregion Methods

    #region Helpers
    private HashSet<T> GetThreadSafeClone()
    {
        HashSet<T>? clone = null;
        try
        {
            _instanceLocker.EnterReadLock();
            clone = new HashSet<T>(_innerSet, _innerSet.Comparer);
        }
        finally
        {
            if (_instanceLocker.IsReadLockHeld)
            {
                _instanceLocker.ExitReadLock();
            }
        }

        return clone!;
    }
    #endregion Helpers
}

