namespace Ark
{
    /// <summary>
    /// + Selects the optimal distributed locking mechanism among registered implementations.
    /// - Fallback strategy is implementation-dependent.
    /// </summary>
    public interface IDistributedLockingMechanismFactory
    {
        /// <summary>
        /// + Gets the selected locking mechanism to use at runtime.
        /// - Returns a no-op implementation if none are registered.
        /// </summary>
        IDistributedLockingMechanism DistributedLockingMechanism { get; }
    }
}
