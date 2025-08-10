

namespace Ark
{
    /// <summary>
    /// + Abstraction for obtaining distributed read/write locks.
    /// - Does not define lock fairness or deadlock avoidance.
    /// </summary>
    /// <remarks>
    ///     In general the rules for distributed locks are as follows.
    ///     <list type="bullet">
    ///         <item>
    ///             <b>Cannot</b> obtain a write lock if a read lock exists for same lock id (except during an upgrade from
    ///             reader -> writer)
    ///         </item>
    ///         <item>
    ///             <b>Cannot</b> obtain a write lock if a write lock exists for same lock id.
    ///         </item>
    ///         <item>
    ///             <b>Cannot</b> obtain a read lock if a write lock exists for same lock id.
    ///         </item>
    ///         <item>
    ///             <b>Can</b> obtain a read lock if a read lock exists for same lock id.
    ///         </item>
    ///     </list>
    /// </remarks>
    public interface IDistributedLockingMechanism
    {
        /// <summary>
        ///     Gets a value indicating whether this distributed locking mechanism can be used.
        /// </summary>
        bool Enabled { get; }

        /// <summary>
        ///     Obtains a distributed read lock.
        /// </summary>
        /// <remarks>
        ///     When <paramref name="obtainLockTimeout" /> is <c>null</c>,
        ///     implementations should use the globally configured read-lock
        ///     timeout.
        /// </remarks>
        /// <exception cref="DistributedReadLockTimeoutException">Failed to obtain distributed read lock in time.</exception>
        IDistributedLock ReadLock(int lockId, TimeSpan? obtainLockTimeout = null);

        /// <summary>
        ///     Obtains a distributed write lock.
        /// </summary>
        /// <remarks>
        ///     When <paramref name="obtainLockTimeout" /> is <c>null</c>,
        ///     implementations should use the globally configured write-lock
        ///     timeout.
        /// </remarks>
        /// <exception cref="DistributedWriteLockTimeoutException">Failed to obtain distributed write lock in time.</exception>
        IDistributedLock WriteLock(int lockId, TimeSpan? obtainLockTimeout = null);
    }
}
