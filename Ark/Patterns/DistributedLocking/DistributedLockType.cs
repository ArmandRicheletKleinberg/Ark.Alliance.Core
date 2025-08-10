namespace Ark
{
    /// <summary>
    /// + Differentiates read and write distributed locks.
    /// - Enum values may not map to specific implementations.
    /// </summary>
    public enum DistributedLockType
    {
        /// <summary>
        /// + Allows concurrent read access.
        /// - Writers are blocked while held.
        /// </summary>
        ReadLock,

        /// <summary>
        /// + Provides exclusive access for mutations.
        /// - Can starve read operations if held too long.
        /// </summary>
        WriteLock,
    }
}
